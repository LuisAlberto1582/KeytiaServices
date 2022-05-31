using KeytiaServiceBL;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace KeytiaWeb.UserInterface.DashboardFC.WorkFlow
{
    public partial class Dispositivos : System.Web.UI.Page
    {

        #region Eventos
        protected void Page_Load(object sender, EventArgs e)
        {

            if (!IsPostBack)
            {
                CargarGrid();
                CargarDDLFormMarcas();
                CargarDDLFromTipoRec();
                CargarDDLFromNumeroTel();

                int id;
                Int32.TryParse(String.IsNullOrEmpty(Request.QueryString["Id"]) ? null : Request.QueryString["Id"].ToString(), out id);
                if (id > 0)
                {
                    DeleteDispoditivo(id);
                }
            }
        }

        protected void gridMarcas_RowCommand(object sender, GridViewCommandEventArgs e)
        {
            string commandName = e.CommandName.ToString().ToLower();
            GridViewRow gvr = (GridViewRow)(((LinkButton)e.CommandSource).NamingContainer);
            int RowIndex = gvr.RowIndex;

            DispositivoModel objDisp = new DispositivoModel();
            objDisp.ID = Convert.ToInt32(gridDispositivos.DataKeys[RowIndex].Values["ID"]);
            objDisp.MarcaDispositivoMovilID = Convert.ToInt32(gridDispositivos.DataKeys[RowIndex].Values["MarcaDispositivoMovilID"]);
            objDisp.MarcaClave = gridDispositivos.DataKeys[RowIndex].Values["MarcaClave"].ToString();
            objDisp.MarcaDesc = gridDispositivos.DataKeys[RowIndex].Values["MarcaDesc"].ToString();
            objDisp.ModeloDispositivoMovilID = Convert.ToInt32(gridDispositivos.DataKeys[RowIndex].Values["ModeloDispositivoMovilID"]);
            objDisp.ModeloClave = gridDispositivos.DataKeys[RowIndex].Values["ModeloClave"].ToString();
            objDisp.ModeloDesc = gridDispositivos.DataKeys[RowIndex].Values["ModeloDesc"].ToString();
            objDisp.TipoRecursoId = Convert.ToInt32(gridDispositivos.DataKeys[RowIndex].Values["TipoRecursoId"]);
            objDisp.TipoRecursoClave = gridDispositivos.DataKeys[RowIndex].Values["TipoRecursoClave"].ToString();
            objDisp.TipoRecursoDesc = gridDispositivos.DataKeys[RowIndex].Values["TipoRecursoDesc"].ToString();
            objDisp.IMEI = gridDispositivos.DataKeys[RowIndex].Values["IMEI"].ToString();
            objDisp.SIMCard = gridDispositivos.DataKeys[RowIndex].Values["SIMCard"].ToString();
            objDisp.LineaID = Convert.ToInt32(gridDispositivos.DataKeys[RowIndex].Values["LineaID"]);
            objDisp.NumeroTelefonico = gridDispositivos.DataKeys[RowIndex].Values["NumeroTelefonico"].ToString();
            objDisp.dtIniVigencia = gridDispositivos.DataKeys[RowIndex].Values["dtIniVigencia"].ToString();
            objDisp.dtFinVigencia = gridDispositivos.DataKeys[RowIndex].Values["dtFinVigencia"].ToString();
            objDisp.dtFecUltAct = gridDispositivos.DataKeys[RowIndex].Values["dtFecUltAct"].ToString();

            if (commandName == "editar")
            {
                if (CargarModal(objDisp) || true)
                {
                    mpeModal.Show();
                }
            }
            else if (commandName == "eliminar")
            {
                Type cstype = this.GetType();
                ClientScriptManager cs = Page.ClientScript;
                String cstext = "";
                cstext = "if(confirm('¿Está seguro que desea eliminar el registro?')){ location.assign('Dispositivos.aspx?Id=" + objDisp.ID.ToString() + "'); }";

                cs.RegisterStartupScript(cstype, "", cstext, true);
            }

        }

        protected void ddlMarca_SelectedIndexChanged(object sender, EventArgs e)
        {
            try
            {

                int idMarca = Convert.ToInt32(ddlMarca.SelectedValue.ToString());

                if (idMarca > 0)
                {
                    CargarDDLFromModelosByMarca(idMarca);
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        protected void InsertaDispositivo_Click(object sender, EventArgs e)
        {
            InsertDispositivo();
        }

        protected void ddlMarcaClaveModal_SelectedIndexChanged(object sender, EventArgs e)
        {
            int idMarca = Convert.ToInt32(ddlMarcaClaveModal.SelectedValue.ToString());
            CargarDDLFromModelosByMarcaModal(idMarca);
            mpeModal.Show();

        }
        #endregion

        #region Métodos


        #region Dispositivos
        public void CargarGrid()
        {
            try
            {
                gridDispositivos.DataSource = SelectAllDisp();
                gridDispositivos.DataBind();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public DataTable SelectAllDisp()
        {
            try
            {
                DataTable dt = DSODataAccess.Execute(QuerySelectAllDisp(DSODataContext.Schema), DSODataContext.ConnectionString);

                return dt;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public string QuerySelectAllDisp(string schema)
        {
            StringBuilder query = new StringBuilder();
            query.AppendLine("");
            try
            {


                query.AppendLine("Select 															        ");
                query.AppendLine("	ID = disp.ID,													        ");
                query.AppendLine("	MarcaDispositivoMovilID = disp.MarcaDispositivoMovilID,			        ");
                query.AppendLine("	MarcaClave = marca.Clave,										        ");
                query.AppendLine("	MarcaDesc = marca.Descripcion,									        ");
                query.AppendLine("	ModeloDispositivoMovilID = disp.ModeloDispositivoMovilID,		        ");
                query.AppendLine("	ModeloClave = modelo.Clave,										        ");
                query.AppendLine("	ModeloDesc = modelo.Descripcion,								        ");
                query.AppendLine("	TipoRecursoId = disp.TipoRecursoId,								        ");
                query.AppendLine("	TipoRecursoClave = recurso.Clave,								        ");
                query.AppendLine("	TipoRecursoDesc = recurso.Descripcion,							        ");
                query.AppendLine("	IMEI = disp.IMEI,												        ");
                query.AppendLine("	SIMCard	 = disp.SIMCard,										        ");
                query.AppendLine("	LineaID = disp.LineaID,											        ");
                query.AppendLine("	NumeroTelefonico = disp.NumeroTelefonico,						        ");
                query.AppendLine("	dtIniVigencia = Convert(varchar,disp.dtIniVigencia,11),								        ");
                query.AppendLine("	dtFinVigencia = Convert(varchar,disp.dtFinVigencia,11),								        ");
                query.AppendLine("	dtFecUltAct = Convert(varchar,disp.dtFecUltAct,11)									        ");
                query.AppendLine("From [" + schema + "].[WorkflowLineasDispositivoMovil] disp				    ");
                query.AppendLine("Inner Join [" + schema + "].WorkflowLineasMarcaDipositivoMovil marca	    ");
                query.AppendLine("	On disp.MarcaDispositivoMovilID = marca.id						        ");
                query.AppendLine("	And marca.dtIniVigencia <> marca.dtFinVigencia					        ");
                query.AppendLine("	And marca.dtFinVigencia >= GETDATE()							        ");
                query.AppendLine("Inner Join [" + schema + "].WorkflowLineasModeloDispositivoMovil modelo	");
                query.AppendLine("	On Disp.ModeloDispositivoMovilID = modelo.id					        ");
                query.AppendLine("	And modelo.dtIniVigencia <> modelo.dtFinVigencia				        ");
                query.AppendLine("	And modelo.dtFinVigencia >= GETDATE()							        ");
                query.AppendLine("Inner Join [" + schema + "].WorkflowLineasTipoRecurso recurso			    ");
                query.AppendLine("	On Disp.TipoRecursoId = recurso.id								        ");
                query.AppendLine("	And recurso.dtIniVigencia <> recurso.dtFinVigencia				        ");
                query.AppendLine("	And recurso.dtFinVigencia >= GETDATE()							        ");
                query.AppendLine("Where disp.dtIniVigencia <> disp.dtFinVigencia					        ");
                query.AppendLine("And disp.dtFinVigencia >= GETDATE()								        ");
                query.AppendLine(" Order by dtfecultact asc");

            }
            catch (Exception ex)
            {
                throw ex;
            }

            return query.ToString();
        }

        public void InsertDispositivo()
        {
            try
            {
                string res = string.Empty;

                DispositivoModel objDispositivo = new DispositivoModel();

                objDispositivo.MarcaDispositivoMovilID = Convert.ToInt32(ddlMarca.SelectedValue.ToString());
                objDispositivo.MarcaClave = ddlMarca.SelectedItem.Text.ToUpper();
                objDispositivo.MarcaDesc = ddlMarca.SelectedItem.Text.ToUpper();

                objDispositivo.ModeloDispositivoMovilID = Convert.ToInt32(ddlModelo.SelectedValue.ToString());
                objDispositivo.ModeloClave = ddlModelo.SelectedItem.Text.ToUpper();
                objDispositivo.ModeloDesc = ddlModelo.SelectedItem.Text.ToUpper();

                objDispositivo.TipoRecursoId = Convert.ToInt32(ddlTipoRecurso.SelectedValue.ToString());
                objDispositivo.TipoRecursoClave = ddlTipoRecurso.SelectedItem.Text.ToUpper().ToString();
                objDispositivo.TipoRecursoDesc = ddlTipoRecurso.SelectedItem.Text.ToUpper().ToString();

                objDispositivo.IMEI = txtIMEI.Text;
                objDispositivo.SIMCard = txtSIM.Text;
                objDispositivo.NumeroTelefonico = ddlNumeroTelefonico.SelectedValue != "0" ? ddlNumeroTelefonico.SelectedItem.Text.ToUpper().ToString() : "";

                res = ValidaDispInsert(objDispositivo).ToLower();
                if (res == "ok")
                {
                    DataTable dt = new DataTable();
                    dt = DSODataAccess.Execute(QueryInsertDisp(objDispositivo, DSODataContext.Schema), DSODataContext.ConnectionString);

                    res = dt.Rows[0][0].ToString();
                }

                Type cstype = this.GetType();
                ClientScriptManager cs = Page.ClientScript;
                String cstext = "alert('" + res + "'); location.href = 'Dispositivos.aspx';";
                cs.RegisterStartupScript(cstype, "", cstext, true);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public string ValidaDispInsert(DispositivoModel objdisp)
        {
            string res = "OK";

            try
            {
                if (objdisp.IMEI.Length > 0)
                {
                    if (!Regex.IsMatch(objdisp.IMEI, @"^\d{15}$"))
                    {
                        res = "La longitud del IMEI debe ser de 15 digitos. ";
                    }
                }

                if (objdisp.SIMCard.Length > 0)
                {
                    if (objdisp.SIMCard.Trim().Length != 19)
                    {
                        res += "La longitud de la SIM debe de ser de 19 caracteres. ";
                    }
                }



            }
            catch (Exception ex)
            {
                res = "Ocurrio un error en la validacion del dispositivo. ";
            }

            return res;
        }

        public string QueryInsertDisp(DispositivoModel objDisp, string schema)
        {
            StringBuilder query = new StringBuilder();
            query.AppendLine("");

            try
            {
                query.AppendLine("Declare @rowcount						int =0											");
                query.AppendLine("Declare @MarcaDispositivoMovilID		int =" + objDisp.MarcaDispositivoMovilID + "		");
                query.AppendLine("Declare @ModeloDispositivoMovilID		int =" + objDisp.ModeloDispositivoMovilID + "		");
                query.AppendLine("Declare @TipoRecursoId				int =" + objDisp.TipoRecursoId + "	    			");
                query.AppendLine("Declare @IMEI							varchar(30) ='" + objDisp.IMEI + "'				");
                query.AppendLine("Declare @SIMCard						varchar(30) ='" + objDisp.SIMCard + "'			");
                query.AppendLine("Declare @LineaID						int =" + objDisp.LineaID + "						");
                query.AppendLine("Declare @NumeroTelefonico				varchar(max) = '" + objDisp.NumeroTelefonico + "'	");
                query.AppendLine("Declare @dtIniVigencia				varchar(max) = '" + objDisp.dtIniVigencia + "'	    ");
                query.AppendLine("Declare @dtFinVigencia				varchar(max) = '" + objDisp.dtFinVigencia + "'		");
                query.AppendLine("																						");
                query.AppendLine("																						");
                query.AppendLine("Declare @IMEIUnico     int =0															");
                query.AppendLine("Declare @lineaValida   int =0															");
                query.AppendLine("Declare @lineaNoEnDisp int =0															");
                query.AppendLine("																						");
                query.AppendLine("Declare @res varchar(max) = ''														");
                query.AppendLine("																						");
                query.AppendLine("Select @IMEIUnico = count(*)															");
                query.AppendLine("From [" + schema + "].WorkflowLineasDispositivoMovil										");
                query.AppendLine("Where dtIniVigencia <> dtFinVigencia													");
                query.AppendLine("And dtFinVigencia >= GETDATE()														");
                query.AppendLine("And IMEI = @IMEI																		");
                query.AppendLine("																						");
                query.AppendLine("Select @lineaValida = count(*)														");
                query.AppendLine("From [" + schema + "].[VisHistoricos('Linea','Lineas','Español')] linea				");
                query.AppendLine("Where dtIniVigencia <> dtFinVigencia													");
                query.AppendLine("And dtFinVigencia >= GETDATE()														");
                query.AppendLine("And iCodCatalogo = @LineaID															");
                query.AppendLine("																						");
                query.AppendLine("Select @lineaNoEnDisp = count(*)														");
                query.AppendLine("From [" + schema + "].WorkflowLineasDispositivoMovil					     			");
                query.AppendLine("Where dtIniVigencia <> dtFinVigencia													");
                query.AppendLine("And dtFinVigencia >=  GETDATE()														");
                query.AppendLine("And LineaID = @LineaID																");
                query.AppendLine("																						");
                query.AppendLine("if(@IMEIUnico =0 or @IMEI = '')														");
                query.AppendLine("Begin																					");
                query.AppendLine("	if(@lineaValida = 1 or @LineaID = 0)												");
                query.AppendLine("	Begin																				");
                query.AppendLine("		if(@lineaNoEnDisp = 0 or @LineaID =0)											");
                query.AppendLine("		Begin																			");
                query.AppendLine("			Insert [" + schema + "].WorkflowLineasDispositivoMovil	    				");
                query.AppendLine("			(																			");
                query.AppendLine("				MarcaDispositivoMovilID,												");
                query.AppendLine("				ModeloDispositivoMovilID,												");
                query.AppendLine("				TipoRecursoId,															");
                query.AppendLine("				IMEI,																	");
                query.AppendLine("				SIMCard,																");
                query.AppendLine("				LineaID,																");
                query.AppendLine("				NumeroTelefonico,														");
                query.AppendLine("				dtIniVigencia,															");
                query.AppendLine("				dtFinVigencia,															");
                query.AppendLine("				dtFecUltAct																");
                query.AppendLine("			)																			");
                query.AppendLine("			values																		");
                query.AppendLine("			(																			");
                query.AppendLine("				@MarcaDispositivoMovilID,												");
                query.AppendLine("				@ModeloDispositivoMovilID,												");
                query.AppendLine("				@TipoRecursoId,															");
                query.AppendLine("				@IMEI,																	");
                query.AppendLine("				@SIMCard,																");
                query.AppendLine("				@LineaID,																");
                query.AppendLine("				@NumeroTelefonico,														");
                query.AppendLine("				'2011-01-01 00:00:00',													");
                query.AppendLine("				'2079-01-01 00:00:00',													");
                query.AppendLine("				GETDATE()																");
                query.AppendLine("			)																			");
                query.AppendLine("																						");
                query.AppendLine("			Set @rowcount = @@ROWCOUNT													");
                query.AppendLine("			Set @res = 'Registro insertado correctamente. '								");
                query.AppendLine("		End 																			");
                query.AppendLine("		Else																			");
                query.AppendLine("		Begin																			");
                query.AppendLine("			Set	@res = 'La linea ya se encuentra asignada a un dispositivo. '			");
                query.AppendLine("		End 																			");
                query.AppendLine("	End																					");
                query.AppendLine("	Else																				");
                query.AppendLine("	Begin																				");
                query.AppendLine("		Set	@res = 'La linea no cuenta con un id valido. '								");
                query.AppendLine("	End 																				");
                query.AppendLine("End 																					");
                query.AppendLine("Else																					");
                query.AppendLine("Begin																					");
                query.AppendLine("	Set	@res = 'El IMEI del dispositivo debe de ser unico. '							");
                query.AppendLine("End																					");
                query.AppendLine("Select @res ");

            }
            catch (Exception ex)
            {
                throw ex;
            }
            return query.ToString();
        }
        #endregion

        #region Marcas
        public void CargarDDLFormMarcas()
        {
            try
            {
                ddlMarca.DataSource = SelectAllMarcas();
                ddlMarca.DataBind();

                ddlMarcaClaveModal.DataSource = SelectAllMarcas();
                ddlMarcaClaveModal.DataBind();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public DataTable SelectAllMarcas()
        {
            DataTable dt = new DataTable();
            try
            {
                dt = DSODataAccess.Execute(QuerySelectAllMarcas(DSODataContext.Schema), DSODataContext.ConnectionString);
            }
            catch (Exception ex)
            {
                throw ex;
            }

            return dt;
        }

        public string QuerySelectAllMarcas(string schema)
        {
            StringBuilder query = new StringBuilder();

            try
            {
                query.AppendLine("Select");
                query.AppendLine("    ID,");
                query.AppendLine("    Clave,");
                query.AppendLine("    Descripcion,");
                query.AppendLine("    dtIniVigencia,");
                query.AppendLine("    dtFinVigencia,");
                query.AppendLine("    dtFecUltAct");
                query.AppendLine("From [" + schema + "].[WorkflowLineasMarcaDipositivoMovil]");
                query.AppendLine("Where dtIniVigencia<> dtFinVigencia");
                query.AppendLine("And dtFinVigencia >= GETDATE()");
                query.AppendLine("And Len(Clave) > 0");
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return query.ToString();
        }
        #endregion

        #region Modelos
        public void CargarDDLFromModelosByMarca(int idMarca = 0)
        {
            try
            {
                ddlModelo.DataSource = SelectModelosByMarca(idMarca);
                ddlModelo.DataBind();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public void CargarDDLFromModelosByMarcaModal(int idMarca = 0)
        {
            try
            {

                ddlModeloClaveModal.Items.Clear();
                ddlModeloClaveModal.Items.Insert(0, new ListItem("Selecciona", ""));

                ddlModeloClaveModal.DataSource = SelectModelosByMarca(idMarca);
                ddlModeloClaveModal.DataBind();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public DataTable SelectModelosByMarca(int idMarca)
        {
            DataTable dt = new DataTable();
            try
            {
                dt = DSODataAccess.Execute(QuerySelectModelosPorMarca(DSODataContext.Schema, idMarca), DSODataContext.ConnectionString);
            }
            catch (Exception ex)
            {
                throw ex;
            }

            return dt;
        }

        public string QuerySelectModelosPorMarca(string schema, int idMarca)
        {
            StringBuilder query = new StringBuilder();

            try
            {

                query.AppendLine("Declare @marcaDisID int = " + idMarca + "");
                query.AppendLine("");
                query.AppendLine("Select");
                query.AppendLine("    ID,");
                query.AppendLine("    Clave,");
                query.AppendLine("    Descripcion,");
                query.AppendLine("    MarcaDispositivoMovilID,");
                query.AppendLine("    dtIniVigencia,");
                query.AppendLine("    dtFinVigencia,");
                query.AppendLine("    dtFecUltAct");
                query.AppendLine("From [" + schema + "].[WorkflowLineasModeloDispositivoMovil]");
                query.AppendLine("Where dtIniVigencia<> dtFinVigencia");
                query.AppendLine("And dtFinVigencia >= GETDATE()");
                query.AppendLine("And Len(Clave) > 0");
                query.AppendLine("And MarcaDispositivoMovilID = @marcaDisID");
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return query.ToString();
        }
        #endregion

        #region TipoRec
        public void CargarDDLFromTipoRec()
        {
            try
            {
                ddlTipoRecurso.DataSource = SelectAllTipoRec();
                ddlTipoRecurso.DataBind();

                ddlTipoRecursoClaveModal.DataSource = SelectAllTipoRec();
                ddlTipoRecursoClaveModal.DataBind();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public DataTable SelectAllTipoRec()
        {
            DataTable dt = new DataTable();
            try
            {
                dt = DSODataAccess.Execute(QuerySelectAllTiposRecursos(DSODataContext.Schema), DSODataContext.ConnectionString);
            }
            catch (Exception ex)
            {
                throw ex;
            }

            return dt;
        }

        public string QuerySelectAllTiposRecursos(string schema)
        {
            StringBuilder query = new StringBuilder();

            try
            {

                query.AppendLine("Select");
                query.AppendLine("    Id,");
                query.AppendLine("    Clave,");
                query.AppendLine("    Descripcion,");
                query.AppendLine("    dtIniVigencia,");
                query.AppendLine("    dtFinVigencia,");
                query.AppendLine("    dtFecUltAct");
                query.AppendLine("From [" + schema + "].WorkflowLineasTipoRecurso");
                query.AppendLine("Where Clave = 'Celular'");
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return query.ToString();
        }
        #endregion

        #region NumTel
        public void CargarDDLFromNumeroTel(int idLinea = 0)
        {
            try
            {
                ddlNumeroTelefonico.DataSource = SelectAllNumeroTel();
                ddlNumeroTelefonico.DataBind();

                ddlLineaIDModal.DataSource = SelectAllNumeroTel();
                ddlLineaIDModal.DataBind();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public DataTable SelectAllNumeroTel(int idLinea = 0)
        {
            DataTable dt = new DataTable();
            try
            {
                dt = DSODataAccess.Execute(QuerySellectAllNumTel(DSODataContext.Schema), DSODataContext.ConnectionString);
            }
            catch (Exception ex)
            {
                throw ex;
            }

            return dt;
        }

        public string QuerySellectAllNumTel(string schema, int idLinea = 0)
        {
            StringBuilder query = new StringBuilder();

            try
            {
                query.AppendLine("Select");
                query.AppendLine("    ID = iCodCatalogo,");
                query.AppendLine("    Clave = Tel,");
                query.AppendLine("    vchCodigo,");
                query.AppendLine("    vchDescripcion,");
                query.AppendLine("    dtIniVigencia,");
                query.AppendLine("    dtFinVigencia,");
                query.AppendLine("    Tel");
                query.AppendLine("From[" + schema + "].[visHistoricos('linea', 'lineas', 'español')]");
                query.AppendLine("Where dtinivigencia<> dtfinvigencia");
                query.AppendLine("And dtfinvigencia >= GETDATE()");
                query.AppendLine("And");
                query.AppendLine("  (icodcatalogo not in ");
                query.AppendLine("     (");
                query.AppendLine("        Select lineaID");
                query.AppendLine("        From[" + schema + "].[WorkflowLineasDispositivoMovil]");
                query.AppendLine("      )");
                query.AppendLine("     Or");
                query.AppendLine("     (icodcatalogo = " + idLinea + ")");
                query.AppendLine(")");
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return query.ToString();
        }


        #endregion

        #region Modal
        public bool CargarModal(DispositivoModel objDisp)
        {
            bool res = false;
            try
            {
                CargarDDLFromModelosByMarcaModal(objDisp.MarcaDispositivoMovilID);
                CargarDDLFromNumeroTel(objDisp.LineaID);


                txtIDModal.Text = objDisp.ID.ToString();
                txtMarcaDispositivoMovilIDModal.Text = objDisp.MarcaDispositivoMovilID.ToString();
                txtModeloDispositivoMovilIDModal.Text = objDisp.ModeloDispositivoMovilID.ToString();
                txtTipoRecursoIdModal.Text = objDisp.TipoRecursoId.ToString();
                ddlMarcaClaveModal.SelectedValue = objDisp.MarcaDispositivoMovilID.ToString();
                ddlModeloClaveModal.SelectedValue = objDisp.ModeloDispositivoMovilID.ToString();
                ddlTipoRecursoClaveModal.SelectedValue = objDisp.TipoRecursoId.ToString();
                txtIMEIModal.Text = objDisp.IMEI.ToString();
                txtSIMCardModal.Text = objDisp.SIMCard.ToString();
                ddlLineaIDModal.SelectedValue = objDisp.LineaID.ToString();

                res = true;
            }
            catch (Exception ex)
            {
                res = false;
            }

            return res;
        }
        #endregion

        #endregion

        protected void btnAceptar_Click(object sender, EventArgs e)
        {
            DispositivoModel objDisp = new DispositivoModel();
            try
            {
                objDisp.ID = Convert.ToInt32(txtIDModal.Text);
                objDisp.MarcaDispositivoMovilID = Convert.ToInt32(ddlMarcaClaveModal.SelectedValue.ToString());
                objDisp.MarcaClave = ddlMarcaClaveModal.SelectedItem.Text.ToString();
                objDisp.MarcaDesc = ddlMarcaClaveModal.SelectedItem.Text.ToString();
                objDisp.ModeloDispositivoMovilID = Convert.ToInt32(ddlModeloClaveModal.SelectedValue.ToString());
                objDisp.ModeloClave = ddlModeloClaveModal.SelectedItem.Text.ToString();
                objDisp.ModeloDesc = ddlModeloClaveModal.SelectedItem.Text.ToString();
                objDisp.TipoRecursoId = Convert.ToInt32(ddlModeloClaveModal.SelectedValue.ToString());
                objDisp.TipoRecursoClave = ddlTipoRecurso.SelectedItem.Text;
                objDisp.TipoRecursoDesc = ddlTipoRecurso.SelectedItem.Text;
                objDisp.IMEI = txtIMEIModal.Text.ToString();
                objDisp.SIMCard = txtSIMCardModal.Text.ToString();
                objDisp.LineaID = Convert.ToInt32(ddlLineaIDModal.SelectedValue.ToString());
                objDisp.NumeroTelefonico = ddlLineaIDModal.SelectedItem.Text.ToString();

                UpdateDisp(objDisp);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public void UpdateDisp(DispositivoModel objDisp)
        {
            string res = string.Empty;
            try
            {
                DataTable dt = DSODataAccess.Execute(QueryUpdateDisp(objDisp, DSODataContext.Schema), DSODataContext.ConnectionString);
                res = dt.Rows[0][0].ToString();
            }
            catch (Exception ex)
            {
                res = "Ocurrio un error al modificar el registro. ";
            }

            Type cstype = this.GetType();
            ClientScriptManager cs = Page.ClientScript;
            String cstext = "alert('" + res + "'); location.href = 'Dispositivos.aspx';";
            cs.RegisterStartupScript(cstype, "", cstext, true);
        }

        public string QueryUpdateDisp(DispositivoModel objDisp, string schema)
        {
            StringBuilder query = new StringBuilder();
            query.AppendLine("");
            try
            {
                query.AppendLine("Declare @rowCount int = 0																								");
                query.AppendLine("Declare @res varchar(max) = ''																						");
                query.AppendLine("																														");
                query.AppendLine("Declare @idDisp int = " + objDisp.ID + "																					");
                query.AppendLine("Declare @MarcaDispositivoMovilID int = " + objDisp.MarcaDispositivoMovilID + "										");
                query.AppendLine("Declare @ModeloDispositivoMovilID int = " + objDisp.ModeloDispositivoMovilID + "									    ");
                query.AppendLine("Declare @TipoRecursoId int = " + objDisp.TipoRecursoId + "															");
                query.AppendLine("Declare @IMEI varchar(30) = '" + objDisp.IMEI + "'																		");
                query.AppendLine("Declare @SIMCard varchar(30) = '" + objDisp.SIMCard + "'																");
                query.AppendLine("Declare @LineaID int = " + objDisp.LineaID + "																		");
                query.AppendLine("Declare @NumeroTelefonico varchar(30)	 = '" + objDisp.NumeroTelefonico + "'                                             ");
                query.AppendLine("																														");
                query.AppendLine("																														");
                query.AppendLine("Declare @existeIdDisp int = 0																							");
                query.AppendLine("Declare @existeMarcaDispositivoMovilID int = 0																		");
                query.AppendLine("Declare @existeModeloDispositivoMovilID int = 0																		");
                query.AppendLine("Declare @existeTipoRecursoId int = 0																					");
                query.AppendLine("Declare @unicoIMEI int = 0																							");
                query.AppendLine("Declare @unicoSIMCard int = 0																							");
                query.AppendLine("Declare @unicoLineaID int = 0																							");
                query.AppendLine("																														");
                query.AppendLine("");
                query.AppendLine("if(convert(int,@lineaID) > 0)");
                query.AppendLine("Begin");
                query.AppendLine("Set @NumeroTelefonico = ''");
                query.AppendLine("End");
                query.AppendLine("");
                query.AppendLine("Select @existeIdDisp = count(*)																						");
                query.AppendLine("From [" + schema + "].WorkflowLineasDispositivoMovil																		");
                query.AppendLine("Where dtIniVigencia <> dtFinVigencia																					");
                query.AppendLine("And dtFinVigencia >= GETDATE()																						");
                query.AppendLine("And ID = @idDisp																										");
                query.AppendLine("																														");
                query.AppendLine("Select @existeMarcaDispositivoMovilID  = count(*)																		");
                query.AppendLine("From [" + schema + "].WorkflowLineasMarcaDipositivoMovil																");
                query.AppendLine("Where dtIniVigencia <> dtFinVigencia																					");
                query.AppendLine("And dtFinVigencia >= GETDATE()																						");
                query.AppendLine("And id = @MarcaDispositivoMovilID																						");
                query.AppendLine("																														");
                query.AppendLine("Select @existeModeloDispositivoMovilID =Count(*)																		");
                query.AppendLine("From [" + schema + "].WorkflowLineasModeloDispositivoMovil																");
                query.AppendLine("Where dtIniVigencia <> dtFinVigencia																					");
                query.AppendLine("And dtFinVigencia >= GETDATE()																						");
                query.AppendLine("And id = @ModeloDispositivoMovilID																					");
                query.AppendLine("																														");
                query.AppendLine("Select @existeTipoRecursoId =Count(*)																					");
                query.AppendLine("From [" + schema + "].WorkflowLineasTipoRecurso																		");
                query.AppendLine("Where dtIniVigencia <> dtFinVigencia																					");
                query.AppendLine("And dtFinVigencia >= GETDATE()																						");
                query.AppendLine("And id = @TipoRecursoId																								");
                query.AppendLine("																														");
                query.AppendLine("Select @unicoIMEI = Count(*)																							");
                query.AppendLine("From [" + schema + "].WorkflowLineasDispositivoMovil																	");
                query.AppendLine("Where dtIniVigencia <> dtFinVigencia																					");
                query.AppendLine("And dtFinVigencia >= GETDATE()																						");
                query.AppendLine("And IMEI = @IMEI																										");
                query.AppendLine("And id <> @idDisp");
                query.AppendLine("																														");
                query.AppendLine("Select @unicoSIMCard = count(*)																						");
                query.AppendLine("From [" + schema + "].WorkflowLineasDispositivoMovil																	");
                query.AppendLine("Where dtIniVigencia <> dtFinVigencia																					");
                query.AppendLine("And dtFinVigencia >= GETDATE()																						");
                query.AppendLine("And SIMCard = @SIMCard																								");
                query.AppendLine("And id <> @idDisp");
                query.AppendLine("																														");
                query.AppendLine("Select @unicoLineaID = count(*)																						");
                query.AppendLine("From [" + schema + "].WorkflowLineasDispositivoMovil																	");
                query.AppendLine("Where dtIniVigencia <> dtFinVigencia																					");
                query.AppendLine("And dtFinVigencia >= GETDATE()																						");
                query.AppendLine("And LineaID = @LineaID																								");
                query.AppendLine("																														");
                query.AppendLine("																														");
                query.AppendLine("if(@existeIdDisp = 1)																									");
                query.AppendLine("Begin																													");
                query.AppendLine("	if(@existeMarcaDispositivoMovilID = 1)																				");
                query.AppendLine("	Begin																												");
                query.AppendLine("		if(@existeModeloDispositivoMovilID = 1)																			");
                query.AppendLine("		Begin																											");
                query.AppendLine("			if(@existeTipoRecursoId = 1)																				");
                query.AppendLine("			Begin																										");
                query.AppendLine("				if(@LineaID =0 or  @unicoLineaID = 0)																	");
                query.AppendLine("				Begin																									");
                query.AppendLine("					if(@IMEI ='' or @unicoIMEI =0)																		");
                query.AppendLine("					Begin																								");
                query.AppendLine("						if(@SIMCard = '' or @unicoSIMCard =0)															");
                query.AppendLine("						Begin																							");
                query.AppendLine("							Update Disp																					");
                query.AppendLine("							Set																							");
                query.AppendLine("								MarcaDispositivoMovilID = @MarcaDispositivoMovilID,										");
                query.AppendLine("								ModeloDispositivoMovilID = @ModeloDispositivoMovilID,									");
                query.AppendLine("								TipoRecursoId = @TipoRecursoId,															");
                query.AppendLine("								IMEI = @IMEI,																			");
                query.AppendLine("								SIMCard =@SIMCard,																		");
                query.AppendLine("								LineaID = @LineaID,																		");
                query.AppendLine("								NumeroTelefonico = @NumeroTelefonico,													");
                query.AppendLine("                              dtfecUltAct = GETDATE() ");
                query.AppendLine("							From [" + schema + "].WorkflowLineasDispositivoMovil Disp									");
                query.AppendLine("							where dtIniVigencia <> dtFinVigencia														");
                query.AppendLine("							And dtFinVigencia >= GETDATE()																");
                query.AppendLine("							And id = @idDisp																			");
                query.AppendLine("																														");
                query.AppendLine("							Set @rowCount = @@ROWCOUNT																	");
                query.AppendLine("																														");
                query.AppendLine("							if(@rowCount > 0)																			");
                query.AppendLine("							Begin																						");
                query.AppendLine("								Set @res = 'Registro modificado con exito. '											");
                query.AppendLine("							End 																						");
                query.AppendLine("							Else																						");
                query.AppendLine("							Begin																						");
                query.AppendLine("								Set @res = 'Ocurrio un error al modificar el registro. '								");
                query.AppendLine("							End 																						");
                query.AppendLine("						End																								");
                query.AppendLine("						Else																							");
                query.AppendLine("						Begin 																							");
                query.AppendLine("							Set @res = 'Se encontro otro registro con la misma SIMCard. '								");
                query.AppendLine("						End																								");
                query.AppendLine("					End 																								");
                query.AppendLine("					Else																								");
                query.AppendLine("					Begin 																								");
                query.AppendLine("						Set @res = 'Se encontro otro registro con el mismo IMEI. '										");
                query.AppendLine("					End																									");
                query.AppendLine("				End 																									");
                query.AppendLine("				Else																									");
                query.AppendLine("				Begin 																									");
                query.AppendLine("					Set @res = 'La linea ya se encuentra asignada a otro dispositivo. '									");
                query.AppendLine("				End																										");
                query.AppendLine("			End																											");
                query.AppendLine("			Else																										");
                query.AppendLine("			Begin 																										");
                query.AppendLine("				Set @res = 'Tipo de dispositivo no encontrado. '														");
                query.AppendLine("			End																											");
                query.AppendLine("		End 																											");
                query.AppendLine("		Else																											");
                query.AppendLine("		Begin 																											");
                query.AppendLine("			Set @res = 'Modelo no encontrado. '																			");
                query.AppendLine("		End																												");
                query.AppendLine("	End 																												");
                query.AppendLine("	Else																												");
                query.AppendLine("	Begin 																												");
                query.AppendLine("		Set @res = 'Marca no encontrada. '																				");
                query.AppendLine("	End																													");
                query.AppendLine("End 																													");
                query.AppendLine("Else																													");
                query.AppendLine("Begin 																												");
                query.AppendLine("	Set @res = 'Dispositivo no encontrado. '																			");
                query.AppendLine("End																													");
                query.AppendLine("");
                query.AppendLine("Select @res");
            }
            catch (Exception ex)
            {
                throw ex;
            }

            return query.ToString();
        }



        public void DeleteDispoditivo(int id)
        {
            string res = string.Empty;
            try
            {
                DispositivoModel objDisp = new DispositivoModel();
                objDisp.ID = id;

                DataTable dt = DSODataAccess.Execute(QueryDeleteDispositivo(objDisp, DSODataContext.Schema), DSODataContext.ConnectionString);
                res = dt.Rows[0][0].ToString();
            }
            catch (Exception ex)
            {
                res = "Ocurrio un error al dar de baja elregistro. ";
            }

            Type cstype = this.GetType();
            ClientScriptManager cs = Page.ClientScript;
            String cstext = "alert('" + res + "'); location.href = 'Dispositivos.aspx';";
            cs.RegisterStartupScript(cstype, "", cstext, true);
        }

        public string QueryDeleteDispositivo(DispositivoModel objDisp, string schema)
        {
            StringBuilder query = new StringBuilder();
            query.AppendLine("");

            try
            {
                query.AppendLine("Declare @rowCount int = 0												");
                query.AppendLine("Declare @res varchar(max) = ''										");
                query.AppendLine("																		");
                query.AppendLine("Declare @idDisp int = " + objDisp.ID + "			     				");
                query.AppendLine("																		");
                query.AppendLine("																		");
                query.AppendLine("Update  Disp");
                query.AppendLine("Set dtFinVigencia = dtIniVigencia, dtfecultact = GETDATE()            ");
                query.AppendLine("From [K5Afirme].WorkflowLineasDispositivoMovil Disp					");
                query.AppendLine("where  dtIniVigencia <> dtFinVigencia									");
                query.AppendLine("And dtFinVigencia >= GETDATE()										");
                query.AppendLine("And id = @idDisp														");
                query.AppendLine("																		");
                query.AppendLine("Set @rowCount = @@ROWCOUNT											");
                query.AppendLine("																		");
                query.AppendLine("if(@rowCount > 0)														");
                query.AppendLine("Begin																	");
                query.AppendLine("	set @res = 'Registro eliminado con exito. ';						");
                query.AppendLine("End 																	");
                query.AppendLine("Else																	");
                query.AppendLine("Begin																	");
                query.AppendLine("	set @res = 'Ocurrio un error al eliminar el registro. ';			");
                query.AppendLine("End																	");
                query.AppendLine("Select @res");
            }
            catch (Exception ex)
            {
                throw ex;
            }

            return query.ToString();
        }


    }

    public class DispositivoModel
    {
        public int ID { set; get; }
        public int MarcaDispositivoMovilID { set; get; }
        public string MarcaClave { set; get; }
        public string MarcaDesc { set; get; }
        public int ModeloDispositivoMovilID { set; get; }
        public string ModeloClave { set; get; }
        public string ModeloDesc { set; get; }
        public int TipoRecursoId { set; get; }
        public string TipoRecursoClave { set; get; }
        public string TipoRecursoDesc { set; get; }
        public string IMEI { set; get; }
        public string SIMCard { set; get; }
        public int LineaID { set; get; }
        public string NumeroTelefonico { set; get; }
        public string dtIniVigencia { set; get; }
        public string dtFinVigencia { set; get; }
        public string dtFecUltAct { set; get; }

    }

}