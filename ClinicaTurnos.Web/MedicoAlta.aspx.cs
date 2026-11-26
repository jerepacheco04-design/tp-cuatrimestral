using System;
using System.Data.SqlClient;
using System.Linq;
using System.Web.UI.WebControls;
using ClinicaTurnos.Dominio;
using ClinicaTurnos.Negocio;

namespace ClinicaTurnos.Web
{
    public partial class MedicoAlta : PaginaBase
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            lblMensaje.Text = "";
            int id = IdEdicion;
            if (id < 0)
            {
                lblMensaje.Text = "El médico seleccionado no es válido.";
                btnGuardar.Enabled = false;
                return;
            }
            if (id > 0)
            {
                litTitulo.Text = "Modificar médico";
                btnGuardar.Text = "Guardar cambios";
            }
            if (IsPostBack)
                return;

            try
            {
                ddlHorario.DataSource = new TurnoTrabajoNegocio().Listar();
                ddlHorario.DataTextField = "Nombre";
                ddlHorario.DataValueField = "Id";
                ddlHorario.DataBind();
                ddlHorario.Items.Insert(0, new ListItem("Seleccioná un horario", ""));

                foreach (Especialidad especialidad in new EspecialidadNegocio().Listar())
                    listaEspecialidades.Items.Add(new ListItem(Server.HtmlEncode(especialidad.Nombre), especialidad.Id.ToString()));

                if (ddlHorario.Items.Count == 1 || listaEspecialidades.Items.Count == 0)
                {
                    lblMensaje.Text = "Para cargar un médico, primero tiene que haber al menos un horario y una especialidad.";
                    btnGuardar.Enabled = false;
                }
                if (id > 0)
                    CargarMedico(id);
            }
            catch (SqlException)
            {
                lblMensaje.Text = "No se pudo cargar el formulario del médico.";
                btnGuardar.Enabled = false;
            }
        }

        private void CargarMedico(int id)
        {
            Medico medico = new MedicoNegocio().Obtener(id);
            if (medico == null)
            {
                lblMensaje.Text = "El médico ya no existe. Volvé al listado.";
                btnGuardar.Enabled = false;
                return;
            }
            txtMatricula.Text = medico.Matricula;
            txtNombre.Text = medico.Nombre;
            txtApellido.Text = medico.Apellido;
            txtEmail.Text = medico.Email;

            ListItem horario = ddlHorario.Items.FindByValue(medico.TurnoTrabajo.Id.ToString());
            if (horario != null)
                ddlHorario.SelectedValue = horario.Value;

            foreach (Especialidad especialidad in medico.Especialidades)
            {
                ListItem opcion = listaEspecialidades.Items.FindByValue(especialidad.Id.ToString());
                if (opcion != null)
                    opcion.Selected = true;
            }
        }

        protected void btnGuardar_Click(object sender, EventArgs e)
        {
            int id = IdEdicion;
            if (id < 0 || !btnGuardar.Enabled)
                return;

            int horario;
            int.TryParse(ddlHorario.SelectedValue, out horario);
            Medico medico = new Medico
            {
                Id = id,
                Matricula = txtMatricula.Text,
                Nombre = txtNombre.Text,
                Apellido = txtApellido.Text,
                Email = txtEmail.Text,
                TurnoTrabajo = new TurnoTrabajo { Id = horario },
                Especialidades = listaEspecialidades.Items.Cast<ListItem>()
                    .Where(opcion => opcion.Selected)
                    .Select(opcion => new Especialidad { Id = int.Parse(opcion.Value) })
                    .ToList()
            };
            try
            {
                new MedicoNegocio().Guardar(medico);
                Session["Mensaje"] = id == 0 ? "Médico agregado correctamente." : "Médico modificado correctamente.";
                Response.Redirect("Medicos.aspx");
            }
            catch (ArgumentException ex)
            {
                lblMensaje.Text = Server.HtmlEncode(ex.Message);
            }
            catch (InvalidOperationException ex)
            {
                lblMensaje.Text = Server.HtmlEncode(ex.Message);
            }
            catch (SqlException)
            {
                lblMensaje.Text = "No se pudo guardar el médico.";
            }
        }

        private int IdEdicion
        {
            get
            {
                string valor = Request.QueryString["id"];
                if (valor == null)
                    return 0;
                int id;
                return int.TryParse(valor, out id) && id > 0 ? id : -1;
            }
        }
    }
}
