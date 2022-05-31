using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Web;
using System.Web.Caching;
using System.Web.UI.WebControls;

using KeytiaServiceBL;
using KeytiaWeb.UserInterface;
using DSOControls2008;

namespace KeytiaWeb
{
    public class Globals
    {
        public static void Init()
        {
            HttpContext.Current.Session["MasterPage"] = "";
            HttpContext.Current.Session["HomePage"] = "";
            HttpContext.Current.Session["Language"] = "";
            HttpContext.Current.Session["Currency"] = "";
            HttpContext.Current.Session["CustomerLogo"] = "";
            HttpContext.Current.Session["StyleSheet"] = "";
            HttpContext.Current.Session["iCodPerfil"] = "";
            HttpContext.Current.Session["DiaLimiteMesAnt"] = "";
            HttpContext.Current.Session["FechaInicio"] = "";
            HttpContext.Current.Session["FechaFin"] = "";
            HttpContext.Current.Session["OcultarColumnImporte"] = "";

            LoadUserInfo();
        }

        public static void LoadUserInfo()
        {
            KDBAccess kdb = new KDBAccess();
            DataTable ldt = null;
            DataTable ldtEmp = null;
            DataTable ldtCli = null;

            int liUsrCodEmpre;
            int liUsrCodCliente;


            try
            {
                //Inicializa el contexto de datos
                DSODataContext.SetContext((int)HttpContext.Current.Session["iCodUsuarioDB"]);


                //Inicializa los datos del usuario en su nueva conexión
                ldt = kdb.GetHisRegByEnt("Usuar", "Usuarios",
                    new string[] { "iCodCatalogo", "{Empre}", "{Idioma}", "{Moneda}", "{HomePage}", "{Perfil}" },
                    "vchCodigo = '" + HttpContext.Current.Session["vchCodUsuario"] + "'");

                if (ldt != null && ldt.Rows.Count > 0)
                {
                    liUsrCodEmpre = (int)Util.IsDBNull(ldt.Rows[0]["{Empre}"], -1);
                    HttpContext.Current.Session["iCodUsuario"] = ldt.Rows[0]["iCodCatalogo"];
                    HttpContext.Current.Session["HomePage"] = (string)Util.IsDBNull(ldt.Rows[0]["{HomePage}"], "");
                    HttpContext.Current.Session["Language"] = (string)Util.IsDBNull(kdb.ExecuteScalar("Idioma", "Idioma", "select vchCodigo from catalogos where iCodRegistro = " + ldt.Rows[0]["{Idioma}"]), "");
                    HttpContext.Current.Session["Currency"] = (string)Util.IsDBNull(kdb.ExecuteScalar("Moneda", "Monedas", "select vchCodigo from catalogos where iCodRegistro = " + ldt.Rows[0]["{Moneda}"]), "");
                    HttpContext.Current.Session["iCodPerfil"] = ldt.Rows[0]["{Perfil}"];
                    HttpContext.Current.Session["vchCodPerfil"] = (string)Util.IsDBNull(kdb.ExecuteScalar("Perfil", "Perfiles", "select vchCodigo from catalogos where iCodRegistro = " + ldt.Rows[0]["{Perfil}"]), "");

                    ldtEmp = kdb.GetHisRegByEnt("Empre", "Empresas",
                        new string[] { "{Client}", "{MasterPage}", "{Logo}", "{StyleSheet}", "{DiaLimiteDefault}" },
                        "iCodCatalogo = " + liUsrCodEmpre);

                    if (ldtEmp != null && ldtEmp.Rows.Count > 0)
                    {
                        liUsrCodCliente = (int)Util.IsDBNull(ldtEmp.Rows[0]["{Client}"], -1);
                        HttpContext.Current.Session["MasterPage"] = (string)Util.IsDBNull(ldtEmp.Rows[0]["{MasterPage}"], "");
                        HttpContext.Current.Session["CustomerLogo"] = (string)Util.IsDBNull(ldtEmp.Rows[0]["{Logo}"], "");
                        string esquema = DSODataContext.Schema;
                        HttpContext.Current.Session["StyleSheet"] = (string)Util.IsDBNull(ldtEmp.Rows[0]["{StyleSheet}"], "");
                        HttpContext.Current.Session["DiaLimiteMesAnt"] = (ldtEmp.Rows[0]["{DiaLimiteDefault}"] != System.DBNull.Value) ? ldtEmp.Rows[0]["{DiaLimiteDefault}"].ToString() : "1";

                        if ((string)HttpContext.Current.Session["MasterPage"] == "" ||
                            (string)HttpContext.Current.Session["CustomerLogo"] == "" ||
                            (string)HttpContext.Current.Session["StyleSheet"] == "")
                        {
                            ldtCli = kdb.GetHisRegByEnt("Client", "Clientes",
                                new string[] { "{MasterPage}", "{Logo}", "{StyleSheet}", "{Esquema}" },
                                "iCodCatalogo = " + liUsrCodCliente);

                            if (ldtCli != null && ldtCli.Rows.Count > 0)
                            {
                                if ((string)HttpContext.Current.Session["MasterPage"] == "")
                                    HttpContext.Current.Session["MasterPage"] = (string)Util.IsDBNull(ldtCli.Rows[0]["{MasterPage}"], "");

                                if ((string)HttpContext.Current.Session["CustomerLogo"] == "")
                                    HttpContext.Current.Session["CustomerLogo"] = (string)Util.IsDBNull(ldtCli.Rows[0]["{Logo}"], "");

                                if ((string)HttpContext.Current.Session["StyleSheet"] == "")
                                    HttpContext.Current.Session["StyleSheet"] = (string)Util.IsDBNull(ldtCli.Rows[0]["{StyleSheet}"], "");
                            }
                        }
                    }
                }
                else
                    HttpContext.Current.Response.Redirect("~/Login.aspx");
            }
            catch (Exception ex)
            {
                HttpContext.Current.Session.Clear();
                KeytiaServiceBL.Util.LogException(ex);
                HttpContext.Current.Response.Redirect("~/Login.aspx?err=UCInc");
            }

            //Si algún dato no pudo ser iniciado, lo inicializa con el default
            if ((string)HttpContext.Current.Session["HomePage"] == "")
                HttpContext.Current.Session["HomePage"] = GetDefaultPage();

            if ((string)HttpContext.Current.Session["Language"] == "")
                HttpContext.Current.Session["Language"] = GetLanguage();

            if ((string)HttpContext.Current.Session["Currency"] == "")
                HttpContext.Current.Session["Currency"] = GetCurrency();

            if ((string)HttpContext.Current.Session["MasterPage"] == "")
                HttpContext.Current.Session["MasterPage"] = GetMasterPage();

            if ((string)HttpContext.Current.Session["CustomerLogo"] == "")
                HttpContext.Current.Session["CustomerLogo"] = GetCustomerLogo();

            if ((string)HttpContext.Current.Session["StyleSheet"] == "")
                HttpContext.Current.Session["StyleSheet"] = GetStyleSheet();

            //NZ Para configurar si el menu aparecera colapsado.
            GetCollapseOption();
        }

