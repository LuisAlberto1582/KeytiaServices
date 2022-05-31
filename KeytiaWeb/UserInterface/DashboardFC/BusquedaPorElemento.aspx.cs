using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Text;
using DSOControls2008;
using System.Data;
using KeytiaServiceBL;
using KeytiaWeb.UserInterface.DashboardLT;

namespace KeytiaWeb.UserInterface.DashboardFC
{
    public partial class BusquedaPorElemento : System.Web.UI.Page
    {
        StringBuilder query = new StringBuilder();
        int tipoElemento = int.MinValue;
        string busqueda = string.Empty;

        public void Page_PreInit(object o, EventArgs e)
        {
            EnsureChildControls();
            Page.ClientScript.GetPostBackEventReference(this, "");
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            try
            {
                #region Buscar el control de fecha que hereda de la Master y lo oculta ya que aquí no se usa.
                ((Panel)Form.FindControl("pnlRangeFechas")).Visible = false;
                #endregion

                //Controles de la pagina actual                
                Rep1.Visible = false;
                Rep2.Visible = false;                    

                if (!Page.IsPostBack)
                {
                    #region Inicia los valores default de los controles
                    try
                    {
                        LlenarDropDownList();
                    }
                    catch (Exception ex)
                    {
                        throw new KeytiaWebException(
                            "Ocurrio un error al darle valores default a los tipos de elementos para la busqueda.'" + Request.Path
                            + HttpContext.Current.Session["vchCodUsuario"] + "'", ex);
                    }
                    #endregion Inicia los valores default de los controles 
                }
            }
            catch (Exception ex)
            {
                throw new KeytiaWebException("Ocurrio un error en " + Request.Path
                      + HttpContext.Current.Session["vchCodUsuario"] + "'", ex);
            }
        }

        private void LlenarDropDownList()
        {
            #region DropDownList Tipo Elemento
            DataTable dtTipoElemento = new DataTable();
            dtTipoElemento.Columns.Add("Id", typeof(Int32));
            dtTipoElemento.Columns.Add("Elemento", typeof(string));
            
            DataRow rowEmple = dtTipoElemento.NewRow();
            rowEmple["Id"] = 0;
            rowEmple["Elemento"] = "Empleado";
            dtTipoElemento.Rows.InsertAt(rowEmple, 0);

            DataRow rowCenCos = dtTipoElemento.NewRow();
            rowCenCos["Id"] = 1;
            rowCenCos["Elemento"] = "Centro de Costos";
            dtTipoElemento.Rows.InsertAt(rowCenCos, 1);

            DataRow rowSitio = dtTipoElemento.NewRow();
            rowSitio["Id"] = 2;
            rowSitio["Elemento"] = "Sitio";
            dtTipoElemento.Rows.InsertAt(rowSitio, 2);

            cboTipoElemento.DataSource = dtTipoElemento;
            cboTipoElemento.DataValueField = "Id";
            cboTipoElemento.DataTextField = "Elemento";
            cboTipoElemento.DataBind();

            #endregion DropDownList Tipo Elemento
        }

        protected void btnAceptar_Click(object sender, EventArgs e)
        {
            Busqueda();
        }

        private void Busqueda() 
        {
            tipoElemento = Convert.ToInt32(cboTipoElemento.SelectedItem.Value);
            busqueda = txtTexto.Text.Trim().Replace("'", "");

            if (!string.IsNullOrEmpty(busqueda))
            {
                string titulo = "Resultado de la búsqueda";
                switch (tipoElemento)
                {
                    case 0:
                        BusquedaPorEmpleado("/UserInterface/DashboardFC/Dashboard.aspx?Nav=EmpleMCN2&Emple=", titulo);
                        break;
                    case 1:
                        BusquedaPorCenCos("/UserInterface/DashboardFC/Dashboard.aspx?Nav=CenCosN2&CenCos=", titulo);
                        break;
                    case 2:
                        BusquedaPorSitio("/UserInterface/DashboardFC/Dashboard.aspx?Nav=SitioN2&Sitio=", titulo);
                        break;
                    default:
                        break;
                }
            }
            else 
            {
                lblTituloModalMsn.Text = "Sin texto de búsqueda";
                lblBodyModalMsn.Text = "Favor de proporcionar un texto de búsqueda.";
                mpeEtqMsn.Show();                
            }
        }     

