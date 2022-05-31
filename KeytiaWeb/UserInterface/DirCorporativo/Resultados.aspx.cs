using System;
using System.Collections;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Xml.Linq;
using DSOControls2008;
using KeytiaServiceBL;

namespace KeytiaWeb.UserInterface.DirCorporativo
{
    public partial class Resultados : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            #region Buscar el control de fecha que hereda de la Master y lo oculta ya que aquí no se usa.
            ((Panel)Form.FindControl("pnlRangeFechas")).Visible = false;
            #endregion

            string empleado = Request.QueryString["Empleado"].ToString();

            //string query = "select NominaA Nomina, NomCompleto,Nombre,Paterno,Materno, Email, Ubica Sitio, " +
            //            " TipoEmDesc [Tipo de Empleado], A.CenCosDesc Departamento, ProveedorDesc Empresa " +
            //            " from [vishistoricos('emple','empleados','español')] A " +
            //            " left join [vishistoricos('empleB','empleados B','español')] B" +
            //            " on A.iCodCatalogo = B.Emple " +
            //            " where A.dtIniVigencia<>A.dtFinVigencia and A.dtFinVigencia>= GETDATE()" +
            //            " and B.dtIniVigencia<>B.dtFinVigencia and B.dtFinVigencia>= GETDATE()" +
            //            " and A.iCodCatalogo = " + empleado;

            string query = "select NominaA Nomina, " +
                        " upper(ltrim(rtrim(isnull(Nombre,'')))+' '+ltrim(rtrim(isnull(SegundoNombre,'')))+' '+ltrim(rtrim(isnull(Paterno,'')))+' '+ltrim(rtrim(isnull(Materno,'')))) as NomCompleto," +
                        " upper(ltrim(rtrim(isnull(Nombre,'')))+' '+ltrim(rtrim(isnull(SegundoNombre,'')))) as Nombre,upper(Paterno) as Paterno,upper(Materno) as Materno, lower(Email) as email, upper(Ubica) Sitio, " +
                        " TipoEmDesc [Tipo de Empleado], A.CenCosDesc Departamento, ProveedorDesc Empresa " +
                        " from [vishistoricos('emple','empleados','español')] A " +
                        " left join [vishistoricos('empleB','empleados B','español')] B" +
                        " on A.iCodCatalogo = B.Emple " +
                        "   and B.dtIniVigencia<>B.dtFinVigencia " +
                        "   and B.dtFinVigencia>= GETDATE()" +
                        "   where A.dtIniVigencia<>A.dtFinVigencia " +
                        "   and A.dtFinVigencia>= GETDATE() " +
                        "   and  (ISNULL(banderasemple,0) & 1)/1<>0 " +
                        "   and A.iCodCatalogo = " + empleado;

            try
            {

                DataRow detalle = KeytiaServiceBL.DSODataAccess.ExecuteDataRow(query);

                if (detalle != null)
                {
                    txtDetallNombre.Text = detalle["Nombre"].ToString();
                    txtDetallPaterno.Text = detalle["Paterno"].ToString();
                    txtDetallMaterno.Text = detalle["Materno"].ToString();
                    txtDetallEmail.Text = detalle["Email"].ToString();
                    txtDetallTipoEmple.Text = detalle["Tipo de Empleado"].ToString();
                    txtDetallSitio.Text = detalle["Sitio"].ToString();
                    txtDetallDepto.Text = detalle["Departamento"].ToString();
                    txtDetallEmpresa.Text = detalle["Empresa"].ToString();
                }

                //string queryext = "select vchCodigo Extension, SitioDesc Sitio " +
                //                    " from [vishistoricos('exten','extensiones','español')] " +
                //                    "where dtIniVigencia <> dtFinVigencia and dtFinVigencia >= getdate()" +
                //                   " and Emple = " + empleado;

                string queryext = "select Exten.vchCodigo Extension, Exten.SitioDesc Sitio " +
                                    " from [vishistoricos('exten','extensiones','español')] Exten" +
                                    " join nextel.[VisHistoricos('Extenb','Extensiones b','español')] extenB " +
                                    " on extenB.dtinivigencia<>extenB.dtfinvigencia " +
                                    "   and extenB.dtfinvigencia>=getdate() " +
                                    "   and Exten.icodcatalogo = extenb.exten " +
                                    "   and  (ISNULL(banderasextens,0) & 1)/1<>0 " +
                                    " join ((select min(icodregistro) as icodRegistro " +
                                    "                     from catalogos " +
                                    "                     where vchcodigo = 'EXTENPRINC' " +
                                    "                     and icodcatalogo =( " +
                                    " 				                        select min(icodregistro) " +
                                    " 				                        from catalogos " +
                                    " 				                        where vchdescripcion like 'Tipo de recurso' " +
                                    " 				                        and icodcatalogo is null))) as TipoRec " +
                                    " on extenb.tiporecurso = TipoRec.icodRegistro " +
                                    " where Exten.dtIniVigencia <> Exten.dtFinVigencia " +
                                    " and Exten.dtFinVigencia >= getdate()" +
                                    " and Exten.Emple = " + empleado;

                DataTable extensiones = KeytiaServiceBL.DSODataAccess.Execute(queryext);

                for (int i = 0; i < extensiones.Rows.Count && i < 10; i++)
                {
                    Table2.Rows[i + 1].Visible = true;
                    Table2.Rows[i + 1].Cells[0].Text = extensiones.Rows[i][0].ToString();
                    Table2.Rows[i + 1].Cells[1].Text = extensiones.Rows[i][1].ToString();

                }
                string queryradio = "select numradio " +
                                    " from [VisHistoricos('CCustodia','Cartas custodia','Español')] " +
                                    "where dtIniVigencia <> dtFinVigencia and dtFinVigencia >= getdate()" +
                                   " and Emple = " + empleado;

                DataTable radio = KeytiaServiceBL.DSODataAccess.Execute(queryradio);

                for (int i = 0; i < radio.Rows.Count && i < 10; i++)
                {
                    Table2.Rows[i + 1].Visible = true;
                    Table2.Rows[i + 1].Cells[2].Text = radio.Rows[i][0].ToString();


                }



            }
            catch
            {
            }

        }

        protected void btnBuscarOtro_Click(object sender, EventArgs e)
        {
            Page.Response.Redirect("~/UserInterface/DirCorporativo/DirectorioCorporativo.aspx");
        }

    }
}
