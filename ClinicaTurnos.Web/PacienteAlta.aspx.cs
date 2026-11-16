using System;
using System.Data.SqlClient;
using System.Globalization;
using ClinicaTurnos.Dominio;
using ClinicaTurnos.Negocio;

namespace ClinicaTurnos.Web
{
    public partial class PacienteAlta : PaginaBase
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            lblMensaje.Text = "";
            txtFechaNacimiento.Attributes["max"] = DateTime.Today.ToString("yyyy-MM-dd");
        }

        protected void btnGuardar_Click(object sender, EventArgs e)
        {
            Page.Validate();
            if (!Page.IsValid) return;
            DateTime fecha;
            if (!DateTime.TryParseExact(txtFechaNacimiento.Text, "yyyy-MM-dd", CultureInfo.InvariantCulture, DateTimeStyles.None, out fecha))
            {
                lblMensaje.Text = "Ingresá una fecha de nacimiento válida.";
                return;
            }

            Paciente dato = new Paciente
            {
                Dni = txtDni.Text,
                Nombre = txtNombre.Text,
                Apellido = txtApellido.Text,
                FechaNacimiento = fecha,
                Telefono = txtTelefono.Text,
                Email = txtEmail.Text,
                Direccion = txtDireccion.Text
            };
            try
            {
                new PacienteNegocio().Agregar(dato);
                Session["Mensaje"] = "Registro agregado correctamente.";
                Response.Redirect("Pacientes.aspx");
            }
            catch (ArgumentException ex)
            {
                lblMensaje.Text = Server.HtmlEncode(ex.Message);
            }
            catch (SqlException)
            {
                lblMensaje.Text = "No se pudieron guardar los cambios. Revisá la base de datos e intentá nuevamente.";
            }
        }
    }
}
