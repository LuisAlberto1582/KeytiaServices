using KeytiaServiceBL.XmlRpc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Xml;
using SeeYouOnServiceBL.Models;
using SeeYouOnServiceBL.MCUOperations;
using DSOControls2008;
using System.Text;
using KeytiaServiceBL;
using System.Data;
using KeytiaServiceBL.DataAccess.ModelsDataAccess;


namespace KeytiaWeb.UserInterface.SYO.admin.Conference
{
    public partial class ConferenceAdd : System.Web.UI.Page
    {
        StringBuilder query = new StringBuilder();
        string nombreConferencia = string.Empty;
        string descripcionConferencia = string.Empty;
        int numberIdConferencia = 0;
        DateTime fechaInicioConferencia;
        DateTime fechaFinConferencia;

        protected void Page_Load(object sender, EventArgs e)
        {
            try
            {
                #region Buscar el control de fecha que hereda de la Master y lo oculta ya que aquí no se usa.
                ((Panel)Form.FindControl("pnlRangeFechas")).Visible = false;
                #endregion

                if (!Page.IsPostBack)
                {
                    #region Configuración Inicial
                    try
                    {
                        if (Session["Language"].ToString() == "Español")
                        {
                            pdtInicio.setRegion("es");
                            pdtFin.setRegion("es");
                        }

                        pdtInicio.MaxDateTime = new DateTime((DateTime.Now.Year + 3), 1, 1);
                        pdtInicio.MinDateTime = DateTime.Now;

                        pdtFin.MaxDateTime = pdtInicio.MaxDateTime;
                        pdtFin.MinDateTime = pdtInicio.MinDateTime;

                        pdtInicio.CreateControls();
                        pdtInicio.DataValue = (object)DateTime.Now;

                        pdtFin.CreateControls();
                        pdtFin.DataValue = (object)DateTime.Now;

                        FillParticipantes();
                    }
                    catch (Exception ex)
                    {
                        throw new KeytiaWebException(
                            "Ocurrio un error al darle valores default a los campos de fecha en '" + Request.Path
                            + HttpContext.Current.Session["vchCodUsuario"] + "'", ex);
                    }
                    #endregion Configuración Inical
                }

                DashboardInicial();
            }
            catch (Exception ex)
            {
                throw new KeytiaWebException("Ocurrio un error en " + Request.Path + HttpContext.Current.Session["vchCodUsuario"] + "'", ex);
            }
        }

        #region Vista Principal

        void DashboardInicial()
        {            
            FillConferencias();
        }

        void FillParticipantes()
        {
            var dt = DSODataAccess.Execute(GetParticipantes());
            chkListParticipantes.DataSource = null;
            chkListParticipantes.DataSource = dt;
            chkListParticipantes.DataTextField = "Nombre";
            chkListParticipantes.DataValueField = "Id";
            chkListParticipantes.DataBind();
        }

        void FillConferencias()
        {
            var dt = DSODataAccess.Execute(GetHistoriaConferencias());
            grvConferencias.DataSource = null;
            grvConferencias.DataSource = dt;
            grvConferencias.DataBind();
            if (dt != null && dt.Rows.Count > 0)
            {
                grvConferencias.HeaderRow.TableSection = TableRowSection.TableHeader; 
            }
        }

        #endregion Vista Principal

        #region Consultas

        string GetParticipantes()
        {
            query.Length = 0;
            query.AppendLine("SELECT Id, Nombre, Tipo, Protocolo, Address");
            query.AppendLine("FROM " + DSODataContext.Schema + ".BMUsuarios");
            return query.ToString();
        }

