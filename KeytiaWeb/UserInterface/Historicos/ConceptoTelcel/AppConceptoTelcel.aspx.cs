using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.Services;
using KeytiaServiceBL;
using DSOControls2008;
using System.Data;
using System.Collections;
using KeytiaWeb.UserInterface.CCustodia;
using System.Text;

namespace KeytiaWeb.UserInterface
{
    public partial class AppConceptoTelcel : System.Web.UI.Page
    {
        protected DataTable dtConceptosTelcel = new DataTable();
        static string connStr = DSODataContext.ConnectionString;
        protected void Page_Load(object sender, EventArgs e)
        {
            #region Buscar el control de fecha que hereda de la Master y lo oculta ya que aquí no se usa.
            ((Panel)Form.FindControl("pnlRangeFechas")).Visible = false;
            #endregion

            connStr = DSODataContext.ConnectionString;
            if (!Page.IsPostBack)
            {
                FillConceptosTelcelGrid();
            }
        }

        private void FillConceptosTelcelGrid()
        {
            StringBuilder lsbQuery = new StringBuilder();

            lsbQuery.Length = 0;

            lsbQuery.Append("SELECT vchCodAtribOrigen, vchDescAtribOrigen, vchCodAtribDestino, vchDescAtribDestino, ConceptoFiltro, iCodCatalogo, iCodCatAtribDestino, iCodCatAtribOrigen\r");
            lsbQuery.Append("FROM  " + DSODataContext.Schema + ".[vishistoricos('DesgConcTelcelF1','Desglose conceptos Telcel F1','Español')] \r");
            lsbQuery.Append("where dtinivigencia <> dtfinvigencia  \r");
            lsbQuery.Append("and dtfinvigencia >= getdate() \r");
            dtConceptosTelcel = DSODataAccess.Execute(lsbQuery.ToString());

            grvConceptosTelcel.DataSource = dtConceptosTelcel;
            grvConceptosTelcel.DataBind();

            upConceptosTelcel.Update();

        }

        protected void grvConceptosTelcel_EditRow(object sender, ImageClickEventArgs e)
        {
            ImageButton ibtn3 = sender as ImageButton;
            int rowIndex = Convert.ToInt32(ibtn3.Attributes["RowIndex"]);

            GridViewRow selectedRow = (GridViewRow)grvConceptosTelcel.Rows[rowIndex];
            hfdiCodCatalogo.Value = grvConceptosTelcel.DataKeys[rowIndex].Values[0].ToString();
            txtCampoDestinoID.Text = grvConceptosTelcel.DataKeys[rowIndex].Values[1].ToString();
            txtCampoOrigenID.Text = grvConceptosTelcel.DataKeys[rowIndex].Values[2].ToString();

            trCampoDestino.Visible = true;
            trConceptoFiltro.Visible = true;
            trGrabar.Visible = true;

            txtCampoOrigenCod.Text = selectedRow.Cells[0].Text.Trim();
            txtCampoOrigen.Text = selectedRow.Cells[1].Text.Trim();
            txtCampoDestinoCod.Text = selectedRow.Cells[2].Text.Trim();
            txtCampoDestino.Text = selectedRow.Cells[3].Text.Trim();
            if (selectedRow.Cells[4].Text.Trim().Contains("&nbsp;"))
            {
                txtConceptoFiltro.Text = string.Empty;
            }
            else
            {
                txtConceptoFiltro.Text = selectedRow.Cells[4].Text.Trim();
            }

            //Solo si es descripcion se habilitará el campo 
            if (txtCampoDestinoCod.Text.Trim() == "Descripcion")
            {
                txtConceptoFiltro.Enabled = true;
            }
            else
            {
                txtConceptoFiltro.Enabled = false;
            }

            btnGrabar.Text = "Editar";

        }
       
        //Borrar filas inventario
        protected void grvConceptosTelcel_DeleteRow(object sender, ImageClickEventArgs e)
        {
            ImageButton ibtn3 = sender as ImageButton;
            int rowIndex = Convert.ToInt32(ibtn3.Attributes["RowIndex"]); 

            GridViewRow selectedRow = (GridViewRow)grvConceptosTelcel.Rows[rowIndex];
            hfdiCodCatalogo.Value = grvConceptosTelcel.DataKeys[rowIndex].Values[0].ToString();

            lblConceptoTelcel.Text = selectedRow.Cells[1].Text + " - " + selectedRow.Cells[3].Text + " - " + selectedRow.Cells[4].Text;
            
            mpeConceptoTelcel.Show();
        }

