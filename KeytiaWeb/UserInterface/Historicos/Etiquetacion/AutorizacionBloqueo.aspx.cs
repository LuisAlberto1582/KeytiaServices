using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Data;
using System.Text;
using KeytiaServiceBL;

namespace KeytiaWeb.UserInterface
{
    public partial class AutorizacionBloqueo : System.Web.UI.Page
    {


        private StringBuilder query = new StringBuilder();
        private int gBloqueo = 0;
        private int tiposBloqueo = 0;
        private string esquema = "";


        protected void Page_Load(object sender, EventArgs e)
        {
            LeerQueryString();
            LeerEsquema();

            if (!IsPostBack)
            {
                if (gBloqueo > 0 && tiposBloqueo > 0 && esquema != null && esquema != "" || 1 == 1)
                {
                    CargarDatosGridViewsEmpleados(GetEmpleBitacora());
                }
            }


        }


        public void LeerEsquema()
        {
            if (DSODataContext.Schema != "" && DSODataContext.Schema != null)
            {
                esquema = DSODataContext.Schema;
            }
        }


        public DataTable GetEmpleBitacora()
        {
            DataTable dtEmples = new DataTable();

            dtEmples = DSODataAccess.Execute(ConsultaGetEmpleBitacora());

            return dtEmples;
        }

        public void LeerQueryString()
        {

            if (Request.QueryString["gNumber"] != null && Request.QueryString["tNumber"] != null)
            {
                gBloqueo = Convert.ToInt32(Util.Decrypt(Request.QueryString["gNumber"].ToString()));
                tiposBloqueo = Convert.ToInt32(Util.Decrypt(Request.QueryString["tNumber"].ToString()));
            }

        }

        public void CargarDatosGridViewsEmpleados(DataTable dtEmples)
        {
            grdEmpleSinEtiq.DataSource = dtEmples;
            grdEmpleSinEtiq.DataBind();
        }

        public void GuardarPrevio_Click(object sender, EventArgs e)
        {
            ActualizaBEmpleAutorizadoBloqueo();

            if (gBloqueo > 0 && tiposBloqueo > 0 && esquema != null && esquema != "" || 1 == 1)
            {
                CargarDatosGridViewsEmpleados(GetEmpleBitacora());
            }
        }

        public void GuardarYEnviar_Click(object sender, EventArgs e)
        {
            ActualizaBEmpleAutorizadoBloqueo();
            ActualizarcBloquePendienteBloqueo();

            if (gBloqueo > 0 && tiposBloqueo > 0 && esquema != null && esquema != "" || 1 == 1)
            {
                CargarDatosGridViewsEmpleados(GetEmpleBitacora());
            }
        }


        public void ActualizaBEmpleAutorizadoBloqueo()
        {
            List<EtiquetacionAutorizacionBloqueo> listaEtiqAuto = new List<EtiquetacionAutorizacionBloqueo>();
            EtiquetacionAutorizacionBloqueo objEtiqAuto = new EtiquetacionAutorizacionBloqueo();
            string listaiCodCatalogos = "";

            foreach (GridViewRow row in grdEmpleSinEtiq.Rows)
            {
                objEtiqAuto = new EtiquetacionAutorizacionBloqueo();

                objEtiqAuto.autorizado = (row.FindControl("chbxAutorizado") as CheckBox).Checked == true ? 1 : 0;
                objEtiqAuto.iCodcatalogo = Convert.ToInt32((row.FindControl("lbliCodCatalogoEmple") as Label).Text.ToString());
                objEtiqAuto.cantNums = Convert.ToInt32((row.FindControl("lblCantNums") as Label).Text.ToString());

                listaEtiqAuto.Add(objEtiqAuto);
            }

            for (int i = 0; i <= 1; i++)
            {
                listaiCodCatalogos = "";
                var listaAutorizados = listaEtiqAuto.Where(x => x.autorizado == i);

                foreach (var cod in listaAutorizados)
                {
                    listaiCodCatalogos += cod.iCodcatalogo + ",";

                }

                listaiCodCatalogos = listaiCodCatalogos.Trim(',');

                if (listaiCodCatalogos != null && listaiCodCatalogos != "")
                {
                    DSODataAccess.Execute(ConsultaActualizaBEmpleAutorizadoBloqueo(i, listaiCodCatalogos));
                }
            }
        }