        string GetHistoriaConferencias()
        {
            query.Length = 0;
            query.AppendLine("SET NUMERIC_ROUNDABORT OFF");
            query.AppendLine("SET ANSI_NULLS, ANSI_PADDING, ANSI_WARNINGS, ARITHABORT, CONCAT_NULL_YIELDS_NULL, QUOTED_IDENTIFIER ON");
            query.AppendLine("");
            query.AppendLine("SELECT C.Id, C.NumberId, C.Nombre, C.FechaInicio, C.FechaFin,");
            query.AppendLine("      Estatus		= CASE WHEN GETDATE() BETWEEN C.FechaInicio AND C.FechaFin THEN 'En progreso'");
            query.AppendLine("				            WHEN C.FechaFin < GETDATE() THEN 'Finalizada'");
            query.AppendLine("				            WHEN C.FechaFin > GETDATE() THEN 'Programada' ELSE '' END,");
            query.AppendLine("      Participante	= U.Nombre");
            query.AppendLine("INTO #tempTabla");
            query.AppendLine("FROM " + DSODataContext.Schema + ".BMConferencia C");
            query.AppendLine("    JOIN " + DSODataContext.Schema + ".BMConferenciaParticipante R");
            query.AppendLine("        ON R.IdConferencia = C.Id");
            query.AppendLine("    JOIN " + DSODataContext.Schema + ".BMUsuarios U");
            query.AppendLine("        ON U.Id = R.IdParticipante");
            query.AppendLine("GROUP BY C.Id, C.NumberId, C.Nombre, C.FechaInicio, C.FechaFin, U.Nombre");
            query.AppendLine("ORDER BY C.FechaInicio DESC");
            query.AppendLine("");
            query.AppendLine("");
            query.AppendLine("SELECT Id");
            query.AppendLine("       , NumberId");
            query.AppendLine("       , Nombre");
            query.AppendLine("       , FechaInicio");
            query.AppendLine("       , FechaFin");
            query.AppendLine("       , Estatus");
            query.AppendLine("       , STUFF(( SELECT ', ' + Participante + ''");
            query.AppendLine("                 FROM #tempTabla t2 ");
            query.AppendLine("                 WHERE t2.Id = t1.Id ");
            query.AppendLine("		              FOR XML PATH(''), TYPE).value('.', 'varchar(max)'),1,1,'' ");
            query.AppendLine("             ) AS Participantes ");
            query.AppendLine(" FROM #tempTabla t1 ");
            query.AppendLine(" GROUP BY Id, NumberId, Nombre, FechaInicio, FechaFin, Estatus");
            query.AppendLine(" ORDER BY FechaInicio DESC");
            return query.ToString();
        }

        bool ValidaExisteValor(string valor, string nombreCampo)
        {
            query.Length = 0;
            query.AppendLine("SELECT COUNT(Id)");
            query.AppendLine("FROM " + DSODataContext.Schema + ".BMConferencia");
            query.AppendLine("WHERE " + nombreCampo + " = '" + valor + "'");

            var dt = DSODataAccess.Execute(query.ToString());
            if (dt != null && dt.Rows.Count > 0)
            {
                var cantidad = Convert.ToInt32(dt.Rows[0][0]);
                return cantidad > 0 ? true : false;
            }
            return true;
        }

        int GetNumberId() 
        {
            query.Length = 0;
            query.AppendLine("SELECT ISNULL(MAX(CONVERT(INT, NumberId)),1000) + 1");
            query.AppendLine("FROM " + DSODataContext.Schema + ".BMConferencia");

            var dt = DSODataAccess.Execute(query.ToString());
            if (dt != null && dt.Rows.Count > 0)
            {
                return Convert.ToInt32(dt.Rows[0][0]);
            }
            return 1000;
        }

        #endregion Consultas

        #region Evento Botones

        protected void lbtnSaveConferencia_Click(object sender, EventArgs e)
        {
            try
            {
                ValidacionDatos();
                var listaPart = chkListParticipantes.Items.Cast<ListItem>().Where(li => li.Selected).Select(li => Convert.ToInt32(li.Value)).ToList();
                TimeSpan duracion = fechaFinConferencia - fechaInicioConferencia;
                CrearConferenciaMCU(nombreConferencia, descripcionConferencia, numberIdConferencia, fechaInicioConferencia, Convert.ToInt32(duracion.TotalSeconds), listaPart);
                CrearConferenciaBD(nombreConferencia, descripcionConferencia, numberIdConferencia, fechaInicioConferencia, fechaFinConferencia, listaPart);

                FillConferencias();
                LimpiarDatos();
            }
            catch (ArgumentException ex)
            {
                lblTituloModalMsn.Text = "Validación de datos";
                lblBodyModalMsn.Text = ex.Message;
                mpeEtqMsn.Show();
            }
            catch (Exception)
            {
                lblTituloModalMsn.Text = "Error";
                lblBodyModalMsn.Text = "No fue posible completar la operación.";
                mpeEtqMsn.Show();
            }
        }