        protected void btnEliminarConcepto_PopUp(object sender, EventArgs e)
        {
            try
            {

                DSODataAccess.ExecuteNonQuery("update " + DSODataContext.Schema + ".[vishistoricos('DesgConcTelcelF1','Desglose conceptos Telcel F1','Español')] " +
                                          "set dtFinVigencia = dtIniVigencia, dtFecUltAct = getdate(), iCodUsuario = " + HttpContext.Current.Session["iCodUsuario"].ToString() +
                                          "where iCodCatalogo = " + hfdiCodCatalogo.Value +
                                          "and dtIniVigencia<>dtFinVigencia and dtFinVigencia >= getdate()");
                
                dtConceptosTelcel.Clear();
                FillConceptosTelcelGrid();

            }

            catch (Exception ex)
            {
                mensajeDeAdvertencia("Ocurrio un error al dar de baja el registro.");
                KeytiaServiceBL.Util.LogException("Ocurrio un error al intentar dar de baja el registro iCodCatalogo:" + hfdiCodCatalogo.Value +" usuario: '" + HttpContext.Current.Session["vchCodUsuario"] + "'", ex);
            }
        }

        protected void btnCampoOrigen_Click(object sender, EventArgs e)
        {
            string value = string.Empty;
            trCampoDestino.Visible = true;
            trConceptoFiltro.Visible = false;
            trGrabar.Visible = false;
        }

        protected void btnCampoDestino_Click(object sender, EventArgs e)
        {
            string value = string.Empty;
            trConceptoFiltro.Visible = true;
            trGrabar.Visible = true;

            //Solo si es descripcion se habilitará el campo 
            if (txtCampoDestinoCod.Text.Trim() == "Descripcion")
            {
                txtConceptoFiltro.Enabled = true;
            }
            else
            {
                txtConceptoFiltro.Enabled = false;
            }
        }

