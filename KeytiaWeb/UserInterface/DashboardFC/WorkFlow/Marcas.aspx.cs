using KeytiaServiceBL;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace KeytiaWeb.UserInterface.DashboardFC.WorkFlow
{
    public partial class Marcas : System.Web.UI.Page
    {
        #region Eventos
        protected void Page_Load(object sender, EventArgs e)
        {
            CargarGridMarcas();

            int id;
            Int32.TryParse(String.IsNullOrEmpty(Request.QueryString["Id"]) ? null : Request.QueryString["Id"].ToString(), out id);
            if (id > 0)
            {
                DeleteMarca(id);
            }
        }

        protected void gridMarcas_RowCommand(object sender, GridViewCommandEventArgs e)
        {
            string commandName = e.CommandName.ToString().ToLower();
            GridViewRow gvr = (GridViewRow)(((LinkButton)e.CommandSource).NamingContainer);
            int RowIndex = gvr.RowIndex;


            Marca objMarca = new Marca();
            objMarca.id = Convert.ToInt32(gridMarcas.DataKeys[RowIndex].Values["ID"].ToString());
            objMarca.Nombre = gridMarcas.DataKeys[RowIndex].Values["Clave"].ToString();
            objMarca.Descripcion = gridMarcas.DataKeys[RowIndex].Values["Descripcion"].ToString();

            if (commandName.ToLower() == "editar")
            {
                if (CargarModal(objMarca))
                {
                    mpeModal.Show();
                }
            }
            else if (commandName.ToLower() == "eliminar")
            {

                Type cstype = this.GetType();
                ClientScriptManager cs = Page.ClientScript;
                String cstext = "";
                cstext = "if(confirm('¿Está seguro que desea eliminar el registro?')){ location.assign('Marcas.aspx?Id=" + objMarca.id.ToString() + "'); }";

                cs.RegisterStartupScript(cstype, "", cstext, true);
            }
        }

        protected void ImgInsertaMarca_Click(object sender, EventArgs e)
        {
            InsertaMarca();
        }
        #endregion

        #region Métodos
        public bool CargarModal(Marca objMarca)
        {
            bool res = false;

            try
            {
                txtIdModal.Text = objMarca.id.ToString();
                txtNombreModal.Text = objMarca.Nombre;
                txtdescripcionModal.Text = objMarca.Descripcion;

                res = true;

            }
            catch (Exception ex)
            {
            }

            return res;
        }

        public void CargarGridMarcas()
        {
            try
            {
                gridMarcas.DataSource = BuscaMarcas();
                gridMarcas.DataBind();
            }
            catch (Exception ex)
            {
            }
        }

        public DataTable BuscaMarcas()
        {
            DataTable dt = new DataTable();
            try
            {


                string esquema = DSODataContext.Schema;
                dt = DSODataAccess.Execute(CrearQuerySelect(esquema), DSODataContext.ConnectionString);


            }
            catch (Exception ex)
            {
            }
            return dt;
        }

        public string CrearQuerySelect(string esquema)
        {
            StringBuilder query = new StringBuilder();
            query.AppendLine("");
            try
            {
                query.AppendLine("Select");
                query.AppendLine("  ID,");
                query.AppendLine("  Clave,");
                query.AppendLine("  Descripcion,");
                query.AppendLine("  dtIniVigencia = convert(varchar, dtIniVigencia, 11),");
                query.AppendLine("  dtFinVigencia = convert(varchar, dtFinVigencia, 11),");
                query.AppendLine("  dtFecUltAct = convert(varchar, dtFecUltAct, 11)");
                query.AppendLine("From [" + esquema + "].WorkflowLineasMarcaDipositivoMovil ");
                query.AppendLine("Where dtinivigencia <> dtFinVigencia");
                query.AppendLine("And dtFinVigencia >= GETDATE()");
                query.AppendLine("And Len(Clave) > 0");
            }
            catch (Exception ex)
            {
            }
            return query.ToString();
        }

        public void InsertaMarca()
        {
            string res = string.Empty;

            try
            {
                Marca objMarca = new Marca();
                objMarca.Nombre = txtInsertNombre.Text.ToString();
                objMarca.Descripcion = txtInsertDescripcion.Text.ToString();

                res = InsertRegMarca(objMarca);


                Type cstype = this.GetType();
                ClientScriptManager cs = Page.ClientScript;
                String cstext = "alert('" + res + "');  location.href = 'Marcas.aspx';";
                cs.RegisterStartupScript(cstype, "", cstext, true);
            }
            catch (Exception ex)
            {
                res = "Ocurrio un error al dar de alta el registro. ";
            }
        }

        public string InsertRegMarca(Marca objMarca)
        {
            string res = string.Empty;
            try
            {

                DataTable dt = DSODataAccess.Execute(CrearQueryInsert(objMarca, DSODataContext.Schema), DSODataContext.ConnectionString);
                res = dt.Rows[0][0].ToString();
            }
            catch (Exception ex)
            {
                res = "Ocurrio un error a insertar  registro. ";
            }

            return res;
        }

        public string CrearQueryInsert(Marca objMarca, string esquema)
        {
            string res = string.Empty;
            StringBuilder query = new StringBuilder();

            query.AppendLine("");
            try
            {
                query.AppendLine("Declare @res varchar(max) =  ''										");
                query.AppendLine("Declare @rowCount int =0                                              ");
                query.AppendLine("Declare @nombreInsert varchar(100) = '" + objMarca.Nombre + "'        ");
                query.AppendLine("Declare @descInsert varchar(300) = '" + objMarca.Descripcion + "'         ");
                query.AppendLine("                                                                      ");
                query.AppendLine("if                                                                    ");
                query.AppendLine("(                                                                     ");
                query.AppendLine("	(                                                                   ");
                query.AppendLine("		Select count(*)                                                 ");
                query.AppendLine("		From [" + esquema + "].WorkflowLineasMarcaDipositivoMovil           ");
                query.AppendLine("		Where Clave =  @nombreInsert                                    ");
                query.AppendLine("	) =0                                                                ");
                query.AppendLine(")                                                                     ");
                query.AppendLine("Begin                                                                 ");
                query.AppendLine("	Insert into [" + esquema + "].WorkflowLineasMarcaDipositivoMovil    ");
                query.AppendLine("	(                                                                   ");
                query.AppendLine("		Clave,                                                          ");
                query.AppendLine("		Descripcion	,                                                   ");
                query.AppendLine("		dtIniVigencia,                                                  ");
                query.AppendLine("		dtFinVigencia,                                                  ");
                query.AppendLine("		dtFecUltAct                                                     ");
                query.AppendLine("	)                                                                   ");
                query.AppendLine("	values                                                              ");
                query.AppendLine("	(                                                                   ");
                query.AppendLine("		@nombreInsert,                                                  ");
                query.AppendLine("		@descInsert,                                                    ");
                query.AppendLine("		'2011-01-01 00:00:00',                                          ");
                query.AppendLine("		'2079-01-01 00:00:00',                                          ");
                query.AppendLine("		GETDATE()                                                       ");
                query.AppendLine("	)                                                                   ");
                query.AppendLine("                                                                      ");
                query.AppendLine("	Set @rowCount = @@rowcount                                          ");
                query.AppendLine("                                                                      ");
                query.AppendLine("	if(@rowCount > 0)                                                   ");
                query.AppendLine("	Begin                                                               ");
                query.AppendLine("		set @res = 'El registro fue insertado correctamente. '          ");
                query.AppendLine("	End                                                                 ");
                query.AppendLine("	Else                                                                ");
                query.AppendLine("	Begin                                                               ");
                query.AppendLine("		set @res = 'Ocurrio un error al insertar el registro. '         ");
                query.AppendLine("	End		                                                            ");
                query.AppendLine("                                                                      ");
                query.AppendLine("End                                                                   ");
                query.AppendLine("Else                                                                  ");
                query.AppendLine("Begin                                                                 ");
                query.AppendLine("	Set  @res = 'Ya existe un registro vigente con el mismo nombre, favor de elegir uno distinto. '  ");
                query.AppendLine("End 					");
                query.AppendLine("                      ");
                query.AppendLine("                      ");
                query.AppendLine("Select @res           ");
            }
            catch (Exception ex)
            {
                res = "Ocurrio un error crear query. ";
            }

            return query.ToString();
        }

        public void UpdateMarca(Marca objMarca)
        {
            string res = string.Empty;
            try
            {
                if (objMarca.id > 0)
                {
                    DataTable dt = DSODataAccess.Execute(QueryUpdate(objMarca,DSODataContext.Schema), DSODataContext.ConnectionString);
                    res = dt.Rows[0][0].ToString();                   
                }

            }
            catch (Exception ex)
            {
                res = "Ocurrio un error al modificar el registro. ";
            }

            Type cstype = this.GetType();
            ClientScriptManager cs = Page.ClientScript;
            String cstext = "alert('" + res + "');  location.href = 'Marcas.aspx';";
            cs.RegisterStartupScript(cstype, "", cstext, true);
        }

        public string QueryUpdate(Marca objMarca, string schema)
        {
            StringBuilder query = new StringBuilder();
            query.AppendLine("");
            try
            {
                query.AppendLine("Declare @res varchar(max) = ''															");
                query.AppendLine("Declare @rowCount int = 0																	");
                query.AppendLine("Declare @claveUpdate  varchar(100) = '"+objMarca.Nombre+"'            					");
                query.AppendLine("Declare @descUpdate varchar(300) = '"+objMarca.Descripcion+"'	                 			");
                query.AppendLine("Declare @idUpdate int = "+objMarca.id+"													");
                query.AppendLine("																							");
                query.AppendLine("if																						");
                query.AppendLine("(																							");
                query.AppendLine("	(																						");
                query.AppendLine("		Select count(*)      																");
                query.AppendLine("		From ["+schema+"].WorkflowLineasMarcaDipositivoMovil     							");
                query.AppendLine("		Where dtinivigencia <> dtfinvigencia 												");
                query.AppendLine("		And dtfinvigencia >= GETDATE()														");
                query.AppendLine("		And clave = @claveUpdate															");
                query.AppendLine("	) = 0																					");
                query.AppendLine(")																							");
                query.AppendLine("Begin																						");
                query.AppendLine("																							");
                query.AppendLine("	Update Marcas																			");
                query.AppendLine("	Set 																					");
                query.AppendLine("		Clave = @claveUpdate,																");
                query.AppendLine("		Descripcion = @descUpdate,															");
                query.AppendLine("		dtFecUltAct = GETDATE()																");
                query.AppendLine("	From [" + schema + "].WorkflowLineasMarcaDipositivoMovil Marcas		    				");
                query.AppendLine("	Where id = @idUpdate																	");
                query.AppendLine("	Set @rowCount = @@ROWCOUNT																");
                query.AppendLine("																							");
                query.AppendLine("	if(@rowCount > 0)																		");
                query.AppendLine("	Begin																					");
                query.AppendLine("		Set @res = 'Registro modificado con exito. '										");
                query.AppendLine("	End 																					");
                query.AppendLine("	Else																					");
                query.AppendLine("	Begin																					");
                query.AppendLine("		Set @res = 'Ocurrio un error al modificar el registro. '							");
                query.AppendLine("	End 																					");
                query.AppendLine("End 																						");
                query.AppendLine("Else																						");
                query.AppendLine("Begin																						");
                query.AppendLine("	Set @res = 'Ya existe un elemento con el mismo nombre, favor de teclear otro. '			");
                query.AppendLine("End																						");
                query.AppendLine("Select @res   ");                
            }
            catch (Exception ex)
            {
            }
            return query.ToString();
        }

        public  void DeleteMarca(int id)
        {
            try
            {
                Marca objMarca = new Marca();
                objMarca.id = id;

                string res = string.Empty;
                if (id > 0)
                {
                    DataTable dt = DSODataAccess.Execute(QueryDelete(objMarca, DSODataContext.Schema), DSODataContext.ConnectionString);

                    res = dt.Rows[0][0].ToString();
                }
                else
                {
                    res = "Id incorrecto. ";
                }


                Type cstype = this.GetType();
                ClientScriptManager cs = Page.ClientScript;
                String cstext = "alert('" + res + "');  location.href = 'Marcas.aspx';";
                cs.RegisterStartupScript(cstype, "", cstext, true);
            }
            catch (Exception ex)
            {
            }
        }

        public string QueryDelete(Marca objMarca, string schema)
        {
            StringBuilder query = new StringBuilder();
            query.AppendLine("");


            try
            {
                query.AppendLine("Declare @res varchar(max) = ''															");
                query.AppendLine("Declare @rowCount int = 0																	");
                query.AppendLine("Declare @idDelete int = "+objMarca.id+"    												");
                query.AppendLine("																							");
                query.AppendLine("if																						");
                query.AppendLine("(																							");
                query.AppendLine("	(																						");
                query.AppendLine("		Select count(*)																		");
                query.AppendLine("		From ["+ schema + "].[WorkflowLineasMarcaDipositivoMovil]							");
                query.AppendLine("		Where id = @idDelete																");
                query.AppendLine("	) =1																					");
                query.AppendLine(")																							");
                query.AppendLine("Begin																						");
                query.AppendLine("																							");
                query.AppendLine("	Update Marca																			");
                query.AppendLine("		Set 																				");
                query.AppendLine("			dtFinVigencia = dtIniVigencia,													");
                query.AppendLine("			dtFecUltAct = GETDATE()															");
                query.AppendLine("	From [" + schema + "].[WorkflowLineasMarcaDipositivoMovil] Marca						");
                query.AppendLine("	Where id = @idDelete																	");
                query.AppendLine("																							");
                query.AppendLine("	Set @rowCount = @@ROWCOUNT																");
                query.AppendLine("																							");
                query.AppendLine("	if(@rowCount > 0)																		");
                query.AppendLine("	Begin																					");
                query.AppendLine("		Set @res = 'Registro eliminado de la base de datos. '								");
                query.AppendLine("	End 																					");
                query.AppendLine("	Else																					");
                query.AppendLine("	Begin																					");
                query.AppendLine("		Set @res = 'Ocurrio un error al dar de baja el registro de la base de datos. '		");
                query.AppendLine("	End 																					");
                query.AppendLine("End 																						");
                query.AppendLine("Else																						");
                query.AppendLine("Begin																						");
                query.AppendLine("	Set @res = 'Ocurrio un error al dar de baja el registro de la base de datos. '			");
                query.AppendLine("End 																						");
                query.AppendLine("																							");
                query.AppendLine("Select  @res																				");
            }
            catch (Exception ex)
            {
            }

            return query.ToString();
        }

        #endregion

        protected void btnAceptar_Click(object sender, EventArgs e)
        {
            Marca objMarca = new Marca();
            objMarca.id = Convert.ToInt32(txtIdModal.Text.ToString());
            objMarca.Nombre = txtNombreModal.Text.ToString();
            objMarca.Descripcion = txtdescripcionModal.Text.ToString();

            UpdateMarca(objMarca);
        }
    }

    public class Marca
    {
        public int id { get; set; }
        public string Nombre { get; set; }
        public string Descripcion { get; set; }
        public string dtIniVigencia { get; set; }
        public string dtFinVigencia { get; set; }
        public string dtFecUltAct { get; set; }
    }
}