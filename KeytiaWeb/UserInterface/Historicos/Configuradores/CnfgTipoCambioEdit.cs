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

namespace KeytiaWeb.UserInterface
{
    public class CnfgTipoCambioEdit : HistoricEdit
    {
        protected override void InitGrid()
        {
            base.InitGrid();
            pHisGrid.Config.sAjaxSource = ResolveUrl("~/WebMethods.aspx/GetVisHistorico");

            pHisGrid.Config.aaSorting.Add(new ArrayList());
            pHisGrid.Config.aaSorting[0].Add(pHisGrid.Config.aoColumnDefs.Count - 3); //dtIniVigencia
            pHisGrid.Config.aaSorting[0].Add("asc");
        }

        protected override void InitGridFields()
        {
            DSOGridClientColumn lCol;
            int lTarget = pHisGrid.Config.aoColumnDefs.Count;

            foreach (KeytiaBaseField lField in pFields)
            {
                if (lField.ShowInGrid)
                {
                    lCol = new DSOGridClientColumn();
                    if (lField.Column.StartsWith("iCodCatalogo"))
                    {
                        lCol.sName = lField.ConfigName + "Desc";
                    }
                    else
                    {
                        lCol.sName = lField.ConfigName;
                    }
                    lCol.aTargets.Add(lTarget++);
                    pHisGrid.Config.aoColumnDefs.Add(lCol);
                }
            }
        }

        protected override void InitFiltrosFields()
        {
            DSOTextBox lDSOtxt;
            foreach (KeytiaBaseField lField in pFields)
            {
                if (lField.ShowInGrid)
                {
                    lDSOtxt = new DSOTextBox();
                    lDSOtxt.ID = lField.Column;
                    if (lField.Column.StartsWith("iCodCatalogo"))
                    {
                        lDSOtxt.AddClientEvent("dataFilter", lField.ConfigName + "Desc");
                    }
                    else
                    {
                        lDSOtxt.AddClientEvent("dataFilter", lField.ConfigName);
                    }
                    lDSOtxt.Row = lField.Row + 2;
                    lDSOtxt.ColumnSpan = lField.ColumnSpan;
                    lDSOtxt.Table = pTablaFiltros;
                    lDSOtxt.CreateControls();

                    phtFiltros.Add(lDSOtxt.ID, lDSOtxt);
                }
            }
        }

        protected override DataTable GetDatosRegistro()
        {
            DataTable lDataTable;
            psbQuery.Length = 0;
            psbQuery.AppendLine("select H.*,C.vchCodigo from Historicos H, Catalogos C");
            psbQuery.AppendLine("where H.dtIniVigencia <> H.dtFinVigencia");
            psbQuery.AppendLine("and H.iCodCatalogo = C.iCodRegistro");
            psbQuery.AppendLine("and C.iCodCatalogo = " + iCodEntidad);
            psbQuery.AppendLine("and H.iCodMaestro = " + iCodMaestro);
            psbQuery.AppendLine("and H.iCodRegistro = " + iCodRegistro);

            lDataTable = DSODataAccess.Execute(psbQuery.ToString());
            if (lDataTable.Rows.Count > 0)
            {
                iCodCatalogo = lDataTable.Rows[0]["iCodCatalogo"].ToString();
            }
            else
            {
                iCodCatalogo = null;
            }
            return lDataTable;
        }