        protected void btnGrabar_Click(object sender, EventArgs e)
        {
            bool lbSuccess = true;
            string lsMensaje = string.Empty;
            try
            {
                if (validarDatos())
                {
                    if (hfdiCodCatalogo.Value == string.Empty) //entonces es alta
                    {
                        Hashtable lht = new Hashtable();
                        KeytiaCOM.CargasCOM lCargasCOM = new KeytiaCOM.CargasCOM();
                        int iCodMaestro = int.MinValue;

                        iCodMaestro = Convert.ToInt32(DALCCustodia.getiCodMaestro("Desglose conceptos Telcel F1", "DesgConcTelcelF1"));

                        int iCodCatAtribOrigen = Convert.ToInt32(txtCampoOrigenID.Text);
                        int iCodCatAtribDestino = Convert.ToInt32(txtCampoDestinoID.Text);

                        string vchCodigo = txtCampoOrigenCod.Text.Trim() + "-" + txtCampoDestinoCod.Text.Trim() + "-" + txtConceptoFiltro.Text.Trim();
                        string vchDescripcion = vchCodigo.Substring(0, Math.Min(160, vchCodigo.Length));

                        lht.Add("vchCodigo", vchCodigo.Substring(0, Math.Min(40, vchCodigo.Length)));
                        lht.Add("vchDescripcion", vchDescripcion);
                        lht.Add("iCodMaestro", iCodMaestro);

                        lht.Add("{iCodCatAtribOrigen}", iCodCatAtribOrigen);
                        lht.Add("{vchCodAtribOrigen}", txtCampoOrigenCod.Text.Trim());
                        lht.Add("{vchDescAtribOrigen}", txtCampoOrigen.Text.Trim());

                        lht.Add("{iCodCatAtribDestino}", iCodCatAtribDestino);
                        lht.Add("{vchCodAtribDestino}", txtCampoDestinoCod.Text.Trim());
                        lht.Add("{vchDescAtribDestino}", txtCampoDestino.Text.Trim());

                        lht.Add("{ConceptoFiltro}", txtConceptoFiltro.Text);
                        lht.Add("dtIniVigencia", Convert.ToDateTime("2011-01-01 00:00:00.000"));
                        lht.Add("dtFecUltAct", DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff"));
                        lht.Add("iCodUsuario", HttpContext.Current.Session["iCodUsuario"].ToString());
                        int iCodReg;
                        //Insert a Base de Datos
                        //RZ.20141004 Se habilita replicacion solo en la alta del registro y cuando se trate del esquema keytia. 
                        if (DSODataContext.Schema == "Keytia")
                        {
                            //Solo si se trata de esquema Keytia habrá replicación
                            iCodReg = lCargasCOM.InsertaRegistro(lht, "Historicos", "DesgConcTelcelF1", "Desglose conceptos Telcel F1", true, (int)HttpContext.Current.Session["iCodUsuarioDB"], true);
                        }
                        else
                        {
                            //Si se trata de otro esquema entonces no habrá replicacion
                            iCodReg = lCargasCOM.InsertaRegistro(lht, "Historicos", "DesgConcTelcelF1", "Desglose conceptos Telcel F1", (int)HttpContext.Current.Session["iCodUsuarioDB"]);
                        }
                        

                        if (iCodReg < 0)
                        {
                            lsMensaje = "Ocurrio un error al grabar el registro.";
                            lbSuccess = false;
                        }
                    }
                    else
                    {
                        //Actualizacion del registro
                        StringBuilder lsbUpdate = new StringBuilder();
                        int iCodCatAtribOrigen = Convert.ToInt32(txtCampoOrigenID.Text);
                        int iCodCatAtribDestino = Convert.ToInt32(txtCampoDestinoID.Text);

                        string vchCodigo = txtCampoOrigenCod.Text.Trim() + "-" + txtCampoDestinoCod.Text.Trim() + "-" + txtConceptoFiltro.Text.Trim();
                        string vchDescripcion = vchCodigo.Substring(0, Math.Min(160, vchCodigo.Length));
                        vchCodigo = vchCodigo.Substring(0, Math.Min(40, vchCodigo.Length));

                        lsbUpdate.Append("UPDATE " + DSODataContext.Schema + ".[vishistoricos('DesgConcTelcelF1','Desglose conceptos Telcel F1','Español')] \r");
                        lsbUpdate.Append("set \r");
                        lsbUpdate.Append("vchDescripcion = '" + vchDescripcion + "', \r");
                        lsbUpdate.Append("iCodCatAtribOrigen = " + txtCampoOrigenID.Text.Trim() + ", \r");
                        lsbUpdate.Append("iCodCatAtribDestino = " + txtCampoDestinoID.Text.Trim() + ", \r");
                        lsbUpdate.Append("vchCodAtribDestino = '" + txtCampoDestinoCod.Text.Trim() + "', \r");
                        lsbUpdate.Append("vchDescAtribDestino = '" + txtCampoDestino.Text.Trim() + "', \r");
                        lsbUpdate.Append("vchCodAtribOrigen = '" + txtCampoOrigenCod.Text.Trim() + "', \r");
                        lsbUpdate.Append("vchDescAtribOrigen = '" + txtCampoOrigen.Text.Trim() + "', \r");
                        lsbUpdate.Append("ConceptoFiltro = '" + txtConceptoFiltro.Text.Trim() + "', \r");
                        lsbUpdate.Append("iCodUsuario = " + HttpContext.Current.Session["iCodUsuario"].ToString() + ", \r");
                        lsbUpdate.Append("dtFecUltAct = GETDATE() \r");
                        lsbUpdate.Append("WHERE dtIniVigencia <> dtFinVigencia and dtFinVigencia >= GETDATE() \r");
                        lsbUpdate.Append("and iCodCatalogo = " + hfdiCodCatalogo.Value + " \r");

                        if (!(DSODataAccess.ExecuteNonQuery(lsbUpdate.ToString())))
                        {
                            lsMensaje = "Ocurrio un error al editar el registro.";
                            lbSuccess = false;
                        }
                    }

                }
                else
                {
                    lsMensaje = "El registro ya se encuentra guardado en sistema, verificar datos.";
                    lbSuccess = false;
                }
            }
            catch (Exception ex)
            {
                lsMensaje = "Ocurrio un error al grabar el registro.";
                lbSuccess = false;
                Util.LogException("No se pudo grabar el registro '" + HttpContext.Current.Session["vchCodUsuario"] + "'", ex);
            }
            finally
            {
                if (lbSuccess)
                {
                    FillConceptosTelcelGrid();
                    ClearControls();
                }
                else
                {
                    mensajeDeAdvertencia(lsMensaje);
                }
                
            }
        }

        protected void btnCancelar_Click(object sender, EventArgs e)
        {
            ClearControls();
        }

        protected void ClearControls()
        {
            hfdiCodCatalogo.Value = string.Empty;
            txtCampoOrigenCod.Text = string.Empty;
            txtCampoOrigen.Text = string.Empty;
            txtCampoDestinoCod.Text = string.Empty;
            txtCampoDestino.Text = string.Empty;
            txtConceptoFiltro.Text = string.Empty;
            txtCampoDestinoID.Text = string.Empty;
            txtCampoOrigenID.Text = string.Empty;

            trCampoDestino.Visible = false;
            trConceptoFiltro.Visible = false;
            trGrabar.Visible = false;

            btnGrabar.Text = "Grabar";
        }
        protected bool validarDatos()
        {
            bool lbValida = true;

            StringBuilder lsbValida = new StringBuilder();
            lsbValida.Append("SELECT COUNT(*) FROM \r");
            lsbValida.Append(DSODataContext.Schema + ".[vishistoricos('DesgConcTelcelF1','Desglose conceptos Telcel F1','Español')] \r");
            lsbValida.Append("where dtinivigencia <> dtfinvigencia  \r");
            lsbValida.Append("and dtfinvigencia >= getdate() \r");
            lsbValida.Append("and vchCodAtribDestino = '" + txtCampoDestinoCod.Text.Trim() + "' \r");
            lsbValida.Append("and vchCodAtribOrigen = '" + txtCampoOrigenCod.Text.Trim() + "' \r");
            lsbValida.Append("and ConceptoFiltro = '" + txtConceptoFiltro.Text.Trim() + "'");

            int regs = (int)DSODataAccess.ExecuteScalar(lsbValida.ToString());

            if (regs > 0)
            {
                lbValida = false;
            }
            return lbValida;
        }

        protected void mensajeDeAdvertencia(string mensaje)
        {
            string mensajeJQuery = "<p>" + mensaje + "</p>";
            string script = @"<script type='text/javascript'>alerta('" + mensajeJQuery + "');</script>";
            ScriptManager.RegisterStartupScript(this, typeof(Page), "alerta", script, false);
        }

        [WebMethod]
        public static List<CampoDestino> GetCampoDestino(string id)
        {
            try
            {
                List<CampoDestino> listCampoOrigen = new List<CampoDestino>();
                DataTable ldt = new DataTable();

                ldt = DSODataAccess.Execute("exec spGetAtributosTelcelF2F4 '" + DSODataContext.Schema + "', '" + id + "'");

                foreach (DataRow row in ldt.Rows)
                {
                    int liCodCatalogo = (int)row[0];
                    string lsVchCodigo = row[1].ToString();
                    string lsVchDescripcion = row[2].ToString();

                    listCampoOrigen.Add(new CampoDestino() { iCodCatalogo = liCodCatalogo, vchCodigo = lsVchCodigo, vchDescripcion = lsVchDescripcion });
                }
                
                //List<CampoDestino> listResultado =
                //            (from c in listCampoOrigen
                //             where c.vchDescripcion.Contains(id)
                //             select c).ToList<CampoDestino>();
                return listCampoOrigen;
                //StartsWith(id, StringComparison.CurrentCultureIgnoreCase)

            }
            catch (Exception ex)
            {
                Util.LogException("Ha ocurrido un error al obtener los resultados del Campo Destino", ex);
                return null;
            }
        }

        [WebMethod]
        public static List<CampoOrigen> GetCampoOrigen(string id)
        {
            try
            {
                List<CampoOrigen> listCampoOrigen = new List<CampoOrigen>();

                DataTable dt = DSODataAccess.Execute(" exec spGetAtributosTelcelF1 '" + DSODataContext.Schema + "', '" + id + "'");

                foreach (DataRow row in dt.Rows)
                {
                    int liCodCatalogo = (int) row[0];
                    string lsVchCodigo = row[1].ToString();
                    string lsVchDescripcion = row[2].ToString();

                    listCampoOrigen.Add(new CampoOrigen() { iCodCatalogo = liCodCatalogo, vchCodigo = lsVchCodigo, vchDescripcion = lsVchDescripcion});
                }

                //List<CampoOrigen> listResultado =
                //            (from c in listCampoOrigen
                //             where c.vchDescripcion.Contains(id)
                //             select c).ToList<CampoOrigen>();
                return listCampoOrigen;
                //StartsWith(id, StringComparison.CurrentCultureIgnoreCase)
            }
            catch (Exception ex)
            {
                Util.LogException("Ha ocurrido un error al obtener los resultados del Campo Origen", ex);
                return null;
            }
        }
    }

    public class CampoOrigen
    {
        public int iCodCatalogo { get; set; }
        public string vchCodigo { get; set; }
        public string vchDescripcion { get; set; }
    }

    public class CampoDestino
    {
        public int iCodCatalogo { get; set; }
        public string vchCodigo { get; set; }
        public string vchDescripcion { get; set; }
    }
}