        public static string[] GetUserMessages()
        {
            return GetUserMessages((int)HttpContext.Current.Session["iCodUsuario"]);
        }

        public static string[] GetUserMessages(int liCodUsuario)
        {
            UserMessages lum = new UserMessages(liCodUsuario, GetLanguage());
            return lum.GetMessages();
        }

        #region Idioma
        public static string GetMsgWeb(string lsCodigo)
        {
            return GetMsgWeb(true, lsCodigo, null);
        }

        public static string GetMsgWeb(string lsCodigo, params string[] lsParam)
        {
            return GetMsgWeb(true, lsCodigo, lsParam);
        }

        public static string GetMsgWeb(bool lbHtmlEncode, string lsCodigo)
        {
            return GetMsgWeb(lbHtmlEncode, lsCodigo, null);
        }

        public static string GetMsgWeb(bool lbHtmlEncode, string lsCodigo, params string[] lsParam)
        {
            string lsRet = "";

            if (lbHtmlEncode)
                lsRet = HttpUtility.HtmlEncode(GetLangItem("MsgWeb", "Mensajes Web", lsCodigo, lsParam));
            else
                lsRet = GetLangItem("MsgWeb", "Mensajes Web", lsCodigo, lsParam);

            return lsRet;
        }

        public static string GetLangItem(string lsCodigo)
        {
            return GetLangItem("MsgWeb", "Mensajes Web", lsCodigo, null);
        }

        public static string GetLangItem(string lsCodigo, params object[] lsParam)
        {
            return GetLangItem("MsgWeb", "Mensajes Web", lsCodigo, lsParam);
        }

        public static string GetLangItem(string lsEntidad, string lsMaestro, string lsCodigo)
        {
            return GetLangItem(lsEntidad, lsMaestro, lsCodigo, null);
        }

