/*
Nombre:		    PGS
Fecha:		    20111017
Descripción:	Configuración específica para almacenamiento de Numeros en Directorio.
 * Modificación:	2012-03-22 PGS. Se agrega ReEtiquetación de Llamadas en GrabarRegistro.
*/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using KeytiaServiceBL;
using System.Text;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;
using System.Data;
using DSOControls2008;
using System.Web.UI;

namespace KeytiaWeb.UserInterface
{
    public class CnfgDirectoriosEdit : HistoricEdit
    {
        protected string[] pFieldsNoVisibles;
        protected static string piCodEmpleado = "";
        protected int piHisPreviaEtiqueta;
        protected bool pbEtiquetarUnaVez;
        protected int piDiaCorte = 1;
        protected int piDiaLimite;
        protected DateTime pdtCorte;
        protected DateTime pdtLimite;
        protected bool pbEnableEmpleado = true;

        public override void LoadScripts()
        {
            base.LoadScripts();            
        }

        protected override void InitAccionesSecundarias()
        {
            DisableFields();

            DataTable ldt;
            KeytiaServiceBL.KDBAccess lKDB = new KeytiaServiceBL.KDBAccess();
            ldt = lKDB.GetHisRegByEnt("Emple", "Empleados", "{Usuar}='" + HttpContext.Current.Session["iCodUsuario"].ToString() + "'");
            if (ldt == null || ldt.Rows.Count == 0)
            {
                string lsTitulo = Globals.GetMsgWeb(false, "TituloHistoricos");
                string lsError = DSOControl.JScriptEncode(Globals.GetMsgWeb(false, "UsuarioSinEmpleado"));
                DSOControl.jAlert(Page, pjsObj, lsError, lsTitulo);
                pbEnableEmpleado = false;
                return;
            }                
            piCodEmpleado = ldt.Rows[0]["iCodCatalogo"].ToString();
            GetClientConfig();
        }

        protected override void pbtnAgregar_ServerClick(object sender, EventArgs e)
        {
            int liDiaCorte;
            int liDiaLimite;
            DateTime ldtIniVigencia;

            liDiaLimite = EtiquetaEdit.ValidarDiasMes(DateTime.Today.Year, DateTime.Today.Month, piDiaLimite);
            pdtLimite = new DateTime(DateTime.Today.Year, DateTime.Today.Month, liDiaLimite);

            if (piDiaCorte >= DateTime.Today.Day)
            {
                liDiaCorte = EtiquetaEdit.ValidarDiasMes(DateTime.Today.AddMonths(-1).Year, DateTime.Today.AddMonths(-1).Month, piDiaCorte);
                pdtCorte = new DateTime(DateTime.Today.AddMonths(-1).Year, DateTime.Today.AddMonths(-1).Month, liDiaCorte);
            }
            else
            {
                liDiaCorte = EtiquetaEdit.ValidarDiasMes(DateTime.Today.Year, DateTime.Today.Month, piDiaCorte);
                pdtCorte = new DateTime(DateTime.Today.Year, DateTime.Today.Month, liDiaCorte);
            }

            if (DateTime.Today <= pdtLimite)
            {
                int liDiaIniPeriodo = EtiquetaEdit.ValidarDiasMes(pdtCorte.AddMonths(-1).Year, pdtCorte.AddMonths(-1).Month, piDiaCorte);
                ldtIniVigencia = new DateTime(pdtCorte.AddMonths(-1).Year, pdtCorte.AddMonths(-1).Month, liDiaIniPeriodo);
            }
            else
            {
                ldtIniVigencia = pdtCorte;
            }

            pdtIniVigencia.MinDateTime = ldtIniVigencia;
            pFields.IniVigencia = ldtIniVigencia;

            AgregarRegistro();

            pdtIniVigencia.DataValue = ldtIniVigencia;
            
            FirePostAgregarClick();
        }

        protected override void pbtnEditar_ServerClick(object sender, EventArgs e)
        {
            EditarRegistro();

            int liDiaCorte;
            DateTime ldtIniVigencia;

            if (piDiaCorte >= DateTime.Today.Day)
            {
                liDiaCorte = EtiquetaEdit.ValidarDiasMes(DateTime.Today.AddMonths(-1).Year, DateTime.Today.AddMonths(-1).Month, piDiaCorte);
                pdtCorte = new DateTime(DateTime.Today.AddMonths(-1).Year, DateTime.Today.AddMonths(-1).Month, liDiaCorte);
            }
            else
            {
                liDiaCorte = EtiquetaEdit.ValidarDiasMes(DateTime.Today.Year, DateTime.Today.Month, piDiaCorte);
                pdtCorte = new DateTime(DateTime.Today.Year, DateTime.Today.Month, liDiaCorte);
            }

            int liDiaIniPeriodo = EtiquetaEdit.ValidarDiasMes(pdtCorte.AddMonths(-1).Year, pdtCorte.AddMonths(-1).Month, piDiaCorte);
            ldtIniVigencia = new DateTime(pdtCorte.AddMonths(-1).Year, pdtCorte.AddMonths(-1).Month, liDiaIniPeriodo);
            if (pdtIniVigencia.Date >= ldtIniVigencia)
            {
                pdtIniVigencia.MinDateTime = ldtIniVigencia;
            }
            else
            {
                pdtIniVigencia.MinDateTime = pdtIniVigencia.Date;
            }

            if (State == HistoricState.Baja)
            {
                pFields.DisableFields();
            }
            
            FirePostEditarClick();
        }