        public void ActualizarcBloquePendienteBloqueo()
        {
            List<EtiquetacionAutorizacionBloqueo> listaEtiqAuto = new List<EtiquetacionAutorizacionBloqueo>();
            EtiquetacionAutorizacionBloqueo objEtiqAuto = new EtiquetacionAutorizacionBloqueo();
            string listaiCodCatalogos = "";

            foreach (GridViewRow row in grdEmpleSinEtiq.Rows)
            {
                objEtiqAuto = new EtiquetacionAutorizacionBloqueo();

                objEtiqAuto.autorizado = (row.FindControl("chbxAutorizado") as CheckBox).Checked == true ? 1 : 0;
                objEtiqAuto.iCodcatalogo = Convert.ToInt32((row.FindControl("lbliCodCatalogoEmple") as Label).Text.ToString());
                objEtiqAuto.cantNums = Convert.ToInt32((row.FindControl("lblCantNums") as Label).Text.ToString());

                listaEtiqAuto.Add(objEtiqAuto);
            }

            for (int i = 0; i <= 1; i++)
            {
                listaiCodCatalogos = "";
                var listaAutorizados = listaEtiqAuto.Where(x => x.autorizado == i);

                foreach (var cod in listaAutorizados)
                {
                    listaiCodCatalogos += cod.iCodcatalogo + ",";

                }

                listaiCodCatalogos = listaiCodCatalogos.Trim(',');

                if (listaiCodCatalogos != null && listaiCodCatalogos != "")
                {
                    DSODataAccess.Execute(ConsultaActualizaCBloqueoEstatus(i, listaiCodCatalogos));
                }
            }
        }

        public string ConsultaActualizaBEmpleAutorizadoBloqueo(int aprobado, string listaiCodCatalogos)
        {
            StringBuilder query = new StringBuilder();

            query.AppendLine("Update [" + esquema + "].[VisHistoricos('BitacoraEmpleadosBloqueo','Bitacora Empleados Bloqueo','Español')]");
            query.AppendLine("	Set AprobadoParaBloqueo = " + aprobado);
            query.AppendLine("where Emple in (" + listaiCodCatalogos + ")");
            query.AppendLine("and dtIniVigencia <> dtFinVigencia ");
            query.AppendLine("and dtFinVigencia >= getdate()");
            query.AppendLine("and grupoBloqueo =  " + gBloqueo + "");
            query.AppendLine("and TiposBloqueo = " + tiposBloqueo + "");

            return query.ToString();
        }