        protected void lbtnCancelarAddConferencia_Click(object sender, EventArgs e)
        {
            LimpiarDatos();
        }

        #endregion

        #region Logica Negocio

        void LimpiarDatos()
        {
            txtNombre.Text = string.Empty;
            txtDescripcion.Text = string.Empty;
            pdtInicio.DataValue = (object)DateTime.Now;
            pdtFin.DataValue = (object)DateTime.Now;
            FillParticipantes();

            nombreConferencia = string.Empty;
            descripcionConferencia = string.Empty;
            fechaInicioConferencia = DateTime.MinValue;
            fechaFinConferencia = DateTime.MinValue;
        }

        bool ValidacionDatos()
        {
            nombreConferencia = LimpiarPalabrasReservadas(txtNombre.Text);
            descripcionConferencia = LimpiarPalabrasReservadas(txtDescripcion.Text);
            numberIdConferencia = GetNumberId();
            fechaInicioConferencia = pdtInicio.Date;
            fechaFinConferencia = pdtFin.Date;

            int x;

            if (string.IsNullOrEmpty(nombreConferencia))
            {
                throw new ArgumentException("El nombre de la conferencia no puede quedar vacío.");
            }
            if (string.IsNullOrEmpty(descripcionConferencia))
            {
                throw new ArgumentException("La descripción de la conferencia no puede quedar vacío.");
            }           
            if (fechaInicioConferencia < DateTime.Now)
            {
                throw new ArgumentException("La fecha de inicio de la conferencia debe ser mayor a la fecha actual.");
            }
            if (fechaInicioConferencia == fechaFinConferencia)
            {
                throw new ArgumentException("Las fecha de inicio y fin de la conferencia no pueden ser iguales.");
            }
            if (fechaFinConferencia < fechaInicioConferencia)
            {
                throw new ArgumentException("La fecha fin de la conferencia no puede ser menor que la fecha de inicio de la conferencia.");
            }
            if (chkListParticipantes.Items.Cast<ListItem>().Where(w => w.Selected).Count() == 0) //Valida que se haya incluido por lo menos a un participante
            {
                throw new ArgumentException("Favor de indicar al menos un participante en la conferencia.");
            }
            if (ValidaExisteValor(nombreConferencia, "Nombre")) //Valida que el nombre de la conferencia no exista previamente en la base de datos.
            {
                throw new ArgumentException("Ya existe una conferencia con el nombre especificado. Favor se elegir uno diferente.");
            }
            if (ValidaExisteValor(numberIdConferencia.ToString(), "NumberId")) //Valida que el Number Id no exista ya en la base de datos.
            {
                throw new ArgumentException("Ya existe una conferencia con el mismo Number Id. Favor de elegir uno diferente");
            }

            return true;
        }

