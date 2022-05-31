using KeytiaServiceBL;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Services;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace KeytiaWeb.UserInterface.DashboardFC
{
    public partial class AdministracionDeCajeros : System.Web.UI.Page
    {
        public static StringBuilder consulta = ConsultaTodosLosDatos();

        protected void Page_Load(object sender, EventArgs e)
        {

            consulta = ConsultaTodosLosDatos();

            if (chkBoxFiltroTerrestres.Checked)
            {
                if (!consulta.ToString().Contains("AND"))
                {
                    consulta.AppendLine(" AND");
                    consulta.AppendLine(" EnlacesTerrestres != 0");
                }
                else
                {
                    if (!consulta.ToString().Contains("EnlacesTerrestres != 0"))
                    {
                        consulta.AppendLine(" OR EnlacesTerrestres != 0");
                    }
                }
            }

            if (chkBoxFiltroSatelitales.Checked)
            {
                if (!consulta.ToString().Contains("AND"))
                {
                    consulta.AppendLine(" AND");
                    consulta = consulta.AppendLine(" EnlacesSatelitales != 0");
                }
                else
                {
                    if (!consulta.ToString().Contains("EnlacesSatelitales != 0"))
                    {
                        consulta = consulta.AppendLine(" OR EnlacesSatelitales != 0");
                    }
                }
            }

            if (chkBoxFiltroCelulares.Checked)
            {
                if (!consulta.ToString().Contains("AND"))
                {
                    consulta.AppendLine(" AND");
                    consulta = consulta.AppendLine(" EnlacesCelulares != 0");
                }
                else
                {
                    if (!consulta.ToString().Contains("EnlacesCelulares != 0"))
                    {
                        consulta = consulta.AppendLine(" OR EnlacesCelulares != 0");
                    }
                }
            }


            DataTable dummy = new DataTable();
            dummy.Columns.Add("NombreCajero");
            dummy.Columns.Add("Folio");
            dummy.Columns.Add("FolioEnlaces");
            dummy.Columns.Add("Contacto");
            dummy.Columns.Add("ContactoRegional");
            dummy.Columns.Add("Domicilio");
            dummy.Columns.Add("EntreCalles");
            dummy.Columns.Add("Colonia");
            dummy.Columns.Add("CodigoPostal");
            dummy.Columns.Add("Municipio");
            dummy.Columns.Add("Ciudad");
            dummy.Columns.Add("Estado");
            dummy.Columns.Add("Latitud");
            dummy.Columns.Add("Longitud");
            dummy.Columns.Add("Telefono");
            dummy.Rows.Add();
            example.DataSource = dummy;
            example.DataBind();

            //Required for jQuery DataTables to work.
            example.UseAccessibleHeader = true;
            example.HeaderRow.TableSection = TableRowSection.TableHeader;
        }

        #region Botones

        protected void AgregarCajero_Click(object sender, EventArgs e)
        {
            Response.Redirect("ConfiguracionUbicacionesCajero.aspx");
        }
        #endregion

        public static StringBuilder ConsultaTodosLosDatos()
        {
            StringBuilder consulta = new StringBuilder();

            consulta.AppendLine("SELECT Folio, Nombre, Domicilio, Domicilio2, Colonia, CodigoPostal, Municipio, Ciudad, Estado, Latitud, Longitud, Contacto, ContactoRegional, Telefono, FolioEnlaces \r");
            consulta.AppendLine("FROM Keytia5.k5banorte.CajeroInstalacion");
            consulta.AppendLine("WHERE dtFinVigencia > GETDATE()");

            return consulta;
        }

        public static string ConsultaDatosFiltrados()
        {
            StringBuilder consulta = new StringBuilder();

            consulta.AppendLine("SELECT Folio, Folio, Folio, Domicilio2, Colonia, CodigoPostal, Municipio, Ciudad, Estado, Latitud, Longitud, Contacto, ContactoRegional, Telefono \r");
            consulta.AppendLine("FROM Keytia5.k5banorte.CajeroInstalacion");
            consulta.AppendLine("WHERE dtFinVigencia > GETDATE()");

            return consulta.ToString();
        }

        #region Consultas

        [WebMethod]
        public static List<SampleModel> GetInfo()
        {
            DataTable dtConsulta;
            //StringBuilder consulta = new StringBuilder();
            List<SampleModel> repo = new List<SampleModel>();

            //consulta.AppendLine("SELECT Folio, Nombre, Domicilio, Domicilio2, Colonia, CodigoPostal, Municipio, Ciudad, Estado, Latitud, Longitud, Contacto, ContactoRegional, Telefono \r");
            //consulta.AppendLine("FROM Keytia5.k5banorte.CajeroInstalacion");
            //consulta.AppendLine("WHERE dtFinVigencia > GETDATE()");

            //dtConsulta = DSODataAccess.Execute(consulta.ToString());

            dtConsulta = DSODataAccess.Execute(consulta.ToString());

            foreach (DataRow row in dtConsulta.Rows)
            {
                repo.Add(new SampleModel()
                {
                    NombreCajero = row[1].ToString(),
                    FolioCajero = row[0].ToString(),
                    FolioEnlaces = row[14].ToString(),
                    Contacto = row[11].ToString(),
                    ContactoRegional = row[12].ToString(),
                    Domicilio = row[2].ToString(),
                    EntreCalles = row[3].ToString(),
                    Colonia = row[4].ToString(),
                    CodigoPostal = row[5].ToString(),
                    Municipio = row[6].ToString(),
                    Ciudad = row[7].ToString(),
                    Estado = row[8].ToString(),
                    Latitud = row[9].ToString(),
                    Longitud = row[10].ToString(),
                    Telefono = row[13].ToString(),
                }
                );
            }
            return repo;
        }

        [WebMethod]
        public static List<SampleModel> GetInfo2()
        {
            DataTable dtConsulta;
            StringBuilder consulta = new StringBuilder();
            List<SampleModel> repo = new List<SampleModel>();

            consulta.AppendLine("SELECT Folio, Folio, Folio, Domicilio2, Colonia, CodigoPostal, Municipio, Ciudad, Estado, Latitud, Longitud, Contacto, ContactoRegional, Telefono \r");
            consulta.AppendLine("FROM Keytia5.k5banorte.CajeroInstalacion");
            consulta.AppendLine("WHERE dtFinVigencia > GETDATE()");

            dtConsulta = DSODataAccess.Execute(consulta.ToString());

            foreach (DataRow row in dtConsulta.Rows)
            {
                repo.Add(new SampleModel()
                {
                    NombreCajero = row[1].ToString(),
                    FolioCajero = row[0].ToString(),
                    FolioEnlaces = row[14].ToString(),
                    Contacto = row[11].ToString(),
                    ContactoRegional = row[12].ToString(),
                    Domicilio = row[2].ToString(),
                    EntreCalles = row[3].ToString(),
                    Colonia = row[4].ToString(),
                    CodigoPostal = row[5].ToString(),
                    Municipio = row[6].ToString(),
                    Ciudad = row[7].ToString(),
                    Estado = row[8].ToString(),
                    Latitud = row[9].ToString(),
                    Longitud = row[10].ToString(),
                    Telefono = row[13].ToString(),
                }
                );
            }
            return repo;
        }

        #endregion

        #region Modelos

        public class SampleModel
        {
            public string NombreCajero { get; set; }
            public string FolioCajero { get; set; }
            public string FolioEnlaces { get; set; }
            public string Contacto { get; set; }
            public string ContactoRegional { get; set; }
            public string Domicilio { get; set; }
            public string EntreCalles { get; set; }
            public string Colonia { get; set; }
            public string CodigoPostal { get; set; }
            public string Municipio { get; set; }
            public string Ciudad { get; set; }
            public string Estado { get; set; }
            public string Latitud { get; set; }
            public string Longitud { get; set; }
            public string Telefono { get; set; }
        }

        #endregion
    }
}