        #region Resultados

        private void BusquedaPorEmpleado(string link, string titulo) 
        {
            DataTable dtResult = DSODataAccess.Execute(BuscarElemento(link + "'' + CONVERT(VARCHAR, Emple.iCodCatalogo)'", "BusquedaPorEmpleado"));
            if (dtResult.Rows.Count > 0)
            {
                DataView dvldt = new DataView(dtResult);
                dtResult = dvldt.ToTable(false,
                    new string[] { "Codigo Empleado", "Nomina", "Nombre Completo", "Puesto", "Clave CenCos", "Centro de Costos", "link" });

                dtResult.Columns["Nomina"].ColumnName = "Nómina";
                Rep2.Controls.Add(
                    DTIChartsAndControls.tituloYBordesReporte(
                                    DTIChartsAndControls.GridView("BusquedaPorEmpleado", dtResult, false, "",
                                    new string[] { "", "", "", "", "", "", "" }, Request.ApplicationPath + "{0}",
                                    new string[] { "link" }, 0, new int[] { 0, 4, 6 }, new int[] { 2, 3, 5 }, new int[] { 1 }), titulo, 0));

                Rep2.Visible = true;
            }
            else 
            {
                Rep1.Visible = true;
            }
        }

        private void BusquedaPorCenCos(string link, string titulo) 
        {
            DataTable dtResult = DSODataAccess.Execute(BuscarElemento(link + "'' + CONVERT(VARCHAR, CenCos.iCodCatalogo)'", "BusquedaPorCenCos"));
            if (dtResult.Rows.Count > 0)
            {
                DataView dvldt = new DataView(dtResult);
                dtResult = dvldt.ToTable(false, new string[] { "Codigo CenCos", "Clave CenCos", "Centro de Costos", "Responsable", "link" });

                dtResult.Columns["Clave CenCos"].ColumnName = "Clave";
                Rep2.Controls.Add(
                    DTIChartsAndControls.tituloYBordesReporte(
                                    DTIChartsAndControls.GridView("BusquedaPorCenCos", dtResult, false, "", 
                                    new string[] { "", "", "", "", "" }, Request.ApplicationPath + "{0}",
                                    new string[] { "link" }, 0, new int[] { 0, 4 }, new int[] { 2, 3 }, new int[] { 1 }), titulo, 0));

                Rep2.Visible = true;
            }
            else
            {
                Rep1.Visible = true;
            }
        }

        private void BusquedaPorSitio(string link, string titulo) 
        {
            DataTable dtResult = DSODataAccess.Execute(BuscarElemento(link + "'' + CONVERT(VARCHAR, Sitio.iCodCatalogo)'", "BusquedaPorSitio"));
            if (dtResult.Rows.Count > 0)
            {
                DataView dvldt = new DataView(dtResult);
                dtResult = dvldt.ToTable(false, new string[] { "Codigo Sitio", "Clave Sitio", "Sitio", "link" });

                Rep2.Controls.Add(
                    DTIChartsAndControls.tituloYBordesReporte(
                                    DTIChartsAndControls.GridView("BusquedaPorSitio", dtResult, false, "", 
                                    new string[] { "", "", "", "" }, Request.ApplicationPath + "{0}",
                                    new string[] { "link" }, 0, new int[] { 0, 1, 3 }, new int[] { }, new int[] { 2 }), titulo, 0));

                Rep2.Visible = true;
            }
            else
            {
                Rep1.Visible = true;
            }
        }

        #endregion

        #region Consultas

        private string BuscarElemento(string link, string nombreSP)
        {
            query.Length = 0;
            query.AppendLine("EXEC " + nombreSP);
            query.AppendLine("  	@Schema = '" + DSODataContext.Schema + "',");
            query.AppendLine("      @Texto = '''%" + busqueda.Trim().Replace(" ", "%") + "%''',");
            if (!string.IsNullOrEmpty(link))
            {
                query.AppendLine("      @Link = '''" + link + ",");
            }
            else 
            {  
                query.AppendLine("      @Link = '''''',");
            }
            query.AppendLine("      @Usuario = " + Session["iCodUsuario"].ToString() + ",");
            query.AppendLine("      @Perfil = " + Session["iCodPerfil"].ToString());

            return query.ToString();
        }       

        #endregion

    }
}