        public string ConsultaActualizaCBloqueoEstatus(int aprobado, string listaiCodCatalogos)
        {
            string estatus = aprobado == 1 ? "2PendienteBloqueo" : "0PendienteAutorización";

            StringBuilder query = new StringBuilder();
            query.AppendLine("Update [" + esquema + "].[VisHistoricos('CodigoBloqueo','CodigosBloqueo','Español')]							   ");
            query.AppendLine("	set EstatusBloqueo = ");
            query.AppendLine("(	");
            query.AppendLine("	select iCodCatalogo ");
            query.AppendLine("		From evox.[VisHistoricos('EstatusBloqueo','Estatus Bloqueos','Español')]	");
            query.AppendLine("		where vchCodigo = '" + estatus + "'  ");
            query.AppendLine("		and dtIniVigencia <> dtFinVigencia  ");
            query.AppendLine("		and dtFinVigencia >= getdate()  ");
            query.AppendLine(")  ");
            query.AppendLine("                                                                                                                 ");
            query.AppendLine("	From  [" + esquema + "].[VisHistoricos('CodigoBloqueo','CodigosBloqueo','Español')] cBloqueo                                ");
            query.AppendLine("                                                                                                                 ");
            query.AppendLine("	INNER JOIN  [" + esquema + "].[VisHistoricos('BitacoraEmpleadosBloqueo','Bitacora Empleados Bloqueo','Español')] bEmple     ");
            query.AppendLine("	On bEmple.emple = cBloqueo.emple                                                                               ");
            query.AppendLine("	and bemple.dtIniVigencia <> bemple..dtFinVigencia                                                              ");
            query.AppendLine("	and bemple.dtFinVigencia >= GETDATE()                                                                          ");
            query.AppendLine("	and bemple.emple in (" + listaiCodCatalogos + ")                                                               ");
            query.AppendLine("  and bEmple.grupoBloqueo =  " + gBloqueo + "");
            query.AppendLine("  and bEmple.TiposBloqueo = " + tiposBloqueo + "");
            query.AppendLine("                                                                                                                 ");
            query.AppendLine("where cBloqueo.dtinivigencia <> cBloqueo.dtFinVigencia														   ");
            query.AppendLine("and   cBloqueo.dtFinVigencia >= getdate() 																	   ");

            return query.ToString();
        }

