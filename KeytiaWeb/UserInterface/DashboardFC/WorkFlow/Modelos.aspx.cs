using KeytiaServiceBL;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Text;

namespace KeytiaWeb.UserInterface.DashboardFC.WorkFlow
{
    public partial class Modelos : System.Web.UI.Page
    {
        #region Eventos
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                CargarGridModelos();
                CargarDDLMarcas();
            }

            int id;
            Int32.TryParse(String.IsNullOrEmpty(Request.QueryString["Id"]) ? null : Request.QueryString["Id"].ToString(), out id);
            if (id > 0)
            {
                DeleteModelo(id);
            }
        }

        protected void ImgInsertaModelo_Click(object sender, EventArgs e)
        {
            try
            {
                ModeloDisp objModelo = new ModeloDisp();

                objModelo.MarcaID = Convert.ToInt32(ddlInsertMarca.SelectedValue.ToString());
                objModelo.MarcaClave = "";
                objModelo.MarcaDesc = "";
                objModelo.ModeloID = 0;
                objModelo.ModeloClave = txtInsertNombre.Text;
                objModelo.ModeloDesc = txtInsertDescripcion.Text;
                objModelo.ModeloIniVigencia = "2011-01-01 00:00:00";
                objModelo.ModeloFinVigencia = "2079-01-01 00:00:00";

                if (objModelo.MarcaID > 0 && objModelo.ModeloClave.Length > 0 && objModelo.ModeloDesc.Length > 0)
                {
                    InsertModelo(objModelo);
                }

            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        protected void gridModelos_RowCommand(object sender, GridViewCommandEventArgs e)
        {
            string commandName = e.CommandName.ToString().ToLower();
            GridViewRow gvr = (GridViewRow)(((LinkButton)e.CommandSource).NamingContainer);
            int RowIndex = gvr.RowIndex;

            ModeloDisp objModelo = new ModeloDisp(); ;
            objModelo.MarcaID = Convert.ToInt32(gridModelos.DataKeys[RowIndex].Values["MarcaID"].ToString());
            objModelo.MarcaClave = gridModelos.DataKeys[RowIndex].Values["MarcaClave"].ToString();
            objModelo.MarcaDesc = gridModelos.DataKeys[RowIndex].Values["MarcaDesc"].ToString();
            objModelo.ModeloID = Convert.ToInt32(gridModelos.DataKeys[RowIndex].Values["ModeloID"].ToString());
            objModelo.ModeloClave = gridModelos.DataKeys[RowIndex].Values["ModeloClave"].ToString();
            objModelo.ModeloDesc = gridModelos.DataKeys[RowIndex].Values["ModeloDesc"].ToString();
            objModelo.ModeloIniVigencia = gridModelos.DataKeys[RowIndex].Values["ModeloIniVigencia"].ToString();
            objModelo.ModeloFinVigencia = gridModelos.DataKeys[RowIndex].Values["ModeloFinVigencia"].ToString();

            if (commandName.ToLower() == "editar")
            {
                ShowModal(objModelo);
            }
            else if (commandName.ToLower() == "eliminar")
            {
                Type cstype = this.GetType();
                ClientScriptManager cs = Page.ClientScript;
                String cstext = "";
                cstext = "if(confirm('¿Está seguro que desea eliminar el registro?')){ location.assign('Modelos.aspx?Id=" + objModelo.ModeloID.ToString() + "'); }";

                cs.RegisterStartupScript(cstype, "", cstext, true);
            }
        }

        protected void btnAceptar_Click(object sender, EventArgs e)
        {
            try
            {
                ModeloDisp objModelo = new ModeloDisp();

                objModelo.MarcaID = Convert.ToInt32(ddlMarcaModal.SelectedValue.ToString());
                objModelo.MarcaClave = ddlMarcaModal.SelectedItem.Text;
                objModelo.MarcaDesc = ddlMarcaModal.SelectedItem.Text;
                objModelo.ModeloID = Convert.ToInt32(txtIDModeloModal.Text);
                objModelo.ModeloClave = txtNombreModeloModal.Text;
                objModelo.ModeloDesc = txtDescModal.Text;
                objModelo.ModeloIniVigencia = "2011-01-01 00:00:00";
                objModelo.ModeloFinVigencia = "2079-01-01 00:00:00";

                UpdateModelo(objModelo);
            }
            catch (Exception ex)
            {
                throw ex;
            }

        }
        #endregion

        #region Métodos
        public void CargarGridModelos()
        {
            try
            {
                List<ModeloDisp> listaModelos = new List<ModeloDisp>();
                listaModelos = BuscaModelosDisp();

                gridModelos.DataSource = listaModelos;
                gridModelos.DataBind();
            }
            catch (Exception ex)
            {

                throw ex;
            }
        }

        public List<ModeloDisp> BuscaModelosDisp()
        {
            try
            {
                List<ModeloDisp> listaModelos = new List<ModeloDisp>();

                DataTable dt = DSODataAccess.Execute(QuerySelectModelos(DSODataContext.Schema), DSODataContext.ConnectionString);


                if (dt != null && dt.Rows.Count > 0 && dt.Columns.Count > 0)
                {
                    foreach (DataRow row in dt.Rows)
                    {
                        ModeloDisp objModelo = new ModeloDisp();
                        objModelo.MarcaID = Convert.ToInt32(row["MarcaID"].ToString());
                        objModelo.MarcaClave = row["MarcaClave"].ToString();
                        objModelo.MarcaDesc = row["MarcaDesc"].ToString();
                        objModelo.ModeloID = Convert.ToInt32(row["ModeloID"].ToString());
                        objModelo.ModeloClave = row["ModeloClave"].ToString();
                        objModelo.ModeloDesc = row["ModeloDesc"].ToString();
                        objModelo.ModeloIniVigencia = row["ModeloIniVigencia"].ToString();
                        objModelo.ModeloFinVigencia = row["ModeloFinVigencia"].ToString();

                        listaModelos.Add(objModelo);
                    }
                }


                return listaModelos;
            }
            catch (Exception ex)
            {

                throw;
            }
        }

        public List<Marcadisp> BuscaMarcasDisp()
        {
            try
            {
                List<Marcadisp> listaMarcas = new List<Marcadisp>();
                DataTable dt = DSODataAccess.Execute(QuerySelectMarcas(DSODataContext.Schema), DSODataContext.ConnectionString);

                if (dt != null && dt.Rows.Count > 0 && dt.Columns.Count > 0)
                {
                    foreach (DataRow row in dt.Rows)
                    {
                        Marcadisp objMarca = new Marcadisp();
                        objMarca.ID = Convert.ToInt32(row["ID"].ToString());
                        objMarca.Clave = row["Clave"].ToString();
                        objMarca.Descripcion = row["Descripcion"].ToString();
                        objMarca.dtIniVigencia = row["dtIniVigencia"].ToString();
                        objMarca.dtFinVigencia = row["dtFinVigencia"].ToString();

                        listaMarcas.Add(objMarca);
                    }
                }

                return listaMarcas;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public string QuerySelectModelos(string schema)
        {
            try
            {
                StringBuilder query = new StringBuilder();
                query.AppendLine("--WorkFlowLineas Select Modelos ");

                query.AppendLine("Select 															    ");
                query.AppendLine("	MarcaID				= marca.id,									    ");
                query.AppendLine("	MarcaClave			= marca.Clave,								    ");
                query.AppendLine("	MarcaDesc			= marca.Descripcion,						    ");
                query.AppendLine("	ModeloID			= modelo.id,								    ");
                query.AppendLine("	ModeloClave			= modelo.Clave,								    ");
                query.AppendLine("	ModeloDesc			= modelo.Descripcion,						    ");
                query.AppendLine("	ModeloIniVigencia	= Convert(varchar,modelo.dtIniVigencia,11),     ");
                query.AppendLine("	ModeloFinVigencia	= Convert(varchar,modelo.dtFinVigencia,11)      ");
                query.AppendLine("From [" + schema + "].WorkflowLineasModeloDispositivoMovil modelo			");
                query.AppendLine("inner Join [" + schema + "].WorkflowLineasMarcaDipositivoMovil marca	");
                query.AppendLine("	On modelo.MarcaDispositivoMovilID = marca.ID					    ");
                query.AppendLine("	And marca.dtIniVigencia <> marca.dtFinVigencia					    ");
                query.AppendLine("	And marca.dtFinVigencia >= GETDATE()							    ");
                query.AppendLine("where modelo.dtIniVigencia <> modelo.dtFinVigencia				    ");
                query.AppendLine("And modelo.dtFinVigencia >= GETDATE()								    ");
                query.AppendLine("order by Marca.Clave,modelo.Clave									    ");


                return query.ToString();
            }
            catch (Exception ex)
            {

                throw ex;
            }
        }

        public string QuerySelectMarcas(string schema)
        {
            try
            {
                StringBuilder query = new StringBuilder();
                query.AppendLine("-- WorkFlowLieas Select Marcas                             ");
                query.AppendLine("Select 													 ");
                query.AppendLine("	ID,														 ");
                query.AppendLine("	Clave	,												 ");
                query.AppendLine("	Descripcion	,											 ");
                query.AppendLine("	dtIniVigencia,											 ");
                query.AppendLine("	dtFinVigencia											 ");
                query.AppendLine("From [" + schema + "].WorkflowLineasMarcaDipositivoMovil marca ");
                query.AppendLine("Where marca.dtIniVigencia <> marca.dtFinVigencia			 ");
                query.AppendLine("And marca.dtFinVigencia >= GETDATE()						 ");
                query.AppendLine("And isNull(marca.clave,'') <> ''							 ");


                return query.ToString();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public void CargarDDLMarcas()
        {
            try
            {
                List<Marcadisp> listaMarcas = new List<Marcadisp>();

                listaMarcas = BuscaMarcasDisp();
                ddlInsertMarca.DataSource = listaMarcas;
                ddlInsertMarca.DataBind();

                ddlMarcaModal.DataSource = listaMarcas;
                ddlMarcaModal.DataBind();
            }
            catch (Exception ex)
            {

                throw ex;
            }
        }

        public void InsertModelo(ModeloDisp objModelo)
        {
            string res = "";
            try
            {
                DataTable dt = DSODataAccess.Execute(QueryInsertModelo(DSODataContext.Schema, objModelo), DSODataContext.ConnectionString);

                if (dt != null && dt.Rows.Count > 0 && dt.Columns.Count > 0)
                {
                    res = dt.Rows[0][0].ToString();

                    if (res.Length > 0)
                    {
                        Type cstype = this.GetType();
                        ClientScriptManager cs = Page.ClientScript;
                        String cstext = "alert('" + res + "'); location.href = 'Modelos.aspx';";
                        cs.RegisterStartupScript(cstype, "", cstext, true);
                    }
                }

            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public string QueryInsertModelo(string schema, ModeloDisp objModelo)
        {
            try
            {
                StringBuilder query = new StringBuilder();
                query.AppendLine("");
                query.AppendLine("--Insert Modelo																													");
                query.AppendLine("declare @res varchar(max) =''																										");
                query.AppendLine("declare @rowcount int =0																											");
                query.AppendLine("declare @idMarca int = " + objModelo.MarcaID + "																      					");
                query.AppendLine("declare @nombreModelo varchar(max) = '" + objModelo.ModeloClave + "'											      					");
                query.AppendLine("declare @descricripcion varchar(max) = '" + objModelo.ModeloDesc + "'											      					");
                query.AppendLine("declare @fechaInicio varchar(max) = '" + objModelo.ModeloIniVigencia + "'										      					");
                query.AppendLine("declare @fechaFin varchar(max) = '" + objModelo.ModeloFinVigencia + "'										      					");
                query.AppendLine("																																	");
                query.AppendLine("if																																");
                query.AppendLine("(																																	");
                query.AppendLine("	(																																");
                query.AppendLine("		Select count(*)																												");
                query.AppendLine("		From [" + schema + "].WorkflowLineasMarcaDipositivoMovil marcas																	");
                query.AppendLine("		Where dtIniVigencia <> dtFinVigencia																						");
                query.AppendLine("		And id = @idMarca																											");
                query.AppendLine("	) =1																															");
                query.AppendLine(")																																	");
                query.AppendLine("Begin																																");
                query.AppendLine("	if																																");
                query.AppendLine("	(																																");
                query.AppendLine("		(																															");
                query.AppendLine("			Select count(*)																											");
                query.AppendLine("			From [" + schema + "].WorkflowLineasModeloDispositivoMovil																");
                query.AppendLine("			Where dtIniVigencia <> dtFinVigencia																					");
                query.AppendLine("			And dtFinVigencia >= GETDATE()																							");
                query.AppendLine("			And MarcaDispositivoMovilID = @idMarca 																					");
                query.AppendLine("			And Clave = @nombreModelo																								");
                query.AppendLine("		) = 0																														");
                query.AppendLine("	)																																");
                query.AppendLine("	Begin																															");
                query.AppendLine("		Insert into [" + schema + "].WorkflowLineasModeloDispositivoMovil															");
                query.AppendLine("		(																															");
                query.AppendLine("		Clave,																														");
                query.AppendLine("		Descripcion,																												");
                query.AppendLine("		MarcaDispositivoMovilID,																									");
                query.AppendLine("		dtIniVigencia,																												");
                query.AppendLine("		dtFinVigencia,																												");
                query.AppendLine("		dtFecUltAct																													");
                query.AppendLine("		)																															");
                query.AppendLine("		values																														");
                query.AppendLine("		(																															");
                query.AppendLine("			@nombreModelo,																											");
                query.AppendLine("			@descricripcion,																										");
                query.AppendLine("			@idMarca,																												");
                query.AppendLine("			@fechaInicio,																											");
                query.AppendLine("			@fechaFin,																												");
                query.AppendLine("			GETDATE()																												");
                query.AppendLine("		)																															");
                query.AppendLine("																																	");
                query.AppendLine("		set @rowcount = @@ROWCOUNT																									");
                query.AppendLine("																																	");
                query.AppendLine("		if(@rowcount > 0)																											");
                query.AppendLine("		Begin																														");
                query.AppendLine("			set @res = 'El modelo se dio de alta correctamente. '																	");
                query.AppendLine("		End 																														");
                query.AppendLine("		Else																														");
                query.AppendLine("		Begin																														");
                query.AppendLine("			set @res = 'Ocurrio un error al dar de alta el modelo. '																");
                query.AppendLine("		End 																														");
                query.AppendLine("																																	");
                query.AppendLine("	End 																															");
                query.AppendLine("	Else																															");
                query.AppendLine("	Begin																															");
                query.AppendLine("		Set @res = 'Ya existe un momdelo con el mismo nombre para esa marca, el modelo no se pudo dar de alta. '					");
                query.AppendLine("	End 																															");
                query.AppendLine("																																	");
                query.AppendLine("End 																																");
                query.AppendLine("Else																																");
                query.AppendLine("Begin																																");
                query.AppendLine("Set @res = 'No se encontro la marca, el modelo no se pudo dar de alta. '															");
                query.AppendLine("End 																																");
                query.AppendLine("																																	");
                query.AppendLine("Select @res																														");

                return query.ToString();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public void UpdateModelo(ModeloDisp objModelo)
        {
            try
            {
                string res = "";
                DataTable dt = DSODataAccess.Execute(QueryUpdateModelo(objModelo, DSODataContext.Schema), DSODataContext.ConnectionString);
                if (dt != null && dt.Rows.Count > 0 && dt.Columns.Count > 0)
                {
                    res = dt.Rows[0][0].ToString();

                    if (res.Length > 0)
                    {
                        Type cstype = this.GetType();
                        ClientScriptManager cs = Page.ClientScript;
                        String cstext = "alert('" + res + "'); location.href = 'Modelos.aspx';";
                        cs.RegisterStartupScript(cstype, "", cstext, true);
                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public string QueryUpdateModelo(ModeloDisp objModelo, string schema)
        {
            try
            {
                StringBuilder query = new StringBuilder();

                query.AppendLine("--Update Modelo																					");
                query.AppendLine("declare @res varchar(max) =''																		");
                query.AppendLine("declare @rowcount int =0																			");
                query.AppendLine("declare @idMarca int = " + objModelo.MarcaID + "   													");
                query.AppendLine("declare @idModelo int =" + objModelo.ModeloID + "  													");
                query.AppendLine("declare @nombreModelo varchar(max) = '" + objModelo.ModeloClave + "'			    					");
                query.AppendLine("declare @descricripcionModelo varchar(max) = '" + objModelo.ModeloDesc + "'   						");
                query.AppendLine("declare @fechaInicioModelo varchar(max) = '" + objModelo.ModeloIniVigencia + "'    					");
                query.AppendLine("declare @fechaFinModelo varchar(max) = '" + objModelo.ModeloFinVigencia + "'   						");
                query.AppendLine("																									");
                query.AppendLine("if																								");
                query.AppendLine("(																									");
                query.AppendLine("	(																								");
                query.AppendLine("		Select count(*)																				");
                query.AppendLine("		From [" + schema + "].WorkflowLineasMarcaDipositivoMovil marcas	    						");
                query.AppendLine("		where dtIniVigencia <> dtFinVigencia														");
                query.AppendLine("		And dtFinVigencia  >= GETDATE()																");
                query.AppendLine("		And ID = @idMarca																			");
                query.AppendLine("	) = 1																							");
                query.AppendLine(")																									");
                query.AppendLine("Begin																								");
                query.AppendLine("	if																								");
                query.AppendLine("	(																								");
                query.AppendLine("		(																							");
                query.AppendLine("			Select COUNT(*)																			");
                query.AppendLine("			From [" + schema + "].WorkflowLineasModeloDispositivoMovil modelos	    				");
                query.AppendLine("			Where dtIniVigencia  <> dtFinVigencia													");
                query.AppendLine("			And dtFinVigencia <= GETDATE()															");
                query.AppendLine("			And MarcaDispositivoMovilID = @idMarca													");
                query.AppendLine("			And Clave = @nombreModelo																");
                query.AppendLine("		)=0																							");
                query.AppendLine("	)																								");
                query.AppendLine("	Begin																							");
                query.AppendLine("	Update modelos																					");
                query.AppendLine("	Set 																							");
                query.AppendLine("		MarcaDispositivoMovilID = @idMarca,															");
                query.AppendLine("		Clave = @nombreModelo,																		");
                query.AppendLine("		Descripcion = @descricripcionModelo															");
                query.AppendLine("	From [" + schema + "].WorkflowLineasModeloDispositivoMovil modelos		    					");
                query.AppendLine("	Where dtIniVigencia <> dtFinVigencia															");
                query.AppendLine("	And dtFinVigencia >= GETDATE()																	");
                query.AppendLine("	And id = @idModelo																				");
                query.AppendLine("																									");
                query.AppendLine("	set @rowcount = @@ROWCOUNT																		");
                query.AppendLine("	if(@rowcount > 0)																				");
                query.AppendLine("	Begin																							");
                query.AppendLine("		set @res = 'Registro modificado exitosamente. '												");
                query.AppendLine("	End 																							");
                query.AppendLine("	Else																							");
                query.AppendLine("	Begin																							");
                query.AppendLine("		set @res = 'Ha ocurrido un error al tratar de modificar el registro. '						");
                query.AppendLine("	End 																							");
                query.AppendLine("																									");
                query.AppendLine("	End 																							");
                query.AppendLine("	Else																							");
                query.AppendLine("	Begin																							");
                query.AppendLine("		set @res = 'Ya se encuentra un registro activo con el mismo nombre para esa marca. '		");
                query.AppendLine("	End																								");
                query.AppendLine("End 																								");
                query.AppendLine("Else																								");
                query.AppendLine("Begin																								");
                query.AppendLine("	set @res = 'La marca selecionada no se encuentra activa. '										");
                query.AppendLine("End																								");
                query.AppendLine("");
                query.AppendLine("select @res");
                return query.ToString();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public void DeleteModelo(int id)
        {
            try
            {
                string res = "";
                DataTable dt = DSODataAccess.Execute(QueryDeleteModelo(id, DSODataContext.Schema), DSODataContext.ConnectionString);
                if (dt != null && dt.Rows.Count > 0 && dt.Columns.Count > 0)
                {
                    res = dt.Rows[0][0].ToString();

                    if (res.Length > 0)
                    {
                        Type cstype = this.GetType();
                        ClientScriptManager cs = Page.ClientScript;
                        String cstext = "alert('" + res + "'); location.href = 'Modelos.aspx';";
                        cs.RegisterStartupScript(cstype, "", cstext, true);
                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public string QueryDeleteModelo(int id, string schema)
        {
            try
            {
                StringBuilder query = new StringBuilder();
                query.AppendLine("");
                query.AppendLine("--Delete Modelo																");
                query.AppendLine("declare @rowcount int =0														");
                query.AppendLine("declare @res varchar(max) =''													");
                query.AppendLine("declare @idModelo int =" + id + "													");
                query.AppendLine("																				");
                query.AppendLine("																				");
                query.AppendLine("Update modelos																");
                query.AppendLine("	Set 																		");
                query.AppendLine("		dtFinVigencia = dtIniVigencia,											");
                query.AppendLine("		dtFecUltAct = GETDATE()													");
                query.AppendLine("From [" + schema + "].WorkflowLineasModeloDispositivoMovil modelos				");
                query.AppendLine("Where dtIniVigencia <> dtFinVigencia											");
                query.AppendLine("And dtFinVigencia >= GETDATE()												");
                query.AppendLine("And ID = @idModelo															");
                query.AppendLine("																				");
                query.AppendLine("Set @rowcount = @@ROWCOUNT													");
                query.AppendLine("																				");
                query.AppendLine("if(@rowcount > 0)																");
                query.AppendLine("Begin																			");
                query.AppendLine("set @res = 'El registro se dio de baja exitosamente. '						");
                query.AppendLine("End 																			");
                query.AppendLine("Else																			");
                query.AppendLine("Begin																			");
                query.AppendLine("	set @res = 'Ha ocurrido un error al tratar de dar de baja el registro. '	");
                query.AppendLine("End 																			");
                query.AppendLine("																				");
                query.AppendLine("select @res																	");


                return query.ToString();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public void ShowModal(ModeloDisp objModelo)
        {
            try
            {
                if (CargarModal(objModelo))
                {
                    mpeModal.Show();
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public bool CargarModal(ModeloDisp objModelo)
        {
            try
            {
                bool res = false;

                txtIDModeloModal.Text = objModelo.ModeloID.ToString();
                txtIDMarcaModal.Text = objModelo.MarcaID.ToString();
                ddlMarcaModal.SelectedValue = objModelo.MarcaID.ToString();
                txtNombreModeloModal.Text = objModelo.ModeloClave.ToString();
                txtDescModal.Text = objModelo.ModeloDesc;

                res = true;
                return res;
            }
            catch (Exception ex)
            {

                throw ex;
            }
        }
        #endregion


    }

    public class ModeloDisp
    {
        public int MarcaID { set; get; }
        public string MarcaClave { set; get; }
        public string MarcaDesc { set; get; }
        public int ModeloID { set; get; }
        public string ModeloClave { set; get; }
        public string ModeloDesc { set; get; }
        public string ModeloIniVigencia { set; get; }
        public string ModeloFinVigencia { set; get; }
    }

    public class Marcadisp
    {
        public int ID { set; get; }
        public string Clave { set; get; }
        public string Descripcion { set; get; }
        public string dtIniVigencia { set; get; }
        public string dtFinVigencia { set; get; }
    }
}