        public static string GetLangItem(string lsEntidad, string lsMaestro, string lsElemento, params object[] lsParam)
        {
            //object loItem = DSODataContext.GetObject("Lang-" + lsEntidad + "-" + lsMaestro + "-" + GetLanguage() + "-" + lsElemento);
            string lsRet = "#undefined-" + lsElemento + "#";

            //if (loItem != null)
            //    lsRet = (string)loItem;
            //else
            //{
            Hashtable lhtLang;

            lhtLang = GetLangHT(lsEntidad, lsMaestro);

            if (lhtLang != null && lhtLang.ContainsKey(lsElemento))
            {
                lsRet = (string)Util.IsDBNull(lhtLang[lsElemento], "");
                //DSODataContext.SetObject("Lang-" + lsEntidad + "-" + lsMaestro + "-" + GetLanguage() + "-" + lsElemento, lsRet);
            }
            //}

            return (lsParam == null ? lsRet : string.Format(lsRet, lsParam));
        }

        public static DSOGridLanguage GetGridLanguage()
        {
            return GetGridLanguage(GetLanguage());
        }

        public static DSOGridLanguage GetGridLanguage(string lsLang)
        {
            DSOGridLanguage loLang;

            //loLang = (DSOGridLanguage)HttpContext.Current.Cache["GridLang-" + lsLang];
            loLang = (DSOGridLanguage)DSODataContext.GetObject("GridLang-" + lsLang);

            if (loLang == null)
            {
                loLang = new DSOGridLanguage();
                loLang.oPginate = new DSOGridLanguagePaginate();

                loLang.oPginate.sFirst = GetMsgWeb("GridFirst");
                loLang.oPginate.sLast = GetMsgWeb("GridLast");
                loLang.oPginate.sNext = GetMsgWeb("GridNext");
                loLang.oPginate.sPrevious = GetMsgWeb("GridPrevious");
                loLang.sEmptyTable = GetMsgWeb("GridEmptyTable");
                loLang.sInfo = GetMsgWeb("GridInfo");
                loLang.sInfoEmpty = GetMsgWeb("GridInfoEmpty");
                loLang.sInfoFiltered = GetMsgWeb("GridInfoFiltered");
                loLang.sInfoPostFix = GetMsgWeb("GridInfoPostFix");
                loLang.sLengthMenu = GetMsgWeb("GridLengthMenu");
                loLang.sProcessing = GetMsgWeb("GridProcessing");
                loLang.sSearch = GetMsgWeb("GridSearch");
                loLang.sZeroRecords = GetMsgWeb("GridZeroRecords");

                //AddToCache("GridLang-" + lsLang, loLang);
                DSODataContext.SetObject("GridLang-" + lsLang, loLang);
            }

            return loLang;
        }

        public static DSODateTimeBox.Region GetDateTimeLanguage()
        {
            return GetDateTimeLanguage(GetLanguage());
        }