        protected void GetClientConfig()
        {
            DataRow ldrCliente;
            DataTable ldtEmpleColaborador = new DataTable();
            piHisPreviaEtiqueta = 0;
            pbEtiquetarUnaVez = false;

            ldrCliente = KeytiaServiceBL.Alarmas.Alarma.getCliente(int.Parse(piCodEmpleado));
            if (ldrCliente == null)
            {
                return;
            }

            if ((((int)Util.IsDBNull(ldrCliente["{BanderasEtiquetacion}"], 0) & 0x01) / 0x01) == 1)
            {
                piHisPreviaEtiqueta = 1;
            }
            if ((((int)Util.IsDBNull(ldrCliente["{BanderasEtiquetacion}"], 0) & 0x04) / 0x04) == 1)
            {
                pbEtiquetarUnaVez = true;
            }
            piDiaCorte = int.Parse(Util.IsDBNull(ldrCliente["{DiaEtiquetacion}"], 1).ToString());            
            piDiaLimite = int.Parse(Util.IsDBNull(ldrCliente["{DiaLmtEtiquetacion}"], 0).ToString());           

            if (piDiaCorte > DateTime.Today.Day)
            {
                int liDiaCorte = EtiquetaEdit.ValidarDiasMes(DateTime.Today.AddMonths(-1).Year, DateTime.Today.AddMonths(-1).Month, piDiaCorte);
                pdtCorte = new DateTime(DateTime.Today.AddMonths(-1).Year, DateTime.Today.AddMonths(-1).Month, liDiaCorte);
            }
            else
            {
                int liDiaCorte = EtiquetaEdit.ValidarDiasMes(DateTime.Today.Year, DateTime.Today.Month, piDiaCorte);
                pdtCorte = new DateTime(DateTime.Today.Year, DateTime.Today.Month, liDiaCorte);
            }

            int liDiaLimite = EtiquetaEdit.ValidarDiasMes(DateTime.Today.Year, DateTime.Today.Month, piDiaLimite);
            pdtLimite = new DateTime(DateTime.Today.Year, DateTime.Today.Month, liDiaLimite);
        }

        public override void InitLanguage()
        {
            base.InitLanguage();
            if (pFields == null)
            {
                return;
            }

            pvchCodigo.Descripcion = Globals.GetMsgWeb(false, "NumeroTel");
            pvchDescripcion.Descripcion = "";

            if (State == HistoricState.Edicion && pFields.ContainsConfigName("GEtiqueta"))
            {
                DSODropDownList oDropDown = (DSODropDownList)pFields.GetByConfigName("GEtiqueta").DSOControlDB;
                if (oDropDown.DropDownList.DataSource != null)
                {
                    oDropDown.DropDownList.Items.Remove(oDropDown.DropDownList.Items.FindByValue("0"));
                }
            }

            if (pbEnableEmpleado && pFields.ContainsConfigName("Etiqueta"))
            {
                DataRow ldrCliente = KeytiaServiceBL.Alarmas.Alarma.getCliente(int.Parse(piCodEmpleado));
                if (ldrCliente != null && ldrCliente.ItemArray.Length != 0)
                {
                    DSOTextBox oTextBox = (DSOTextBox)pFields.GetByConfigName("Etiqueta").DSOControlDB;
                    oTextBox.TextBox.MaxLength = (int)(KeytiaServiceBL.Util.IsDBNull(ldrCliente["{LongEtiqueta}"], 40));
                }
            }
        }

