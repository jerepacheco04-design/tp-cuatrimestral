using System;
using System.Data.SqlClient;
using System.Globalization;
using System.Web.UI.WebControls;
using ClinicaTurnos.Dominio;
using ClinicaTurnos.Negocio;

namespace ClinicaTurnos.Web
{
    public partial class TurnoAlta : PaginaBase
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            lblMensaje.Text = "";
            txtFecha.Attributes["min"] = DateTime.Today.ToString("yyyy-MM-dd", CultureInfo.InvariantCulture);
            if (IsPostBack)
                return;

            ddlPaciente.Items.Add(new ListItem("Seleccioná un paciente", ""));
            ddlEspecialidad.Items.Add(new ListItem("Seleccioná una especialidad", ""));
            ddlMedico.Items.Add(new ListItem("Primero elegí una especialidad", ""));
            try
            {
                foreach (Paciente paciente in new PacienteNegocio().Listar())
                    ddlPaciente.Items.Add(new ListItem(paciente.Apellido + ", " + paciente.Nombre + " (" + paciente.Dni + ")", paciente.Id.ToString()));
                foreach (Especialidad especialidad in new EspecialidadNegocio().Listar())
                    ddlEspecialidad.Items.Add(new ListItem(especialidad.Nombre, especialidad.Id.ToString()));

                if (ddlPaciente.Items.Count == 1 || ddlEspecialidad.Items.Count == 1)
                {
                    lblMensaje.Text = "Para crear un turno, primero cargá un paciente y una especialidad.";
                    btnGuardar.Enabled = false;
                }
            }
            catch (SqlException)
            {
                lblMensaje.Text = "No se pudo cargar el formulario. Revisá la base de datos.";
                btnGuardar.Enabled = false;
            }
        }

        protected void ddlEspecialidad_SelectedIndexChanged(object sender, EventArgs e)
        {
            ddlMedico.Items.Clear();
            int especialidad;
            if (!int.TryParse(ddlEspecialidad.SelectedValue, out especialidad))
            {
                ddlMedico.Items.Add(new ListItem("Primero elegí una especialidad", ""));
                return;
            }
            ddlMedico.Items.Add(new ListItem("Seleccioná un médico", ""));
            try
            {
                foreach (Medico medico in new TurnoNegocio().ListarMedicos(especialidad))
                    ddlMedico.Items.Add(new ListItem(medico.Apellido + ", " + medico.Nombre, medico.Id.ToString()));
                if (ddlMedico.Items.Count == 1)
                    lblMensaje.Text = "Todavía no hay médicos para esta especialidad.";
            }
            catch (SqlException)
            {
                lblMensaje.Text = "No se pudieron cargar los médicos. Revisá la base de datos.";
            }
        }

        protected void btnGuardar_Click(object sender, EventArgs e)
        {
            if (!btnGuardar.Enabled)
                return;

            DateTime fechaHora;
            if (!DateTime.TryParseExact(txtFecha.Text + " " + txtHora.Text, "yyyy-MM-dd HH:mm",
                CultureInfo.InvariantCulture, DateTimeStyles.None, out fechaHora))
            {
                lblMensaje.Text = "Ingresá una fecha y una hora válidas.";
                return;
            }

            int pacienteId;
            int especialidadId;
            int medicoId;
            int.TryParse(ddlPaciente.SelectedValue, out pacienteId);
            int.TryParse(ddlEspecialidad.SelectedValue, out especialidadId);
            int.TryParse(ddlMedico.SelectedValue, out medicoId);
            Turno turno = new Turno
            {
                Paciente = new Paciente { Id = pacienteId },
                Especialidad = new Especialidad { Id = especialidadId },
                Medico = new Medico { Id = medicoId },
                FechaHora = fechaHora,
                MotivoConsulta = txtMotivo.Text
            };
            try
            {
                new TurnoNegocio().Agregar(turno);
                Session["Mensaje"] = "Turno agregado correctamente.";
                Response.Redirect("Turnos.aspx");
            }
            catch (ArgumentException ex)
            {
                lblMensaje.Text = Server.HtmlEncode(ex.Message);
            }
            catch (SqlException)
            {
                lblMensaje.Text = "No se pudo guardar el turno. Intentá nuevamente.";
            }
        }
    }
}