        public static DSODateTimeBox.Region GetDateTimeLanguage(string lsLang)
        {
            DSODateTimeBox.Region loLang;

            //loLang = (DSODateTimeBox.Region)HttpContext.Current.Cache["DateTimeLang-" + lsLang];
            loLang = (DSODateTimeBox.Region)DSODataContext.GetObject("DateTimeLang-" + lsLang);

            if (loLang == null)
            {
                loLang = new DSODateTimeBox.Region();

                loLang.prevText = GetLangItem("DTBPrev");
                loLang.nextText = GetLangItem("DTBNext");
                loLang.weekHeader = GetLangItem("DTBWkH");
                loLang.dateFormat = GetLangItem("DTBDateFmt");
                loLang.timeOnlyTitle = GetLangItem("DTBTimeOnlyTit");
                loLang.timeText = GetLangItem("DTBTime");
                loLang.hourText = GetLangItem("DTBHour");
                loLang.minuteText = GetLangItem("DTBMinute");
                loLang.secondText = GetLangItem("DTBSecond");
                loLang.currentText = GetLangItem("DTBNow");
                loLang.closeText = GetLangItem("DTBClose");

                Hashtable lhtLang = GetLangHT("Mes", "Meses", false);
                if (lhtLang != null)
                {
                    loLang.monthNames.January = (string)lhtLang["1"];
                    loLang.monthNames.February = (string)lhtLang["2"];
                    loLang.monthNames.March = (string)lhtLang["3"];
                    loLang.monthNames.April = (string)lhtLang["4"];
                    loLang.monthNames.May = (string)lhtLang["5"];
                    loLang.monthNames.June = (string)lhtLang["6"];
                    loLang.monthNames.July = (string)lhtLang["7"];
                    loLang.monthNames.August = (string)lhtLang["8"];
                    loLang.monthNames.September = (string)lhtLang["9"];
                    loLang.monthNames.October = (string)lhtLang["10"];
                    loLang.monthNames.November = (string)lhtLang["11"];
                    loLang.monthNames.December = (string)lhtLang["12"];
                }

                lhtLang = GetLangHT("MsgWeb", "Mes Corto", false);
                if (lhtLang != null)
                {
                    loLang.monthNamesShort.January = (string)lhtLang["MC1"];
                    loLang.monthNamesShort.February = (string)lhtLang["MC2"];
                    loLang.monthNamesShort.March = (string)lhtLang["MC3"];
                    loLang.monthNamesShort.April = (string)lhtLang["MC4"];
                    loLang.monthNamesShort.May = (string)lhtLang["MC5"];
                    loLang.monthNamesShort.June = (string)lhtLang["MC6"];
                    loLang.monthNamesShort.July = (string)lhtLang["MC7"];
                    loLang.monthNamesShort.August = (string)lhtLang["MC8"];
                    loLang.monthNamesShort.September = (string)lhtLang["MC9"];
                    loLang.monthNamesShort.October = (string)lhtLang["MC10"];
                    loLang.monthNamesShort.November = (string)lhtLang["MC11"];
                    loLang.monthNamesShort.December = (string)lhtLang["MC12"];
                }

                lhtLang = GetLangHT("DiasSem", "Dias Semana", false);
                if (lhtLang != null)
                {
                    loLang.dayNames.Sunday = (string)lhtLang["0"];
                    loLang.dayNames.Monday = (string)lhtLang["1"];
                    loLang.dayNames.Tuesday = (string)lhtLang["2"];
                    loLang.dayNames.Wednesday = (string)lhtLang["3"];
                    loLang.dayNames.Thursday = (string)lhtLang["4"];
                    loLang.dayNames.Friday = (string)lhtLang["5"];
                    loLang.dayNames.Saturday = (string)lhtLang["6"];
                }

                lhtLang = GetLangHT("MsgWeb", "Dia Corto", false);
                if (lhtLang != null)
                {
                    loLang.dayNamesShort.Sunday = (string)lhtLang["DC0"];
                    loLang.dayNamesShort.Monday = (string)lhtLang["DC1"];
                    loLang.dayNamesShort.Tuesday = (string)lhtLang["DC2"];
                    loLang.dayNamesShort.Wednesday = (string)lhtLang["DC3"];
                    loLang.dayNamesShort.Thursday = (string)lhtLang["DC4"];
                    loLang.dayNamesShort.Friday = (string)lhtLang["DC5"];
                    loLang.dayNamesShort.Saturday = (string)lhtLang["DC6"];
                }

                lhtLang = GetLangHT("MsgWeb", "Dia Minimo", false);
                if (lhtLang != null)
                {
                    loLang.dayNamesMin.Sunday = (string)lhtLang["DM0"];
                    loLang.dayNamesMin.Monday = (string)lhtLang["DM1"];
                    loLang.dayNamesMin.Tuesday = (string)lhtLang["DM2"];
                    loLang.dayNamesMin.Wednesday = (string)lhtLang["DM3"];
                    loLang.dayNamesMin.Thursday = (string)lhtLang["DM4"];
                    loLang.dayNamesMin.Friday = (string)lhtLang["DM5"];
                    loLang.dayNamesMin.Saturday = (string)lhtLang["DM6"];
                }

                //AddToCache("DateTimeLang-" + lsLang, loLang);
                DSODataContext.SetObject("DateTimeLang-" + lsLang, loLang);
            }

            return loLang;
        }

        public static Hashtable GetLangHT(string lsEntidad, string lsMaestro)
        {
            return GetLangHT(lsEntidad, lsMaestro, GetLanguage(), true);
        }

        public static Hashtable GetLangHT(string lsEntidad, string lsMaestro, bool lbAddToCache)
        {
            return GetLangHT(lsEntidad, lsMaestro, GetLanguage(), lbAddToCache);
        }

        public static Hashtable GetLangHT(string lsEntidad, string lsMaestro, string lsLang)
        {
            return GetLangHT(lsEntidad, lsMaestro, lsLang, true);
        }