        public static DSOGridServerResponse GetHisDataDirEmple(DSOGridServerRequest gsRequest, int iCodEntidad, int iCodMaestro)
        {
            if (HttpContext.Current.Session["iCodUsuario"] == null)
            {
                string lsTitulo = Globals.GetMsgWeb(false, "TituloHistoricos");
                throw new KeytiaWebSessionException(true, lsTitulo);                
            }

            try
            {
                KDBAccess lKDB = new KDBAccess();
                HistoricFieldCollection lFields = new HistoricFieldCollection(iCodEntidad, iCodMaestro);

                StringBuilder lsbSelectTop = new StringBuilder();
                StringBuilder lsbColumnas = new StringBuilder();
                StringBuilder lsbFrom = new StringBuilder();
                StringBuilder lsbWhere = new StringBuilder();
                StringBuilder lsbOrderBy = new StringBuilder();
                DateTime ldtVigencia = DateTime.Now;

                DataTable ldtAtributos = HistoricFieldCollection.GetAtrib();
                int liCodIdioma = (int)ldtAtributos.Select("vchCodigo = '" + Globals.GetCurrentLanguage() + "'")[0]["iCodCatalogo"];

                string lsDescMaestro = DSODataAccess.Execute("select vchDescripcion from [" + DSODataContext.Schema + "].Maestros Where iCodRegistro = " + iCodMaestro.ToString()).Rows[0]["vchDescripcion"].ToString();
                bool lbMaestroDir = (lsDescMaestro == "Directorio Telefonico" ? true : false);

                DSOGridServerResponse lgsrRet = new DSOGridServerResponse();
                DataTable ldt;

                string lsOrderCol = "";
                string lsOrderDir = "";
                string lsOrderDirInv = "";
                string liCodEmpleado = "''";

                ldt = lKDB.GetHisRegByEnt("Emple", "Empleados", "{Usuar}='" + HttpContext.Current.Session["iCodUsuario"].ToString() + "'");
                if (ldt != null && ldt.Rows.Count > 0)
                {
                    liCodEmpleado = ldt.Rows[0]["iCodCatalogo"].ToString();
                }

                if (gsRequest.iSortCol.Count > 0)
                {
                    lsOrderCol = gsRequest.sColumns.Split(new string[] { "," }, StringSplitOptions.RemoveEmptyEntries)[gsRequest.iSortCol[0]];

                    if (!lFields.Contains(lsOrderCol)
                        && lsOrderCol != "vchCodigo"
                        && lsOrderCol != "vchDescripcion"
                        && lsOrderCol != "dtIniVigencia"
                        && lsOrderCol != "dtFinVigencia"
                        && lsOrderCol != "Consultar")
                    {
                        lsOrderCol = "dtFecUltAct";
                    }

                    switch (gsRequest.sSortDir[0].ToLower())
                    {
                        case "desc":
                            lsOrderDir = " desc";
                            lsOrderDirInv = " asc";
                            break;
                        default:
                            lsOrderDir = " asc";
                            lsOrderDirInv = " desc";
                            break;
                    }
                }
                else
                {
                    lsOrderCol = "dtFecUltAct";
                    lsOrderDir = " asc";
                    lsOrderDirInv = " desc";
                }


                string lsCampoGpo = DSODataAccess.Execute("select Columna from [" + DSODataContext.Schema + "].GetCamposMaestro(" + iCodMaestro + ") where ConfigName = 'GEtiqueta'").Rows[0]["Columna"].ToString();

                lsbSelectTop.AppendLine("select * from (");
                lsbSelectTop.AppendLine("   select top " + gsRequest.iDisplayLength + " * from (");
                lsbSelectTop.AppendLine("       select top " + (gsRequest.iDisplayStart + gsRequest.iDisplayLength));

                ////////////////////////////////////////////////////////////////////////////

                lsbColumnas.AppendLine("iCodRegistro");
                lsbColumnas.AppendLine(",Consultar = null");
                lsbColumnas.AppendLine(",vchCodigo");
                lsbColumnas.AppendLine(",vchDescripcion");

                foreach (KeytiaBaseField lField in lFields)
                {
                    lsbColumnas.AppendLine("," + lField.Column);
                }
                lsbColumnas.AppendLine(", Gpo =" + "[" + DSODataContext.Schema + "].GetCatDesc(GE.iCatGrupo," + liCodIdioma + ",'" + ldtVigencia.ToString("yyyy-MM-dd") + "')");
                lsbColumnas.AppendLine(",dtIniVigencia");
                lsbColumnas.AppendLine(",dtFinVigencia = DateAdd(day,-1,dtFinVigencia)");
                lsbColumnas.AppendLine(",dtFecUltAct");

                //////////////////////////////////////////////////////////////////////////7

                lsbFrom.AppendLine("      from [" + DSODataContext.Schema + "].Historicos a");
                lsbFrom.AppendLine("      where iCodMaestro = " + iCodMaestro);
                lsbFrom.AppendLine("      and iCodCatalogo in(select iCodRegistro from [" + DSODataContext.Schema + "].Catalogos where iCodCatalogo = " + iCodEntidad + ")");
                lsbFrom.AppendLine("      and '" + ldtVigencia.ToString("yyyy-MM-dd") + "' >= dtIniVigencia");
                lsbFrom.AppendLine("      and '" + ldtVigencia.ToString("yyyy-MM-dd") + "' < dtFinVigencia");
                lsbFrom.AppendLine("      and " + lsCampoGpo + " > 0");


                lsbOrderBy.AppendLine("       order by " + lsOrderCol + lsOrderDir + ", iCodRegistro" + lsOrderDir);
                lsbOrderBy.AppendLine("   ) as a order by " + lsOrderCol + lsOrderDirInv + ", iCodRegistro" + lsOrderDirInv);
                lsbOrderBy.AppendLine(") as a order by " + lsOrderCol + lsOrderDir + ", iCodRegistro" + lsOrderDir);

                bool lbPrimero = true;

                //formatos de conversion de fechas de sql
                string lsSqlDateFormat = Globals.GetLangItem("MsgWeb", "Mensajes Web", "SqlDateFormat");
                string lsSqlTimeFormat = Globals.GetLangItem("MsgWeb", "Mensajes Web", "SqlTimeFormat");

                if (gsRequest.sSearchGlobal.Trim() != "")
                {
                    string lsTerm = gsRequest.sSearchGlobal.Replace("'", "''").Trim();
                    StringBuilder lsbColTodas = new StringBuilder();
                    lsbColTodas.AppendLine("isnull(a.vchCodigo,'')");
                    lsbColTodas.AppendLine("+ ' ' + isnull(a.vchDescripcion,'')");

                    lsbColTodas.AppendLine("+ ' ' + isnull(convert(varchar,a.dtIniVigencia," + lsSqlDateFormat + "),'')+ ' ' + isnull(convert(varchar,a.dtIniVigencia," + lsSqlTimeFormat + "),'')");
                    lsbColTodas.AppendLine("+ ' ' + isnull(convert(varchar,a.dtFinVigencia," + lsSqlDateFormat + "),'')+ ' ' + isnull(convert(varchar,a.dtFinVigencia," + lsSqlTimeFormat + "),'')");

                    foreach (KeytiaBaseField lField in lFields)
                    {
                        if (lField.Column.StartsWith("Date"))
                        {
                            lsbColTodas.AppendLine("+ ' ' + isnull(convert(varchar,a." + lField.Column + "," + lsSqlDateFormat + "),'') + ' ' + isnull(convert(varchar,a." + lField.Column + "," + lsSqlTimeFormat + "),'')");
                        }
                        else if (!(lField.Column.StartsWith("VarChar")
                            || lField.Column.StartsWith("iCodCatalogo")))
                        {
                            lsbColTodas.AppendLine("+ ' ' + isnull(convert(varchar," + lField.Column + "),'')");
                        }
                        else
                        {
                            lsbColTodas.AppendLine("+ ' ' + isnull(" + lField.Column + ",'')");
                        }
                    }
                    lsbWhere.AppendLine("where " + lsbColTodas.ToString() + " like '%" + lsTerm + "%'");
                    lbPrimero = false;
                }

                string[] lsColumns = gsRequest.sColumns.Split(',');
                int lidx;
                for (lidx = 0; lidx < lsColumns.Length; lidx++)
                {
                    if (gsRequest.bSearchable[lidx])
                    {
                        string lsColumn = lsColumns[lidx];
                        string lsFiltro = gsRequest.sSearch[lidx].Replace("'", "''").Trim();

                        if (!String.IsNullOrEmpty(lsFiltro)
                            && (lFields.Contains(lsColumn)
                            || lsColumn == "vchCodigo"
                            || lsColumn == "vchDescripcion"
                            || lsColumn == "dtIniVigencia"
                            || lsColumn == "dtFinVigencia"))
                        {
                            if (lbPrimero)
                            {
                                lsbWhere.Append("where ");
                                lbPrimero = false;
                            }
                            else
                            {
                                lsbWhere.Append("and ");
                            }
                            if (lsColumn.StartsWith("Date")
                                || lsColumn == "dtIniVigencia"
                                || lsColumn == "dtFinVigencia")
                            {

                                lsbWhere.Append("(");
                                lsbWhere.AppendLine("convert(varchar, a." + lsColumn + ", " + lsSqlDateFormat + ") like '%" + lsFiltro + "%'");
                                lsbWhere.AppendLine("or convert(varchar, a." + lsColumn + ", " + lsSqlTimeFormat + ") like '%" + lsFiltro + "%'");
                                lsbWhere.AppendLine(")");
                            }
                            else
                            {
                                lsbWhere.AppendLine("a." + lsColumn + " like '%" + lsFiltro + "%'");
                            }
                        }
                    }
                }

                if (lbPrimero)
                {
                    lsbWhere.Append("where ");
                    lbPrimero = false;
                }
                else
                {
                    lsbWhere.Append("and ");
                }

                string lsCampoEmpl = lFields.GetByConfigName("Emple").Column;
                if (piCodEmpleado == null)
                {
                    //lsbWhere.Append(lsCampoEmpl + " is null and GE.iValorGpo > 0");
                    lsbWhere.Append("vchDescripcion like '%Corp%' and GE.iValorGpo > 0");                    
                }
                else
                {
                    lsbWhere.Append(lsCampoEmpl + " = [" + DSODataContext.Schema + "].GetCatDesc(" + liCodEmpleado.ToString() + "," + liCodIdioma + ",'" + ldtVigencia.ToString("yyyy-MM-dd") + "') and GE.iValorGpo > 0");
                }
                string lsSelectCount = "select count(iCodRegistro) ";
                string lsSelectTop = lsbSelectTop.ToString();
                string lsColumnas = lsbColumnas.ToString();
                string lsFrom = lsbFrom.ToString();
                string lsFromGetHisData = "";
                if (lbMaestroDir)
                {
                    lsFromGetHisData = "from (Select iCatGrupo = iCodCatalogo, iValorGpo = Integer01 from [" + DSODataContext.Schema + "].Historicos where iCodCatalogo01 in (select iCodRegistro from " + DSODataContext.Schema + ".Catalogos where vchcodigo = 'GEtiqueta')) GE Right join \r\n";
                    lsFromGetHisData = lsFromGetHisData + "[" + DSODataContext.Schema + "].GetHisData(" + iCodEntidad + "," + iCodMaestro + "," + liCodIdioma + ",'" + ldtVigencia.ToString("yyyy-MM-dd") + "') a \r\n";
                    lsFromGetHisData = lsFromGetHisData + "On a." + lsCampoGpo + " = GE.iValorGpo \r\n";
                    lsFromGetHisData = lsFromGetHisData + "\r\n";
                }
                else
                {
                    lsFromGetHisData = "from " + DSODataContext.Schema + ".GetHisData(" + iCodEntidad + "," + iCodMaestro + "," + liCodIdioma + ",'" + ldtVigencia.ToString("yyyy-MM-dd") + "') a \r\n";

                }
                string lsWhere = lsbWhere.ToString();
                string lsOrderBy = lsbOrderBy.ToString();

                lgsrRet.sEcho = gsRequest.sEcho;
                if (piCodEmpleado == null)
                {
                    lgsrRet.iTotalRecords = (int)DSODataAccess.ExecuteScalar(lsSelectCount + lsFrom + " and vchDescripcion like '%Corp%'");
                }
                else
                {
                    lgsrRet.iTotalRecords = (int)DSODataAccess.ExecuteScalar(lsSelectCount + lsFrom + " and " + lsCampoEmpl + " = " + liCodEmpleado.ToString());
                }
                if (!String.IsNullOrEmpty(lsWhere))
                {
                    lgsrRet.iTotalDisplayRecords = (int)DSODataAccess.ExecuteScalar(lsSelectCount + lsFromGetHisData + lsWhere);
                }
                else
                {
                    lgsrRet.iTotalDisplayRecords = lgsrRet.iTotalRecords;
                }

                ldt = DSODataAccess.Execute(lsSelectTop + lsColumnas + lsFromGetHisData + lsWhere + lsOrderBy);

                lgsrRet.ProcesarDatos(gsRequest, ldt);

                if (lgsrRet.iTotalDisplayRecords > 0 && ldt.Rows.Count == 0)
                {
                    string lsTitulo = Globals.GetMsgWeb(false, "TituloHistoricos");
                    
                    throw new KeytiaWebException(true, "ErrGridData", null, lsTitulo);
                }

                lFields.FormatGridData(lgsrRet, ldt);

                return lgsrRet;
            }
            catch (Exception e)
            {
                string lsTitulo = Globals.GetMsgWeb(false, "TituloHistoricos");
                throw new KeytiaWebException(true, "ErrGridData", e, lsTitulo);
            }
        }


