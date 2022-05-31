using KeytiaServiceBL;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace KeytiaWeb.UserInterface.DashboardFC.TIM
{

    public partial class ConsumoGeneralMoviles : System.Web.UI.Page
    {

        string carrierSelected = "-1";
        string empreSelected = "-1";
        StringBuilder query = new StringBuilder();
        public enum GroupBy { Empresa = 1, Carrier = 2, Concepto = 3 };
        protected void Page_Load(object sender, EventArgs e)
        {
            try
            {
                #region Buscar el control de fecha que hereda de la Master y lo oculta ya que aquí no se usa.
                ((Panel)Form.FindControl("pnlRangeFechas")).Visible = false;
                #endregion
                LeeQueryString();
                //dtCarriers = GetCarriersTIM(0, 0, "ConsumoGeneral");

                //if (dtCarriers.Count > 0) //El Cliente tiene carriers del TIM
                //{
                //    ddlCarrier.Visible = btnAplicar.Visible = false;

                //    //Distinct de Empresas
                //    dtCarriers.GroupBy(g => new { g.Empre, g.EmpreDesc }).Select(x => x.First()).ToList().ForEach(n => dtEmpres.Add(new InfoCarrierGlobal() { Empre = n.Empre, EmpreDesc = n.EmpreDesc }));

                //    if (empreSelected == "-1" && dtEmpres.Count > 1)
                //    {

                //    }
                //    else
                //    {
                //        //En este punto debemos tener una empresa seleccionada
                //        if (empreSelected == "-1" && dtEmpres.Count == 1)
                //        { empreSelected = dtEmpres.First().Empre.ToString(); }

                //        if (carrierSelected == "-1" && dtCarriers.Where(x => x.Empre.ToString() == empreSelected).GroupBy(gpo => gpo.iCodCatalogo).Select(k => k.Key).Count() > 1)
                //        {

                //        }
                //        else
                //        {
                //            if (carrierSelected == "-1" && dtCarriers.Count == 1) { carrierSelected = "1"; }

                //        }
                //    }
                //}

                Navegaciones();
            }
            catch (Exception ex)
            {
                throw new KeytiaWebException("Ocurrio un error en " + Request.Path + HttpContext.Current.Session["vchCodUsuario"] + "'", ex);
            }
        }

        private void LeeQueryString()
        {
            //Se inicializan todos los posibles parametros
            param.Clear();
            param.Add("Nav", string.Empty);
            param.Add("Empre", string.Empty);
            param.Add("Dash", string.Empty);
            param.Add("Sitio", string.Empty);
            param.Add("TDest", string.Empty);
            param.Add("CenCos", string.Empty);
            param.Add("Emple", string.Empty);
            param.Add("TipoLlam", string.Empty);
            param.Add("MesAnio", string.Empty);
            param.Add("NumMarc", string.Empty);
            param.Add("Carrier", string.Empty);
            param.Add("Concepto", string.Empty);
            param.Add("SitioDest", string.Empty);
            param.Add("Linea", string.Empty);
            param.Add("MiConsumo", string.Empty);
            param.Add("Usuario", string.Empty);
            param.Add("NumGpoTronk", string.Empty);
            param.Add("Locali", string.Empty);
            param.Add("Codigo", string.Empty);
            param.Add("Extension", string.Empty);
            param.Add("Level", string.Empty);
            param.Add("Indicador", string.Empty);
            param.Add("DesglosarCosto", string.Empty);
            param.Add("Client", string.Empty);
            param.Add("Sistema", string.Empty);
            param.Add("EmpleConJer", string.Empty);
            param.Add("omitirInfoCDR", string.Empty); //20200122.RJ.Se utiliza para limitar la información en los reportes implementados originalmente para SevenEleven
            param.Add("omitirInfoSiana", string.Empty); //20200122.RJ.Se utiliza para limitar la información en los reportes implementados originalmente para SevenEleven
            param.Add("FiltroNav", string.Empty);
            param.Add("Dia", string.Empty);
            param.Add("NumDesvios", string.Empty);
            param.Add("Hora", string.Empty);
            param.Add("Dispositivo", string.Empty);
            param.Add("TipoDisp", string.Empty);
            for (int i = 0; i < param.Count; i++)
            {
                try
                {
                    if (!string.IsNullOrEmpty(Request.QueryString[param.Keys.ElementAt(i)]))
                    {
                        param[param.Keys.ElementAt(i)] = Request.QueryString[param.Keys.ElementAt(i)];
                    }
                }
                catch (Exception ex)
                {
                    throw new KeytiaWebException(
                        "Ocurrio un error al leer el valor del querystring " + param.Keys.ElementAt(i) + " en " + Request.Path
                        + HttpContext.Current.Session["vchCodUsuario"] + "'", ex);
                }
            }
        }


        public string JavaScriptDrillDown(DataTable ldt, string tipo, string nomColumna)
        {
            tipo = tipo.Replace(" ", "");
            StringBuilder lsb = new StringBuilder();

            lsb.Append("<script type=\"text/javascript\">\n ");
            lsb.Append("function DrillDown" + tipo + "s(" + tipo + "){\n ");
            foreach (DataRow ldr in ldt.Rows)
            {
                lsb.Append("if(" + tipo + " == '" + ldr[nomColumna].ToString() + "'){");
                DataTable DataSource = DSODataAccess.Execute(GetConsumoPorX2Anios(Convert.ToInt32(ldr[nomColumna])));
                if (DataSource.Rows.Count > 0 && DataSource.Columns.Count > 0)
                {
                    DataView dvDataSource = new DataView(DataSource);
                    DataSource = dvDataSource.ToTable(false,
                        new string[] { "Mes", "AÑO ANTERIOR", "AÑO ACTUAL" });
                    DataSource.AcceptChanges();
                }
                string[] DataSourceArrayJSon = FCAndControls.ConvertDataTabletoJSONString(FCAndControls.ConvertDataTabletoDataTableArray(DataSource));

                lsb.Append(FCAndControls.GraficaMultiSeries(DataSourceArrayJSon, FCAndControls.extraeNombreColumnas(DataSource),
                                  "DetalleConsumo" + tipo, "Detalle consumo " + ldr["label"].ToString(), "",
                                  "MES", "IMPORTE", "msline", "$", "", "dti", "98%", "280", false));
                lsb.Append("}");
            }
            lsb.Append("}\n ");
            lsb.Append("</script>\n ");
            return lsb.ToString();
        }

        private string GetConsumoPorX2Anios(int iCodCatTDestCarrierEmpre)
        {
            StringBuilder query = new StringBuilder();
            query.Length = 0;
            query.AppendLine("EXEC [TIMConsumoGeneralPorConceptoOCarrier2Anios] @Esquema = '" + DSODataContext.Schema + "', ");

            //20190523 RM Busca si hay un carrier Seleccionado en el ddlCarrier de se asi  saca el alor y lo manda al query
            int ddlCarrierValue = 0;
            int.TryParse(ddlCarrier.SelectedValue.ToString(), out ddlCarrierValue);


            int carrier = ddlCarrierValue > 0 ? ddlCarrierValue : -1;
            if (empreSelected == "-1")
            {
                query.AppendLine("      @iCodCatEmpre = " + iCodCatTDestCarrierEmpre + ",");
                query.AppendLine("      @iCodCatCarrier = " + carrier + ",");
                query.AppendLine("      @iCodCatTDest = " + -1 + ",");
                query.AppendLine("      @Nvl = 'Empre'");

            }
            else if (carrierSelected == "-1")
            {
                query.AppendLine("      @iCodCatEmpre = " + empreSelected + ",");
                query.AppendLine("      @iCodCatCarrier = " + iCodCatTDestCarrierEmpre + ",");
                query.AppendLine("      @iCodCatTDest = " + -1 + ",");
                query.AppendLine("      @Nvl = 'Carrier'");
            }
            else
            {
                query.AppendLine("      @iCodCatEmpre = " + empreSelected + ",");
                query.AppendLine("      @iCodCatCarrier = " + carrierSelected + ",");
                query.AppendLine("      @iCodCatTDest = " + iCodCatTDestCarrierEmpre + ",");
                query.AppendLine("      @Nvl = 'Categoria'");
            }



            /*Version anterior que dejo NZ
             *
            if (empreSelected == "-1")
            {
                query.AppendLine("      @iCodCatEmpre = " + iCodCatTDestCarrierEmpre + ",");
                query.AppendLine("      @iCodCatCarrier = " + -1 + ",");
                query.AppendLine("      @iCodCatTDest = " + -1 + ",");
                query.AppendLine("      @Nvl = 'Empre'");

            }
            else if (carrierSelected == "-1")
            {
                query.AppendLine("      @iCodCatEmpre = " + empreSelected + ",");
                query.AppendLine("      @iCodCatCarrier = " + iCodCatTDestCarrierEmpre + ",");
                query.AppendLine("      @iCodCatTDest = " + -1 + ",");
                query.AppendLine("      @Nvl = 'Carrier'");
            }
            else
            {
                query.AppendLine("      @iCodCatEmpre = " + empreSelected + ",");
                query.AppendLine("      @iCodCatCarrier = " + carrierSelected + ",");
                query.AppendLine("      @iCodCatTDest = " + iCodCatTDestCarrierEmpre + ",");
                query.AppendLine("      @Nvl = 'Categoria'");
            }
             * 
             */
            return query.ToString();
        }

        public List<InfoCarrierGlobal> GetCarriersTIM(int iCodCarrier, int iCodCatEmpre, string dashboard)
        {
            List<InfoCarrierGlobal> lista = new List<InfoCarrierGlobal>();
            string Carrier = string.IsNullOrEmpty(param["Carrier"]) ? "0" : param["Carrier"];

            string Empre = string.IsNullOrEmpty(param["Empre"]) ? "0" : param["Empre"];
            string Query = $@"EXEC [TIMConsumoGeneralGetCarriersTIM] @Esquema = '{DSODataContext.Schema}',
                                @iCodCatEmpre = {Empre},
                                @iCodCatCarrier = {Carrier},
                                @nomDashboard = 'ConsumoGeneral',
                                @usuario =  {Session["iCodUsuario"]},
                                @incluirInfoTelFija = 1,
                                @incluirInfoTelMovil = 1";
            /*
            query.Length = 0;
            query.AppendLine("EXEC [TIMConsumoGeneralGetCarriersTIM] @Esquema = '" + DSODataContext.Schema + "', ");
            query.AppendLine("      @iCodCatEmpre = " + iCodCatEmpre + ",");
            query.AppendLine("      @iCodCatCarrier = " + iCodCarrier + ",");
            query.AppendLine("      @nomDashboard = '" + dashboard + "'");
            */
            var dt = DSODataAccess.Execute(Query.ToString());
            if (dt != null && dt.Rows.Count > 0)
            {
                foreach (DataRow item in dt.Rows)
                {
                    InfoCarrierGlobal r = new InfoCarrierGlobal();
                    r.iCodCatalogo = Convert.ToInt32(item["iCodCatalogo"]);
                    r.vchDescripcion = item["vchDescripcion"].ToString();
                    r.MaxFecha = Convert.ToInt32(item["MaxFecha"]);
                    r.Año = Convert.ToInt32(item["Anio"]);
                    r.Mes = Convert.ToInt32(item["Mes"]);
                    r.MesNombre = item["MesNombre"].ToString();
                    r.Empre = Convert.ToInt32(item["Empre"]);
                    r.EmpreDesc = item["EmpreDesc"].ToString();
                    r.IdsCarrierEmpre = r.iCodCatalogo + "-" + r.Empre;
                    r.NomCarrierEmpre = r.vchDescripcion + " (" + r.EmpreDesc + ")";
                    lista.Add(r);
                }
            }
            return lista;
        }

    }
}