        public static Hashtable GetLangHT(string lsEntidad, string lsMaestro, string lsLang, bool lbAddToCache)
        {
            Hashtable lht = null;

            lht = (Hashtable)DSODataContext.GetObject("Lang-" + lsEntidad + "-" + lsMaestro + "-" + lsLang);

            if (lht == null)
            {
                KDBAccess kdb = new KDBAccess();
                DataTable ldt = kdb.GetHisRegByEnt(lsEntidad, lsMaestro);

                if (ldt != null && ldt.Rows.Count > 0)
                {
                    lht = new Hashtable();

                    foreach (DataRow dr in ldt.Rows)
                    {
                        if (dr.Table.Columns.Contains("{" + lsLang + "}"))
                        {
                            if (lht.Contains(dr["vchCodigo"]))
                                lht[dr["vchCodigo"]] = dr["{" + lsLang + "}"];
                            else
                                lht.Add(dr["vchCodigo"], dr["{" + lsLang + "}"]);
                        }
                        else
                        {
                            if (lht.Contains(dr["vchCodigo"]))
                                lht[dr["vchCodigo"]] = dr["vchDescripcion"];
                            else
                                lht.Add(dr["vchCodigo"], dr["vchDescripcion"]);
                        }
                    }

                    DSODataContext.SetObject("Lang-" + lsEntidad + "-" + lsMaestro + "-" + lsLang, lht);
                }
            }

            return lht;
        }

        public static void ChangeComboLanguage(ListControl lcbo, string lsEntidad, string lsMaestro)
        {
            Hashtable lht = Globals.GetLangHT(lsEntidad, lsMaestro);

            foreach (ListItem li in lcbo.Items)
            {
                if (lht.ContainsKey(li.Value))
                    li.Text = (string)lht[li.Value];
            }
        }
        #endregion


        public static double GetCurrConv(string lsMonedaOriginal, double ldValor)
        {
            return 0d;
        }



        public static string GetCustomerLogo()
        {
            string lsImg = "";

            if (HttpContext.Current.Session["CustomerLogo"] != null && (string)HttpContext.Current.Session["CustomerLogo"] != "")
                lsImg = (string)HttpContext.Current.Session["CustomerLogo"];
            else if (Util.AppSettings("DefaultLogo") != "")
                lsImg = Util.AppSettings("DefaultLogo");
            else
                lsImg = "~/images/evox2.jpg";

            return lsImg;
        }

        public static string GetStyleSheet()
        {
            string lsCSS = "";

            if (HttpContext.Current.Session["StyleSheet"] != null && (string)HttpContext.Current.Session["StyleSheet"] != "")
                lsCSS = (string)HttpContext.Current.Session["StyleSheet"];
            else if (Util.AppSettings("DefaultStyleSheet") != "")
                lsCSS = Util.AppSettings("DefaultStyleSheet");
            else
                lsCSS = "~/styles";

            return lsCSS;
        }

        public static string GetMasterPage()
        {
            string lsMP = "";

            if (HttpContext.Current.Session["MasterPage"] != null && (string)HttpContext.Current.Session["MasterPage"] != "")
                lsMP = (string)HttpContext.Current.Session["MasterPage"];
            else if (Util.AppSettings("DefaultMasterPage") != "")
                lsMP = Util.AppSettings("DefaultMasterPage");
            else
                lsMP = "~/KeytiaOV.Master";

            return lsMP;
        }

        public static string GetDefaultPage()
        {
            string lsDP = "";

            if (HttpContext.Current.Session["HomePage"] != null && (string)HttpContext.Current.Session["HomePage"] != "")
                lsDP = (string)HttpContext.Current.Session["HomePage"];
            else if (Util.AppSettings("DefaultHomePage") != "")
                lsDP = Util.AppSettings("DefaultHomePage");
            else
                lsDP = "~/WebForm1.aspx";

            return lsDP;
        }



        public static void SetLanguage(string lsLanguage)
        {
            HttpContext.Current.Session["Language"] = lsLanguage;
        }

        public static string GetCurrentLanguage()
        {
            return GetLanguage();
        }

        public static string GetLanguage()
        {
            string lsRet;

            if (HttpContext.Current.Session["Language"] != null && (string)HttpContext.Current.Session["Language"] != "")
                lsRet = (string)HttpContext.Current.Session["Language"];
            else if (Util.AppSettings("DefaultLanguage") != "")
                lsRet = Util.AppSettings("DefaultLanguage");
            else
                lsRet = "Español";

            return lsRet;
        }



