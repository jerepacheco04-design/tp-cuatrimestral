using System;
using System.Data.SqlClient;
using ClinicaTurnos.Dominio;
using ClinicaTurnos.Negocio;

namespace ClinicaTurnos.Web
{
    public partial class HistorialPaciente : PaginaBase
    {
        protected override bool PermiteMedico { get { return true; } }

        protected void Page_Load(object sender, EventArgs e)
        {
            if (IsPostBack)
                return;

            bool esMedico = UsuarioActual.Perfil == PerfilUsuario.Medico;
            lnkVolver.Text = esMedico ? "Volver a mi agenda" : "Volver a pacientes";
            lnkVolver.NavigateUrl = esMedico ? "MiAgenda.aspx" : "Pacientes.aspx";
            grilla.Columns[grilla.Columns.Count - 1].Visible = esMedico;

            int id;
            string parametro = esMedico ? "turnoId" : "id";
            if (!int.TryParse(Request.QueryString[parametro], out id) || id <= 0)
            {
                lblMensaje.Text = "El historial no está disponible.";
                return;
            }

            try
            {
                TurnoNegocio negocio = new TurnoNegocio();
                Paciente paciente;
                int? medicoId = null;
                if (esMedico)
                {
                    medicoId = UsuarioActual.MedicoId.Value;
                    Turno turno = negocio.ObtenerDelMedico(id, medicoId.Value);
                    paciente = turno == null ? null : turno.Paciente;
                    if (turno != null)
                    {
                        lnkVolver.Text = "Volver a la consulta";
                        lnkVolver.NavigateUrl = "ConsultaTurno.aspx?id=" + turno.Id;
                    }
                }
                else
                    paciente = new PacienteNegocio().Obtener(id);

                if (paciente == null)
                {
                    lblMensaje.Text = "El historial no está disponible.";
                    return;
                }

                lblPaciente.Text = Server.HtmlEncode(paciente.Nombre + " " + paciente.Apellido);
                grilla.DataSource = negocio.ListarHistorial(paciente.Id, medicoId);
                grilla.DataBind();
            }
            catch (SqlException)
            {
                lblMensaje.Text = "No se pudo cargar el historial. Revisá la base de datos.";
            }
        }
    }
}