        protected override bool ValidarClaves()
        {
            bool lbret = true;
            StringBuilder lsbErrores = new StringBuilder();
            StringBuilder lsbErroresCodigos = new StringBuilder();
            string lsError = "";
            string lsTitulo = DSOControl.JScriptEncode(AlertTitle);
            DataTable ldt;

            try
            {
                if (pvchCodigo.HasValue)
                {
                    psbQuery.Length = 0;
                    psbQuery.AppendLine("select H.iCodRegistro, H.iCodCatalogo, C.vchDescripcion, BanderasEtiqueta = H.{BanderasEtiqueta}, H.dtIniVigencia, dtFinVigencia = DateAdd(day,-1,H.dtFinVigencia)");
                    psbQuery.AppendLine("from Historicos H, Catalogos C");
                    psbQuery.AppendLine("where H.iCodCatalogo = C.iCodRegistro");
                    psbQuery.AppendLine("and H.iCodRegistro <> isnull(" + iCodRegistro + ",-1)");
                    psbQuery.AppendLine("and H.iCodCatalogo <> isnull(" + iCodCatalogo + ",-1)");
                    psbQuery.AppendLine("and C.iCodCatalogo = " + iCodEntidad);
                    psbQuery.AppendLine("and C.vchCodigo = " + pvchCodigo.DataValue);
                    psbQuery.AppendLine("and H.iCodMaestro = " + iCodMaestro);
                    psbQuery.AppendLine("and H.dtIniVigencia <> H.dtFinVigencia");
                    psbQuery.AppendLine("and ((H.dtIniVigencia <= " + pdtIniVigencia.DataValue);
                    psbQuery.AppendLine("       and H.dtFinVigencia > " + pdtIniVigencia.DataValue + ")");
                    psbQuery.AppendLine("   or (H.dtIniVigencia <= " + pdtFinVigencia.DataValue);
                    psbQuery.AppendLine("       and H.dtFinVigencia > " + pdtFinVigencia.DataValue + ")");
                    psbQuery.AppendLine("   or (H.dtIniVigencia >= " + pdtIniVigencia.DataValue);
                    psbQuery.AppendLine("       and H.dtFinVigencia <= " + pdtFinVigencia.DataValue + "))");
                    psbQuery.AppendLine("order by H.dtIniVigencia, H.dtFinVigencia, H.iCodRegistro");

                    ldt = pKDB.ExecuteQuery("Directorio", "Directorio Telefonico", psbQuery.ToString());

                    string lsNetDateFormat = Globals.GetLangItem("MsgWeb", "Mensajes Web", "NetDateFormat");
                    foreach (DataRow ldataRow in ldt.Rows)
                    {
                        if ((((int)Util.IsDBNull(ldataRow["BanderasEtiqueta"], 0) & 0x01) / 0x01) != 1 && pFields.GetByConfigName("Emple").DSOControlDB.HasValue)
                        {
                            lsError = DSOControl.JScriptEncode(Globals.GetMsgWeb(false, "MsjNumeroCorporativo",pvchCodigo.ToString()));
                            lsError = "<span>" + lsError + "</span>";
                            lsbErrores.Append("<li>" + lsError + "</li>");
                            lsError = "";
                            break;
                        }
                        else if (Util.IsDBNull(ldataRow["vchDescripcion"], "").ToString() == pvchDescripcion.DataValue.ToString().Replace("'",""))
                        {
                            if (lsbErroresCodigos.Length == 0)
                            {
                                lsError = DSOControl.JScriptEncode(Globals.GetMsgWeb(false, "CampoRepetido", pvchCodigo.Descripcion));
                                lsError = "<span>" + lsError + "</span>";
                                lsbErrores.Append("<li>" + lsError);
                            }
                            lsError = DSOControl.JScriptEncode(Globals.GetMsgWeb(false, "RangoVigencias", ((DateTime)ldataRow["dtIniVigencia"]).ToString(lsNetDateFormat), ((DateTime)ldataRow["dtFinVigencia"]).ToString(lsNetDateFormat)));
                            lsbErroresCodigos.Append("<li>" + lsError + "</li>");
                        }
                    }
                    if (lsError.Length > 0)
                    {
                        lsbErrores.Append("<ul>" + lsbErroresCodigos.ToString() + "</ul>");
                        lsbErrores.Append("</li>");
                    }
                }
                else
                {
                    lsbErrores.Append("<li>" + DSOControl.JScriptEncode(Globals.GetMsgWeb(false, "CampoRequerido", pvchCodigo.Descripcion)) + "</li>");                    
                }

                if (!pvchDescripcion.HasValue)
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
            catch (Exception ex)
            {
                throw new KeytiaWebException("ErrValidateRecord", ex);
            }

            return lbret;
        }

        protected override bool ValidarDatos()
        {
            string lsError;
            string lsTitulo = DSOControl.JScriptEncode(AlertTitle);
            StringBuilder lsbErrores = new StringBuilder();
            string lsNumero;
            bool lbret = true;

            try
            {
                if (piCodEmpleado == "")
                {
                    lsError = DSOControl.JScriptEncode(Globals.GetMsgWeb(false, "MsjUsuarioSinEmple"));
                    lsbErrores.Append("<li>" + lsError + "</li>");
                }

                lsNumero = (string)pvchCodigo.DataValue;
                lsNumero = lsNumero.Replace("'", "");
                if (!System.Text.RegularExpressions.Regex.IsMatch(lsNumero, "^([0-9])*$"))
                {
                    lsError = DSOControl.JScriptEncode(Globals.GetMsgWeb(false, "ValEmplFormato", new string[] { pvchCodigo.Descripcion }));
                    lsbErrores.Append("<li>" + lsError + "</li>");
                }
                if (lsbErrores.Length > 0)
                {
                    lbret = false;
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

        public override void SetHistoricState(HistoricState s)
        {
            base.SetHistoricState(s);            
            pTablaAtributos.Enabled = true;

            if (State == HistoricState.Edicion)
            {                
                if (pFields != null && iCodRegistro != "null" && pdtIniVigencia.HasValue)
                {                    
                    int liDiaIniVig = EtiquetaEdit.ValidarDiasMes(pdtIniVigencia.Date.Year, pdtIniVigencia.Date.Month, piDiaCorte);
                    DateTime ldtIniVigencia = new DateTime(pdtIniVigencia.Date.Year, pdtIniVigencia.Date.Month, liDiaIniVig);
                    int liDiaCorteRegistro = EtiquetaEdit.ValidarDiasMes(ldtIniVigencia.Date.AddMonths(1).Year,ldtIniVigencia.Date.AddMonths(1).Month,liDiaIniVig);
                    DateTime ldCorteRegistro = new DateTime(ldtIniVigencia.Date.AddMonths(1).Year, ldtIniVigencia.Date.AddMonths(1).Month, liDiaCorteRegistro);
                    if (pdtCorte > ldCorteRegistro)
                    {                                                
                        SetHistoricState(HistoricState.Baja);
                    }
                    else if (pdtCorte == ldCorteRegistro && (DateTime.Today > pdtLimite || pbEtiquetarUnaVez))
                    {
                        SetHistoricState(HistoricState.Baja);
                    }
                    else
                    {
                        pdtFinVigencia.MinDateTime = ldtIniVigencia;
                    }
                }
                if (s == HistoricState.Consulta)
                {
                    pdtIniVigencia.MinDateTime = DateTime.MinValue;
                }
            }
            else if (State == HistoricState.MaestroSeleccionado)
            {
                pExpRegistro.Visible = false;
                pdtIniVigencia.DataValue = DBNull.Value;
                pdtFinVigencia.DataValue = DBNull.Value;
                if (!pbEnableEmpleado)
                {
                    pbtnAgregar.Visible = false;
                }
            }
            else if (State == HistoricState.Baja)
            {
                pTablaAtributos.Enabled = false;
                pFields.DisableFields();

                int liDiaCorte;
                DateTime ldtFinMinValue;
                DateTime ldtIniVigencia;
                if (piDiaCorte >= DateTime.Today.Day)
                {
                    liDiaCorte = EtiquetaEdit.ValidarDiasMes(DateTime.Today.AddMonths(-1).Year, DateTime.Today.AddMonths(-1).Month, piDiaCorte);
                    ldtFinMinValue = new DateTime(DateTime.Today.AddMonths(-1).Year, DateTime.Today.AddMonths(-1).Month, liDiaCorte);
                }
                else
                {
                    liDiaCorte = EtiquetaEdit.ValidarDiasMes(DateTime.Today.Year, DateTime.Today.Month, piDiaCorte);
                    ldtFinMinValue = new DateTime(DateTime.Today.Year, DateTime.Today.Month, liDiaCorte);
                }
                if (DateTime.Today <= pdtLimite)
                {
                    int liDiaIniPeriodo = EtiquetaEdit.ValidarDiasMes(pdtCorte.AddMonths(-1).Year, pdtCorte.AddMonths(-1).Month, piDiaCorte);
                    ldtIniVigencia = new DateTime(pdtCorte.AddMonths(-1).Year, pdtCorte.AddMonths(-1).Month, liDiaIniPeriodo);
                }
                else
                {
                    ldtIniVigencia = pdtCorte;
                }

                if (liDiaCorte == 1)
                {
                    pdtFinVigencia.MinDateTime = new DateTime(ldtIniVigencia.AddMonths(-1).Year, ldtIniVigencia.AddMonths(-1).Month, DateTime.DaysInMonth(ldtIniVigencia.AddMonths(-1).Year, ldtIniVigencia.AddMonths(-1).Month));
                }
                else
                {
                    pdtFinVigencia.MinDateTime = ldtIniVigencia.AddDays(-1);
                }
            }
        }

        public override void ConsultarRegistro()
        {
            pdtIniVigencia.MinDateTime = DateTime.MinValue;
            pdtFinVigencia.MinDateTime = DateTime.MinValue;
            base.ConsultarRegistro();
            if (pdtFinVigencia.Date != DateTime.MinValue)
            {
                pdtFinVigencia.DataValue = pdtFinVigencia.Date.AddDays(-1);
            }
        }

        protected virtual void DisableFields()
        {
            if (pFields == null)
            {
                return;
            }

            pvchDescripcion.Visible = false;
            
            //Ocultar Fields No necesarios en Dir. Personales.
            if (piCodEntidad != null && piCodMaestro != null)
            {
                pTablaRegistro.Rows[0].Visible = false;
                pTablaRegistro.Rows[1].Visible = false; 
            }

            foreach (string lField in pFieldsNoVisibles)
            {
                if (pFields.GetByConfigName(lField.ToString()) != null)
                {
                    //pTablaAtributos.Rows[pFields.GetByConfigName(lField.ToString()).DSOControlDB.Row - 1].Cells[0].Text = "";
                    pTablaAtributos.Rows[pFields.GetByConfigName(lField.ToString()).DSOControlDB.Row - 1].Visible = false;
                    pFields.GetByConfigName(lField.ToString()).DSOControlDB.Visible = false;

                }
            }
        }

        protected override void InitGrid()
        {
            DSOGridClientColumn lCol;
            int lTarget = 0;

            pHisGrid.ClearConfig();
            pHisGrid.Config.sDom = "<\"H\"Rlf>tr<\"F\"pi>"; //con filtro global
            pHisGrid.Config.bAutoWidth = true;
            pHisGrid.Config.sScrollX = "100%";
            pHisGrid.Config.sScrollY = "400px";
            pHisGrid.Config.sPaginationType = "full_numbers";
            pHisGrid.Config.bJQueryUI = true;
            pHisGrid.Config.bProcessing = true;
            pHisGrid.Config.bServerSide = true;
            pHisGrid.Config.fnServerData = "function(sSource, aoData, fnCallback){" + pjsObj + ".fnServerData(this, sSource, aoData, fnCallback);}";
            pHisGrid.Config.sAjaxSource = ResolveUrl("~/WebMethods.aspx/GetHisDataDirEmple");

            lCol = new DSOGridClientColumn();
            lCol.sName = "iCodRegistro";
            lCol.aTargets.Add(lTarget++);
            lCol.bVisible = false;
            pHisGrid.Config.aoColumnDefs.Add(lCol);

            lCol = new DSOGridClientColumn();
            lCol.sName = "Consultar";
            lCol.aTargets.Add(lTarget++);
            lCol.sWidth = "50px";
            lCol.sClass = "TdConsult";
            if (!pbEnableEmpleado)
            {
                lCol.bVisible = false;
            }
            pHisGrid.Config.aoColumnDefs.Add(lCol);

            lCol = new DSOGridClientColumn();
            lCol.sName = "vchCodigo";
            lCol.aTargets.Add(lTarget++);
            pHisGrid.Config.aoColumnDefs.Add(lCol);

            lCol = new DSOGridClientColumn();
            lCol.sName = "vchDescripcion";
            lCol.aTargets.Add(lTarget++);
            pHisGrid.Config.aoColumnDefs.Add(lCol);

            InitGridFields();

            lTarget = pHisGrid.Config.aoColumnDefs.Count;     

            lCol = new DSOGridClientColumn();
            lCol.sName = "Gpo";
            lCol.aTargets.Add(lTarget++);
            lCol.sWidth = "120px";
            pHisGrid.Config.aoColumnDefs.Add(lCol);

            lCol = new DSOGridClientColumn();
            lCol.sName = "dtIniVigencia";
            lCol.aTargets.Add(lTarget++);
            lCol.sWidth = "120px";
            pHisGrid.Config.aoColumnDefs.Add(lCol);

            lCol = new DSOGridClientColumn();
            lCol.sName = "dtFinVigencia";
            lCol.aTargets.Add(lTarget++);
            lCol.sWidth = "120px";
            pHisGrid.Config.aoColumnDefs.Add(lCol);

            lCol = new DSOGridClientColumn();
            lCol.sName = "dtFecUltAct";
            lCol.aTargets.Add(lTarget++);
            pHisGrid.Config.aoColumnDefs.Add(lCol);

            //if (pHisGrid.Config.aoColumnDefs.Count > 10)
            //{
            //    pHisGrid.Config.sScrollXInner = (pHisGrid.Config.aoColumnDefs.Count * 20).ToString() + "%";
            //}

            pHisGrid.Fill();
        }

        protected override void GrabarRegistro()
        {
            //Las vigencias de los numeros deben de ser de 0 días (baja) o de al menos un periodo.
            if (pdtFinVigencia.Date.Year == 2079)
            {
                //Fecha Default 2079
                pdtFinVigencia.DataValue = new DateTime(2079, 1, EtiquetaEdit.ValidarDiasMes(2079, 1, piDiaCorte));
            }
            else if (pdtFinVigencia.Date != pdtIniVigencia.Date)
            {
                //Calcula la próxima fecha corte del día elegido como fin de vigencia.
                int liDiaCorte;
                DateTime ldtProxCorte = pdtFinVigencia.Date.AddMonths(1);
                if (piDiaCorte > pdtFinVigencia.Date.Day)
                {
                    liDiaCorte = EtiquetaEdit.ValidarDiasMes(ldtProxCorte.AddMonths(-1).Year, ldtProxCorte.AddMonths(-1).Month, piDiaCorte);
                    ldtProxCorte = new DateTime(ldtProxCorte.AddMonths(-1).Year, ldtProxCorte.AddMonths(-1).Month, liDiaCorte);
                }
                else
                {
                    liDiaCorte = EtiquetaEdit.ValidarDiasMes(ldtProxCorte.Year, ldtProxCorte.Month, piDiaCorte);
                    ldtProxCorte = new DateTime(ldtProxCorte.Year, ldtProxCorte.Month, liDiaCorte);
                }
                pdtFinVigencia.DataValue = ldtProxCorte;
            }           
        
            base.GrabarRegistro();

            if (iCodRegistro == "null")
            {
                return;
            }

            // Actualizar el Directorio
            ActualizarDirectorio();

            //Actualiza llamadas Tasadas 
            try
            {
                psbQuery.Length = 0;
                psbQuery.AppendLine("Update CDR Set CDR.GEtiqueta = Dir.GEtiqueta, CDR.Etiqueta = Dir.Etiqueta");
                psbQuery.AppendLine("from [" + DSODataContext.Schema + "].[VisDetallados('Detall','DetalleCDR','" + Globals.GetCurrentLanguage() + "')] CDR,");
                psbQuery.AppendLine("[" + DSODataContext.Schema + "].[VisDirectorio('" + Globals.GetCurrentLanguage() + "')] Dir");
                psbQuery.AppendLine("where CDR.Emple = Dir.Emple");
                psbQuery.AppendLine("and Dir.iCodCatalogo =" + iCodCatalogo);
                psbQuery.AppendLine("and CDR.TelDest = Dir.vchCodigo");
                psbQuery.AppendLine("and Dir.dtIniVigencia <= CDR.FechaInicio");
                psbQuery.AppendLine("and Dir.dtFinVigencia > CDR.FechaInicio");
                psbQuery.AppendLine("and Dir.dtIniVigencia <= '" + dtIniVigencia.Date.ToString("yyyy-MM-dd") + "'");
                psbQuery.AppendLine("and Dir.dtFinVigencia > '" + dtIniVigencia.Date.ToString("yyyy-MM-dd") + "'");
                DSODataAccess.ExecuteNonQuery(psbQuery.ToString());
            }
            catch (Exception ex)
            {
                throw new KeytiaWebException("ErrSaveRecord", ex);
            }

        }

        protected override bool ValidarPermiso(Permiso p)
        {
            if (p != Permiso.Replicar)
            {
                return true;
            }
            return false;
        }

        protected virtual void ActualizarDirectorio()
        {
        }
    }   
}