        bool CrearConferenciaMCU(string conferenceName, string descripcionConf, int numIdConf, DateTime fechaInicioConf, int duracionSeg, List<int> selectedParticipant)
        {
            bool conferenciaCreada = false;
            var participantesConference = new Dictionary<MCU4520Participant, bool>();
            var participantBD = DSODataAccess.Execute(GetParticipantes()).AsEnumerable();
            List<MCU4520Participant> participantes = new List<MCU4520Participant>();

            var credentials = new MCUCredentials
            {
                URI = "https://207.200.56.213/rpc2",
                Username = "dtikeytia",
                Password = "K3yt1a$2019"
            };

            var conference = new MCU4520Conference
            {
                Name = conferenceName,
                Description = descripcionConf,
                NumericId = numIdConf.ToString(),
                RegisterWithSIPRegistrar = true,
                StartTime = fechaInicioConf,
                DurationSeconds = duracionSeg,
                PreconfiguredParticipantsDefer = false
            };

            selectedParticipant.ForEach(p =>
                {
                    var row = participantBD.FirstOrDefault(x => x.Field<int>("Id") == p);
                    if (row != null)
                    {
                        participantes.Add(
                           new MCU4520Participant
                           {
                               ParticipantName = row["Nombre"].ToString(),
                               ParticipantProtocol = row["Protocolo"].ToString(),
                               ParticipantType = row["Tipo"].ToString(),
                               Address = row["Address"].ToString()
                           }
                       );
                    }
                });


            //Crea y programa conferencia
            var responseCreate = MCU4520ConferenceCreate.Execute(credentials, conference);
            if (!responseCreate.IsFault())
            {
                if (responseCreate.GetString().ToLower() == "{status: operation successful}") { conferenciaCreada = true; }
            }
            else
            {
                System.Diagnostics.Debug.WriteLine(responseCreate.GetFaultString());
                throw new ArgumentException("No fue posible crear la conferencia.");
            }

            if (conferenciaCreada)
            {
                foreach (var participante in participantes)
                {
                    //Asigna participantes a conferencia
                    var responseParticipantAdd = MCU4520ParticipantAdd.Execute(credentials, conferenceName, participante);

                    if (!responseParticipantAdd.IsFault())
                    {
                        if (responseCreate.GetString().ToLower() == "{status: operation successful}")
                        {
                            participantesConference.Add(participante, true);
                        }
                        else { participantesConference.Add(participante, false); }
                    }
                    else { participantesConference.Add(participante, false); }
                }
            }

            if (participantesConference.ContainsValue(false))
            {
                throw new ArgumentException("No fue posible agregar a la conferencia a la totalidad de participantes.");
            }

            return true;
        }

        bool CrearConferenciaBD(string nombre, string descripcion, int numId, DateTime fechaInicio, DateTime fechaFin, List<int> participantes)
        {
            try
            {
                query.Length = 0;
                query.AppendLine("Declare @Inserted table (Id Int) ");
                query.AppendLine("INSERT INTO " + DSODataContext.Schema + ".BMConferencia OUTPUT INSERTED.Id INTO @Inserted VALUES");
                query.AppendLine("('" + nombre + "',");
                query.AppendLine("'" + descripcion + "',");
                query.AppendLine("'" + numId + "',");
                query.AppendLine("'" + fechaInicio.ToString("yyyy-MM-dd HH:mm:ss") + "',");
                query.AppendLine("'" + fechaFin.ToString("yyyy-MM-dd HH:mm:ss") + "',");
                query.AppendLine("GETDATE()) ");
                query.AppendLine("select Id from @Inserted ");

                int idConferencia = 0;
                var dt = DSODataAccess.Execute(query.ToString());
                if (dt != null && dt.Rows.Count > 0)
                {
                    idConferencia = Convert.ToInt32(dt.Rows[0][0]);
                }

                if (idConferencia > 0)
                {
                    participantes.ForEach(n =>
                        {
                            query.Length = 0;
                            query.AppendLine("INSERT INTO " + DSODataContext.Schema + ".BMConferenciaParticipante VALUES");
                            query.AppendLine("(" + idConferencia + ",");
                            query.AppendLine(n + ","); ;
                            query.AppendLine("GETDATE())");

                            DSODataAccess.Execute(query.ToString());
                        });
                }
                else { throw new ArgumentException("No fue posible registrar la conferencia en el sistema."); }

                return true;
            }
            catch (Exception ex)
            {
                throw new ArgumentException("No fue posible registrar la conferencia en el sistema.", ex);
            }
        }

        string LimpiarPalabrasReservadas(string x)
        {
            return x.ToUpper().Replace("INSERT", "").Replace("DELETE", "").Replace("TRUNCATE", "");
        }
        
        #endregion

    }
}