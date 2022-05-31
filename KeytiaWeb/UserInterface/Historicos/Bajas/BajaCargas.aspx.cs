using KeytiaServiceBL;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace KeytiaWeb.UserInterface.BajaCargas
{
    public partial class BajaCargas : System.Web.UI.Page
    {
        private string esquema = null;
        private string connStr = null;

        #region Eventos
        protected void Page_Load(object sender, EventArgs e)
        {
            esquema = DSODataContext.Schema.ToString();
            connStr = DSODataContext.ConnectionString.ToString();
        }
        
        protected void btnObtenerCargas_Click(object sender, EventArgs e)
        {
            try
            {
                var respuesta = ProcesoCargaChBoxListCargas();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public void btnAceptarBajaCPorFechas_Click(object sender, EventArgs e)
        {
            try
            {
                ProcesoBajaCPorFechas();
                LimpiaCampos();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public void btnAceptarBajaMC_Click(object sender, EventArgs e)
        {
            try
            {
                ProcesoBajaMC();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        protected void btnAceptarBajaCU_Click(object sender, EventArgs e)
        {
            try
            {
                ProcesoBajaCU();
                LimpiaCampos();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        #endregion Eventos

        #region Metodos
       
        public bool ProcesoCargaChBoxListCargas()
        {
            try
            {
                bool respuesta = false;

                string descSitio = txtDescSitioBajaMC.Text.Trim();
                esquema = DSODataContext.Schema;

                string filtro = "";
                DataTable dtCargas = new DataTable();
                dtCargas = BuscarCargas(descSitio, filtro);

                if (dtCargas.Rows.Count > 0)
                {
                    cargarChBxList(dtCargas);
                }

                respuesta = true;
                return respuesta;
            }
            catch (Exception)
            {
                throw;
            }
        }

        public DataTable BuscarCargas(string descSitio, string filtro)
        {
            DataTable dtCargas = new DataTable();

            try
            {
                dtCargas = DSODataAccess.Execute(ConsultaBuscaCargas(descSitio, filtro), connStr);
            }
            catch (Exception ex)
            {
                throw ex;
            }

            return dtCargas;
        }

        public DataTable BuscaCargasEnDetalle(string descSitio, string filtro)
        {
            try
            {
                DataTable dtCargas = new DataTable();
                dtCargas = DSODataAccess.Execute(ConsultaBuscaCargasEnDetalle(descSitio, filtro), connStr);

                return dtCargas;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public string ConsultaCambiaEstatusCarga(int iCodCatCarga)
        {
            try
            {
                StringBuilder query = new StringBuilder();

                query.AppendLine("Declare @estatusCargaFinal int =0");
                query.AppendLine("");
                query.AppendLine("Select  @estatusCargaFinal =  iCodCatalogo");
                query.AppendLine("From [" + esquema + "].[VisHistoricos('EstCarga','Estatus Cargas','Español')]");
                query.AppendLine("where vchCodigo = 'CarEsperaElimina'");
                query.AppendLine("");
                query.AppendLine("");
                query.AppendLine("if(@estatusCargaFinal > 0)");
                query.AppendLine("begin");
                query.AppendLine("	Update cargas");
                query.AppendLine("		set EstCarga = @estatusCargaFinal,");
                query.AppendLine("		    dtFecUltAct = GETDATE()");
                query.AppendLine("	from  [" + esquema + "].[VisHistoricos('Cargas','Cargas CDRs','Español')] cargas");
                query.AppendLine("	Where iCodCatalogo = " + iCodCatCarga + "");
                query.AppendLine("  And dtIniVigencia <> dtFinVigencia ");
                query.AppendLine("  And dtFinVigencia >= GETDATE()");
                query.AppendLine("	");
                query.AppendLine("End ");

                return query.ToString();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public string ConsultaBuscaCargasEnDetalle(string descSitio, string filtro)
        {
            try
            {
                StringBuilder query = new StringBuilder();
                query.AppendLine("Declare @SitioDesc varchar(50) = '" + descSitio + "'");
                query.AppendLine("Declare @iCodCatSitio  int =0");
                query.AppendLine("");
                query.AppendLine("Select ");
                query.AppendLine("	@iCodCatSitio  = iCodCatalogo");
                query.AppendLine("From [" + DSODataContext.Schema + "].[visHisComun('sitio','Español')]");
                query.AppendLine("Where dtIniVigencia <> dtFinVigencia ");
                query.AppendLine("And dtFinVigencia >= GETDATE()");
                query.AppendLine("And vchDescripcion = @SitioDesc");
                query.AppendLine("");
                query.AppendLine("");
                query.AppendLine("if(@iCodCatSitio >0)");
                query.AppendLine("Begin");
                query.AppendLine("	Select iCodRegistro =  car.iCodRegistro");
                query.AppendLine("		from [" + DSODataContext.Schema + "].[VisDetallados('Detall','DetalleCDR','Español')] detall");
                query.AppendLine("Inner Join [" + DSODataContext.Schema + "].[visHistoricos('cargas','cargas CDRs','español')] car");
                query.AppendLine("On car.iCodCatalogo = detall.iCodCatalogo");
                query.AppendLine("And car.dtinivigencia <> car.dtfinvigencia");
                query.AppendLine("And car.dtfinvigencia >= GETDATE()");
                query.AppendLine("	Where detall.Sitio = @iCodCatSitio");
                query.AppendLine("  " + filtro + "");
                query.AppendLine("");
                query.AppendLine("	Group by car.iCodRegistro");
                query.AppendLine("	order by car.iCodRegistro");
                query.AppendLine("End");

                return query.ToString();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public string ConsultaBuscaCargas(string descSitio, string filtro)
        {
            StringBuilder query = new StringBuilder();
            try
            {
                query.AppendLine("Declare @iCodCatEstEliminacionProceso int =0");
                query.AppendLine("Declare @iCodCatSitio int =0");
                query.AppendLine("");
                query.AppendLine("Select @iCodCatSitio = iCodCatalogo");
                query.AppendLine("from [" + DSODataContext.Schema + "].[visHisComun('sitio','Español')]");
                query.AppendLine("Where vchDescripcion = '" + descSitio + "'");
                query.AppendLine("");
                query.AppendLine("Select @iCodCatEstEliminacionProceso  = iCodCatalogo");
                query.AppendLine("from [" + DSODataContext.Schema + "].[VisHistoricos('EstCarga','Estatus Cargas','Español')]");
                query.AppendLine("Where vchDescripcion = 'Eliminación en Proceso'");
                query.AppendLine("");
                query.AppendLine("if(@iCodCatSitio >0)");
                query.AppendLine("Begin");
                query.AppendLine("	Select ");
                query.AppendLine("		iCodRegistro,");
                query.AppendLine("		iCodCatalogo,");
                query.AppendLine("		vchCodigo,");
                query.AppendLine("		vchDescripcion,");
                query.AppendLine("		CampoMuestra = vchCodigo+'	Archivo Carga:	'+ Case When Len(archivo01) >0 Then Archivo01 Else '' End,");
                query.AppendLine("		[Disable]=  Case When Len(clase) >0 Then 'false' Else 'true' End");
                query.AppendLine("	from  [" + DSODataContext.Schema + "].[VisHistoricos('Cargas','Cargas CDRs','Español')]");
                query.AppendLine("	Where Sitio = @iCodCatSitio");
                query.AppendLine("  And dtIniVigencia <> dtFinVigencia  ");
                query.AppendLine("  And EstCarga <> @iCodCatEstEliminacionProceso");
                query.AppendLine("  And dtFinVigencia >= GETDATE()");

                if (filtro.Length > 0)
                {
                    query.AppendLine(filtro);
                }
                query.AppendLine("order by iCodRegistro Desc");
                query.AppendLine("End ");
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return query.ToString();
        }

        public bool cargarChBxList(DataTable dtCargas)
        {
            bool respuesta = false;

            try
            {
                chBxListCargasPorSitio.DataSource = dtCargas;
                chBxListCargasPorSitio.DataValueField = "iCodRegistro";
                chBxListCargasPorSitio.DataTextField = "CampoMuestra";
                chBxListCargasPorSitio.DataBind();

                foreach (DataRow row in dtCargas.Rows)
                {
                    if (row["Disable"].ToString() == "true")
                    {
                        var chBxist = chBxListCargasPorSitio as CheckBoxList;

                        chBxist.Items.FindByValue(row["iCodRegistro"].ToString()).Enabled = false;
                        chBxist.Items.FindByValue(row["iCodRegistro"].ToString()).Attributes.Add("Style", "color:red;");
                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }

            return respuesta;
        }

        public bool CambiaEstatusCarga(string iCodCatCarga)
        {
            try
            {
                bool respuesta = false;
                int catalogo = 0;
                int.TryParse(iCodCatCarga, out catalogo);

                var resp = DSODataAccess.Execute(ConsultaCambiaEstatusCarga(catalogo), connStr);
                respuesta = true;
                return respuesta;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public bool borrarCargas(DataTable dtCargasABorrar)
        {
            try
            {
                bool respuesta = false; ;

                CargasWeb objCargasWeb = new CargasWeb();

                string filtroiCodRegistrosCargasBaja = "";
                foreach (DataRow row in dtCargasABorrar.Rows)
                {
                    filtroiCodRegistrosCargasBaja += row["iCodRegistro"].ToString();
                    filtroiCodRegistrosCargasBaja += ",";
                }

                filtroiCodRegistrosCargasBaja = filtroiCodRegistrosCargasBaja.Trim(new char[] { ' ', ',' });


                ActualizaEstatusCargasABorrar(filtroiCodRegistrosCargasBaja);

                foreach (DataRow row in dtCargasABorrar.Rows)
                {
                    BorrarRegDetallados(Convert.ToInt32(row["iCodRegistro"]));
                }

                ProcesoCargaChBoxListCargas();

                respuesta = true;
                return respuesta;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public bool ActualizaEstatusCargasABorrar(string filtroiCodRegistrosCargasBaja)
        {
            try
            {
                bool respuesta = false;
                DSODataAccess.Execute(ConsutaActualizaEstatusCargasABorrar(filtroiCodRegistrosCargasBaja), connStr);

                respuesta = true;
                return respuesta;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public string ConsutaActualizaEstatusCargasABorrar(string filtroiCodRegistrosCargasBaja)
        {
            try
            {
                StringBuilder query = new StringBuilder();
                query.AppendLine("Declare @iCodCatEstatusBaja int = 0");
                query.AppendLine("");
                query.AppendLine("Select @iCodCatEstatusBaja = iCodCatalogo");
                query.AppendLine("from [" + esquema + "].[VisHistoricos('EstCarga','Estatus Cargas','Español')]");
                query.AppendLine("where dtinivigencia <> dtfinvigencia ");
                query.AppendLine("and dtfinvigencia >= GETDATE()");
                query.AppendLine("aND vchDescripcion  = 'Eliminación en Proceso'");
                query.AppendLine("");
                query.AppendLine("");
                query.AppendLine("if(@iCodCatEstatusBaja >0)");
                query.AppendLine("Begin");
                query.AppendLine("	Update vCargas");
                query.AppendLine("	Set EstCarga = @iCodCatEstatusBaja,");
                query.AppendLine("		dtFecUltAct = GETDATE()");
                query.AppendLine("	from [" + esquema + "].[VisHistoricos('Cargas','Cargas CDRs','Español')] vCargas");
                query.AppendLine("	Where dtIniVigencia <> dtFinVigencia");
                query.AppendLine("	And dtFinVigencia >= GETDATE()");
                query.AppendLine("  And iCodRegistro in (" + filtroiCodRegistrosCargasBaja + " )");
                query.AppendLine("End");

                return query.ToString();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public bool ProcesoBajaCU()
        {
            try
            {
                bool respuesta = false;
                string descSitio = txtDescSitioBajaCU.Text.Trim();
                string claveCarga = txtClaveCU.Text.Trim();
                string filtro = "And vchCodigo = '" + claveCarga + "'";

                DataTable dtCargas = new DataTable();
                dtCargas = BuscarCargas(descSitio, filtro);

                if (dtCargas.Rows.Count > 0)
                {
                    borrarCargas(dtCargas);
                }

                respuesta = true;
                return respuesta;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public bool ProcesoBajaMC()
        {
            try
            {
                bool respuesta = false;
                List<ListItem> listaChBx = chBxListCargasPorSitio.Items.Cast<ListItem>().Where(l => l.Selected).ToList();

                DataTable dtCargas = new DataTable();
                dtCargas.Columns.Add("iCodRegistro", typeof(string));

                foreach (ListItem lI in listaChBx)
                {
                    DataRow row = dtCargas.NewRow();
                    row["iCodRegistro"] = lI.Value;
                    dtCargas.Rows.Add(row);
                }

                if (dtCargas.Rows.Count > 0)
                {
                    borrarCargas(dtCargas);
                }

                return respuesta;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public bool ProcesoBajaCPorFechas()
        {
            try
            {
                bool respuesta = false;

                string descSitio = txtDescSitioBajaCPorFechas.Text;
                string fechaInicioBaja = txtFechaInicioBajaCPorFechas.Text;
                string fechaFinBaja = txtFechaFinBajaCPorFechas.Text;

                Regex expresionFecha = new Regex(@"^\d{4}-\d{2}-\d{2}( \d{2}:\d{2}:\d{2})?(.\d{3})?$");

                if (expresionFecha.IsMatch(fechaInicioBaja) && expresionFecha.IsMatch(fechaFinBaja))
                {
                    DateTime fechaInicio;
                    DateTime fechaFin;
                    if (DateTime.TryParse(fechaInicioBaja, out fechaInicio) && DateTime.TryParse(fechaFinBaja, out fechaFin) && fechaInicio <= fechaFin)
                    {
                        DataTable dtCargas = new DataTable();
                        StringBuilder filtro = new StringBuilder();

                        filtro.AppendLine("And detall.FechaInicio >= '" + fechaInicio.ToString("yyyy-MM-dd HH:mm:ss.fff") + "' ");
                        filtro.AppendLine("And detall.FechaInicio <= '" + fechaFin.ToString("yyyy-MM-dd HH:mm:ss.fff") + "'  ");

                        dtCargas = BuscaCargasEnDetalle(descSitio, filtro.ToString());

                        if (dtCargas.Rows.Count > 0)
                        {
                            borrarCargas(dtCargas);
                        }

                        respuesta = true;
                    }
                }
                return respuesta;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }


        public bool LimpiaCampos()
        {
            try
            {
                bool respuesta = false;

                txtClaveCU.Text = "";
                txtDescSitioBajaCU.Text = "";

                txtDescSitioBajaMC.Text = "";
                chBxListCargasPorSitio.DataSource = new DataTable();
                chBxListCargasPorSitio.DataBind();

                txtDescSitioBajaCPorFechas.Text = "";
                txtFechaFinBajaCPorFechas.Text = "";
                txtFechaInicioBajaCPorFechas.Text = "";

                respuesta = true;
                return respuesta;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        #endregion


        public void BorrarRegDetallados(int iCodRegistro)
        {
            try
            {
                KeytiaCOM.ICargasCOM pCargaCom = (KeytiaCOM.ICargasCOM)Marshal.BindToMoniker("queue:/new:KeytiaCOM.CargasCOM");

                int liCodUsuario = (int)Session["iCodUsuarioDB"];
                pCargaCom.BajaCarga(iCodRegistro, liCodUsuario);
                Marshal.ReleaseComObject(pCargaCom);
                pCargaCom = null;
            }
            catch (Exception e)
            {
                throw new KeytiaWebException("ErrDeleteRecord", e);
            }
        }

       
    }
}