        public string ConsultaGetEmpleBitacora()
        {
            query.Length = 0;

            query.AppendLine("Declare @emple INT = (SELECT MAX(iCodCatalogo) FROM " + DSODataContext.Schema + ".[VisHistoricos('Emple','Empleados','Español')]");
            query.AppendLine("					    WHERE dtIniVigencia <> dtFinVigencia AND dtFinVigencia >= GETDATE() AND Usuar = " + Session["iCodUsuario"].ToString() + ")");
            query.AppendLine("Declare @esquema nvarchar(100)='[" + esquema + "]'      ");
            query.AppendLine("Declare @MarcaActual varchar(100)=''                   ");
            query.AppendLine("Declare @SitioDesc varchar(100)=''                     ");
            query.AppendLine("Declare @contador int =1                               ");
            query.AppendLine("Declare @numRegs int =0                                ");
            query.AppendLine("Declare @query nvarchar(4000)=''                       ");
            query.AppendLine("                                                       ");
            query.AppendLine("if(OBJECT_ID('Tempdb..#MarcasSitios') is not null)     ");
            query.AppendLine("begin                                                  ");
            query.AppendLine("	drop table #MarcasSitios                             ");
            query.AppendLine("end                                                    ");
            query.AppendLine("                                                       ");
            query.AppendLine("create table #MarcasSitios                             ");
            query.AppendLine("(                                                      ");
            query.AppendLine("	id int primary key identity not null,                ");
            query.AppendLine("	Marca varchar(100)                                   ");
            query.AppendLine(")                                                      ");
            query.AppendLine("                                                       ");
            query.AppendLine("if(OBJECT_ID('Tempdb..#FillersSitio') is not null)     ");
            query.AppendLine("begin                                                  ");
            query.AppendLine("	drop table #FillersSitio                             ");
            query.AppendLine("end                                                    ");
            query.AppendLine("                                                       ");
            query.AppendLine("create table #FillersSitio                             ");
            query.AppendLine("(                                                      ");
            query.AppendLine("	id int primary key identity not null,                ");
            query.AppendLine("	Sitio varchar(100),                                  ");
            query.AppendLine("	vchDescripcionSitio varchar(100),                    ");
            query.AppendLine("	Filler varchar(100),                                 ");
            query.AppendLine("                                                       ");
            query.AppendLine(")                                                      ");
            query.AppendLine("                                                       ");
            query.AppendLine("                                                       ");
            query.AppendLine("                                                       ");
            query.AppendLine("Set @query ='                                          ");
            query.AppendLine("select vchDesMaestro                                   ");
            query.AppendLine("from [" + esquema + "].[VisHisComun(''Sitio'',''Español'')]         ");
            query.AppendLine("where dtinivigencia <> dtfinvigencia                   ");
            query.AppendLine("and dtfinvigencia >= getdate()                         ");
            query.AppendLine("and MarcaSitioDesc is not null                         ");
            query.AppendLine("'                                                      ");
            query.AppendLine("insert into #MarcasSitios                              ");
            query.AppendLine("Execute sp_executesql	@query                           ");
            query.AppendLine("                                                       ");
            query.AppendLine("Select @numRegs= COUNT(*)                              ");
            query.AppendLine("From #MarcasSitios                                     ");
            query.AppendLine("                                                       ");
            query.AppendLine("                                                       ");
            query.AppendLine("while @contador <= @numRegs                            ");
            query.AppendLine("begin                                                  ");
            query.AppendLine("	Select @MarcaActual = Marca                          ");
            query.AppendLine("	From #MarcasSitios                                   ");
            query.AppendLine("	where id = @contador                                 ");
            query.AppendLine("                                                       ");
            query.AppendLine("	Set @query ='Select top 1 iCodCatalogo,vchDescripcion, Filler						  ");
            query.AppendLine("					From [" + esquema + "].[VisHistoricos(''sitio'','''+@MarcaActual+''',''Español'')] ");
            query.AppendLine("					Where dtIniVigencia <> dtFinVigencia                                  ");
            query.AppendLine("					and dtFinVigencia >= GETDATE()                                        ");
            query.AppendLine("					and Filler is not null'                                               ");
            query.AppendLine("                                                                                        ");
            query.AppendLine("					insert into #FillersSitio                                             ");
            query.AppendLine("					(sitio, vchDescripcionSitio, Filler)                                  ");
            query.AppendLine("					Exec (@query)                                                         ");
            query.AppendLine("                                                                                        ");
            query.AppendLine("				Set @contador = @contador +1                                              ");
            query.AppendLine("                                                                                        ");
            query.AppendLine("end                                                                                     ");
            query.AppendLine("                                                                                        ");
            query.AppendLine("select bEmple.Emple iCodCatalogoEmple,                                                   ");
            query.AppendLine("      Case when bEmple.AprobadoParaBloqueo = 1 Then 'true' else 'false' end AprobadoBloqueo,                                   ");
            query.AppendLine("		emple.NomCompleto Nombre,                                                         ");
            query.AppendLine("		cencos.Descripcion CenCos,                                                        ");
            query.AppendLine("		tempSitios.vchDescripcionSitio sitio,                                             ");
            query.AppendLine("		puesto.vchDescripcion Puesto,                                                            ");
            query.AppendLine("		bEmple.[Value]CantNums                                                                    ");
            query.AppendLine("	                                                                                      ");
            query.AppendLine("from [" + esquema + "].[VisHistoricos('BitacoraEmpleadosBloqueo','Bitacora Empleados Bloqueo','Español')] bEmple ");
            query.AppendLine("                                                                                                    ");
            query.AppendLine("	INNER JOIN [" + esquema + "].[VisHistoricos('Emple','Empleados','Español')] emple                              ");
            query.AppendLine("		On emple.iCodCatalogo = bEmple.emple                                                          ");
            query.AppendLine("		and emple.dtIniVigencia <> emple.dtFinVigencia                                                ");
            query.AppendLine("		and emple.dtFinVigencia >= getdate()                                                          ");
            query.AppendLine("                                                                                                    ");
            query.AppendLine("	INNER JOIN [" + esquema + "].[VisHistoricos('Cencos','Centro de Costos','Español')] cencos                     ");
            query.AppendLine("		ON cencos.iCodCatalogo = emple.CenCos                                                         ");
            query.AppendLine("		and cencos.dtinivigencia <> cencos.dtfinvigencia                                              ");
            query.AppendLine("		and cencos.dtfinvigencia >= getdate()                                                         ");
            query.AppendLine("                                                                                                    ");
            query.AppendLine("	INNER JOIN #FillersSitio tempSitios                                                               ");
            query.AppendLine("		ON tempSitios.Filler = emple.Ubica                                                            ");
            query.AppendLine("                                                                                                    ");
            query.AppendLine("	INNER JOIN  [" + esquema + "].[VisHistoricos('Puesto','Puestos Empleado','Español')] puesto                    ");
            query.AppendLine("		On puesto.iCodCatalogo = emple.puesto                                                         ");
            query.AppendLine("		and puesto.dtIniVigencia <> puesto.dtFinVigencia                                              ");
            query.AppendLine("		and puesto.dtFinVigencia >= getdate()                                                         ");
            query.AppendLine("");
            query.AppendLine("INNER JOIN [" + esquema + "].[VisHistoricos('CodigoBloqueo','CodigosBloqueo','Español')] cBloqueo");
            query.AppendLine("	ON cBloqueo.emple = bEmple.emple");
            query.AppendLine("	and cBloqueo.dtIniVigencia <> cBloqueo.dtFinVigencia");
            query.AppendLine("	and cBloqueo.dtFinVigencia >= getdate()");
            query.AppendLine("	and cBloqueo.estatusBloqueo = ");
            query.AppendLine("								(");
            query.AppendLine("										select icodCatalogo");
            query.AppendLine("										from [" + esquema + "].[VisHistoricos('EstatusBloqueo','Estatus Bloqueos','Español')]");
            query.AppendLine("														where dtInivigencia <> dtfinvigencia ");
            query.AppendLine("														and dtfinvigencia >= getdate()");
            query.AppendLine("														and vchCodigo = '0PendienteAutorización'");
            query.AppendLine("								)");
            query.AppendLine("");
            query.AppendLine("INNER JOIN [" + esquema + "].[VisHistoricos('GrupoBloqueo','Grupos de Bloqueo','Español')] gBloqueo");
            query.AppendLine("  On   gBloqueo.dtIniVigencia <> gBloqueo.dtFinVigencia ");
            query.AppendLine("  and     gBloqueo.dtFinVigencia >= GETDATE()");
            query.AppendLine("  and     gBloqueo.iCodCatalogo = " + gBloqueo + "");
            query.AppendLine("  and     gBloqueo.emple = @emple");
            query.AppendLine("");
            query.AppendLine("Where bEmple.dtIniVigencia <> bEmple.dtFinVigencia                                                  ");
            query.AppendLine("and bEmple.dtFinVigencia >= GETDATE()                                                               ");
            query.AppendLine("and bEmple.grupoBloqueo =  " + gBloqueo + "");
            query.AppendLine("and bEmple.TiposBloqueo = " + tiposBloqueo + "");
            query.AppendLine("");
            query.AppendLine("group by    bEmple.Emple ,       ");
            query.AppendLine("            bEmple.AprobadoParaBloqueo,  ");
            query.AppendLine("            emple.NomCompleto,                        ");
            query.AppendLine("            cencos.Descripcion,             ");
            query.AppendLine("            tempSitios.vchDescripcionSitio,    ");
            query.AppendLine("            puesto.vchDescripcion,         ");
            query.AppendLine("            bEmple.[Value] ");
            query.AppendLine("");
            query.AppendLine("Order by emple.nomCompleto");

            return query.ToString();
        }

        protected void btnRegresar_Click(object sender, EventArgs e)
        {
            if (Session["iCodPerfil"].ToString() == "370")
            {
                HttpContext.Current.Response.Redirect(Session["HomePage"].ToString());
            }
            else
            {
                HttpContext.Current.Response.Redirect("~/UserInterface/DashboardFC/Dashboard.aspx?MiConsumo=1&Opc=opcMiConsumo");
            }
        }
    }
}