        protected override void InitHisGridLanguage()
        {
            KeytiaBaseField lField;
            DSOControlDB lFiltro;
            foreach (DSOGridClientColumn lCol in pHisGrid.Config.aoColumnDefs)
            {
                lField = null;
                if (pFields.ContainsConfigName(lCol.sName))
                {
                    lField = pFields.GetByConfigName(lCol.sName);
                    lCol.sTitle = DSOControl.JScriptEncode(lField.Descripcion);
                }
                else if (lCol.sName.EndsWith("Desc") && pFields.ContainsConfigName(lCol.sName.Substring(0, lCol.sName.Length - 4)))
                {
                    lField = pFields.GetByConfigName(lCol.sName.Substring(0, lCol.sName.Length - 4));
                    lCol.sTitle = DSOControl.JScriptEncode(lField.Descripcion);
                }
                else if (lCol.sName == "vchCodigo")
                {
                    lCol.sTitle = DSOControl.JScriptEncode(Globals.GetMsgWeb(false, "vchCodigo"));
                }
                else if (lCol.sName == "vchDescripcion")
                {
                    lCol.sTitle = DSOControl.JScriptEncode(Globals.GetMsgWeb(false, "vchDescripcion"));
                }
                else if (lCol.sName == "dtIniVigencia")
                {
                    lCol.sTitle = DSOControl.JScriptEncode(Globals.GetMsgWeb(false, "dtIniVigencia"));
                }
                else if (lCol.sName == "dtFinVigencia")
                {
                    lCol.sTitle = DSOControl.JScriptEncode(Globals.GetMsgWeb(false, "dtFinVigencia"));
                }
                else if (lCol.sName == "dtFecUltAct")
                {
                    lCol.sTitle = DSOControl.JScriptEncode(Globals.GetMsgWeb(false, "dtFecUltAct"));
                    lCol.bVisible = false;
                }
                else if (lCol.sName == "Consultar")
                {
                    string lsdoPostBack = DSOControl.JScriptEncode(Page.ClientScript.GetPostBackEventReference(this, "btnConsultar:{0}"));
                    lCol.sTitle = DSOControl.JScriptEncode(Globals.GetMsgWeb(false, "btnConsultar"));
                    lCol.bUseRendered = false;
                    lCol.fnRender = "function(obj){ return " + pjsObj + ".fnRender(obj,\"custom-img-search\",\"" + ResolveUrl("~/images/searchsmall.png") + "\",\"" + lsdoPostBack + "\");}";
                }
                if (phtFiltros.ContainsKey(lCol.sName))
                {
                    lFiltro = (DSOControlDB)phtFiltros[lCol.sName];
                    lFiltro.Descripcion = lCol.sTitle;
                }
                else if (lField != null && phtFiltros.ContainsKey(lField.Column))
                {
                    lFiltro = (DSOControlDB)phtFiltros[lField.Column];
                    lFiltro.Descripcion = lCol.sTitle;
                }
            }
        }

        protected override bool ValidarClaves()
        {
            if (iCodRegistro == "null")
            {
                string lsQuery = "select vchCodigo from Catalogos where iCodRegistro = {0}";
                string lsMoneda = (string)DSODataAccess.ExecuteScalar(string.Format(lsQuery, pFields.GetByConfigName("Moneda").DataValue));
                pvchCodigo.DataValue = lsMoneda;

                int liCodUsuarioDB;
                DataTable lKDBTable = pKDB.GetHisRegByEnt("Usuar", "Usuarios", "iCodCatalogo = " + Session["iCodUsuario"]);
                liCodUsuarioDB = (int)lKDBTable.Rows[0]["{UsuarDB}"];
                lKDBTable = pKDB.GetHisRegByEnt("UsuarDB", "Usuarios DB", "iCodCatalogo = " + liCodUsuarioDB);

                string lsMonedaUsuario = "";
                lsQuery = "select vchCodigo from Catalogos where iCodRegistro = {0}";
                lsMonedaUsuario = (string)DSODataAccess.ExecuteScalar(string.Format(lsQuery, lKDBTable.Rows[0]["{Moneda}"]));
                pvchDescripcion.DataValue = lsMonedaUsuario + " / " + lsMoneda;

                string lsColumna = pFields.GetByConfigName("Moneda").Column;
                lsQuery = "select top (1) iCodRegistro, iCodCatalogo from Historicos where iCodMaestro = {0} and {1} = {2}";
                lsQuery = string.Format(lsQuery, iCodMaestro, lsColumna, pFields.GetByConfigName("Moneda").DataValue);
                DataTable ldtRegistro = DSODataAccess.Execute(lsQuery);
                //string lsRegistro = DSODataAccess.ExecuteScalar(lsQuery).ToString();
                if (ldtRegistro != null && ldtRegistro.Rows.Count > 0)
                {
                    iCodRegistro = ldtRegistro.Rows[0]["iCodRegistro"].ToString();
                    iCodCatalogo = ldtRegistro.Rows[0]["iCodCatalogo"].ToString();
                }
            }

            return base.ValidarClaves();
        }

        protected override bool ValidarDatos()
        {
            bool lbret = true;
            StringBuilder sbErrores = new StringBuilder();
            
            if (!(double.Parse(pFields.GetByConfigName("TipoCambioVal").DataValue.ToString()) > 0))
            {
                lbret = false;
                
                string lsTitulo = DSOControl.JScriptEncode(Globals.GetMsgWeb(false, "TituloHistoricos"));
                sbErrores.Append("<ul><li>");
                sbErrores.Append(DSOControl.JScriptEncode(Globals.GetMsgWeb(false, "TipoCambioInvalido")));
                sbErrores.Append("</ul></li>");
                DSOControl.jAlert(Page, pjsObj + ".ValidarRegistro", sbErrores.ToString(), lsTitulo);
            }

            return lbret;
        }

        public override void SetHistoricState(HistoricState s)
        {
            base.SetHistoricState(s);
            pvchCodigo.TextBox.Enabled = false;
            pvchDescripcion.TextBox.Enabled = false;
        }

        protected override void EditarRegistro()
        {
            base.EditarRegistro();
            pFields.GetByConfigName("Moneda").DisableField();
        }
    }
}
