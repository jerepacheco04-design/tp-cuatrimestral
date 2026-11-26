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
            int id = IdEdicion;
            if (id < 0)
            {
                lblMensaje.Text = "El paciente seleccionado no es válido.";
                btnGuardar.Enabled = false;
                return;
            }
            if (id == 0) return;
            litTitulo.Text = "Modificar paciente";
            btnGuardar.Text = "Guardar cambios";
            if (!IsPostBack)
            {
                try
                {
                    Paciente paciente = new PacienteNegocio().Obtener(id);
                    if (paciente == null)
                    {
                        lblMensaje.Text = "El paciente ya no existe. Volvé al listado.";
                        btnGuardar.Enabled = false;
                        return;
                    }
                    txtDni.Text = paciente.Dni;
                    txtNombre.Text = paciente.Nombre;
                    txtApellido.Text = paciente.Apellido;
                    txtFechaNacimiento.Text = paciente.FechaNacimiento.ToString("yyyy-MM-dd");
                    txtTelefono.Text = paciente.Telefono;
                    txtEmail.Text = paciente.Email;
                    txtDireccion.Text = paciente.Direccion;
                }
                catch (SqlException)
                {
                    lblMensaje.Text = "No se pudo cargar el paciente.";
                    btnGuardar.Enabled = false;
                }
            }
        }

        protected void btnGuardar_Click(object sender, EventArgs e)
        {
            int id = IdEdicion;
            if (id < 0 || !btnGuardar.Enabled) return;
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
                Id = id,
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
                PacienteNegocio negocio = new PacienteNegocio();
                if (id == 0)
                {
                    negocio.Agregar(dato);
                    Session["Mensaje"] = "Registro agregado correctamente.";
                }
                else
                {
                    negocio.Modificar(dato);
                    Session["Mensaje"] = "Registro modificado correctamente.";
                }
                Response.Redirect("Pacientes.aspx");
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
                lblMensaje.Text = "No se pudieron guardar los cambios.";
            }
        }

        private int IdEdicion
        {
            get
            {
                string valor = Request.QueryString["id"];
                if (valor == null) return 0;
                int id;
                return int.TryParse(valor, out id) && id > 0 ? id : -1;
            }
        }
    }
}