        public static void SetCurrency(string lsCurrency)
        {
            HttpContext.Current.Session["Currency"] = lsCurrency;
        }

        public static string GetCurrentCurrency()
        {
            return GetCurrency();
        }

        protected static string GetCurrency()
        {
            string lsRet;

            if (HttpContext.Current.Session["Currency"] != null && (string)HttpContext.Current.Session["Currency"] != "")
                lsRet = (string)HttpContext.Current.Session["Currency"];
            else if (Util.AppSettings("DefaultCurrency") != "")
                lsRet = Util.AppSettings("DefaultCurrency");
            else
                lsRet = "MXP";

            return lsRet;
        }

        public static void AddToCache(string key, Object value)
        {
            Util.AddToCache(key, value);
        }

        public static void SetDates(string fechaInicio, string fechaFin)
        {
            HttpContext.Current.Session["FechaInicio"] = fechaInicio;
            HttpContext.Current.Session["FechaFin"] = fechaFin;
        }


        public static void GetCollapseOption()
        {
            string collapseValue = "0";
            try
            {
                System.Text.StringBuilder query = new System.Text.StringBuilder();
                query.AppendLine("Declare @schema  varchar(40)	= '" + DSODataContext.Schema + "'	         								");
                query.AppendLine("Declare @query varchar(max)		= ''																");
                query.AppendLine("																										");
                query.AppendLine("																										");
                query.AppendLine("Set @query = 																							");
                query.AppendLine("'																										");
                query.AppendLine("	Declare @valueCollapse int =0																		");
                query.AppendLine("	Declare @valueBanderaUsuarDB int =0																	");
                query.AppendLine("	Declare @iCodCatAtrib int =0																		");
                query.AppendLine("																										");
                query.AppendLine("	Select @iCodCatAtrib =  iCodCatalogo																");
                query.AppendLine("	From ['+@schema+'].[VisHistoricos(''Atrib'',''Atributos'',''Español'')] atrib						");
                query.AppendLine("	where dtIniVigencia <> dtFinVigencia																");
                query.AppendLine("	And dtFinVigencia >=  GETDATE()																		");
                query.AppendLine("	And vchCodigo = ''BanderasCliente''   																");
                query.AppendLine("																										");
                query.AppendLine("	Select @valueBanderaUsuarDB = Value																	");
                query.AppendLine("	From ['+@schema+'].[VisHistoricos(''Valores'',''Valores'',''Español'')]								");
                query.AppendLine("	Where dtinivigencia <> dtfinvigencia 																");
                query.AppendLine("	And dtfinVigencia >= GETDATE()																		");
                query.AppendLine("	And atrib = @iCodCatAtrib																			");
                query.AppendLine("	And vchCodigo = ''MenuColapsado''		         													");
                query.AppendLine("																										");
                query.AppendLine("	if(@valueBanderaUsuarDB > 0)																		");
                query.AppendLine("	Begin																								");
                query.AppendLine("																										");
                query.AppendLine("	Select  @valueCollapse =  (BanderasCliente & @valueBanderaUsuarDB) / @valueBanderaUsuarDB			");
                query.AppendLine("	From ['+@schema+'].[VisHistoricos(''Client'',''Clientes'',''Español'')] cliente					");
                query.AppendLine("	where dtIniVigencia <> dtFinVigencia																");
                query.AppendLine("	And dtFinVigencia >= GETDATE()																		");
                query.AppendLine("	And vchCodigo <> ''KeytiaC''																		");
                query.AppendLine("																										");
                query.AppendLine("																										");
                query.AppendLine("	End 																								");
                query.AppendLine("																										");
                query.AppendLine("	Select valueCollapse =  @valueCollapse 																");
                query.AppendLine("'																										");
                query.AppendLine("																										");
                query.AppendLine("exec(@query)																							");

                DataTable dt = DSODataAccess.Execute(query.ToString(), DSODataContext.ConnectionString.ToString());
                
                if (dt.Rows.Count > 0 && dt.Columns.Count > 0)
                {
                    collapseValue = dt.Rows[0][0].ToString() == "0" ? "0" : "1";
                }

                HttpContext.Current.Session["CollapseValue"] = collapseValue;
            }
            catch (Exception)
            {
                HttpContext.Current.Session["CollapseValue"] = collapseValue;
            }

        }
    }
}
