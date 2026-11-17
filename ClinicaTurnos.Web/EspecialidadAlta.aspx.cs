using System;
using System.Data.SqlClient;
using ClinicaTurnos.Dominio;
using ClinicaTurnos.Negocio;

namespace ClinicaTurnos.Web
{
    public partial class EspecialidadAlta : PaginaBase
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            lblMensaje.Text = "";
            int id = IdEdicion;

            if (id < 0)
            {
                lblMensaje.Text = "La especialidad seleccionada no es válida.";
                btnGuardar.Enabled = false;
                return;
            }

            if (id == 0)
            {
                litTitulo.Text = "Agregar especialidad";
                btnGuardar.Text = "Guardar";
                return;
            }

            litTitulo.Text = "Modificar especialidad";
            btnGuardar.Text = "Guardar cambios";

            if (!IsPostBack)
            {
                try
                {
                    Especialidad especialidad = new EspecialidadNegocio().Obtener(id);
                    if (especialidad == null)
                    {
                        lblMensaje.Text = "No se encontró la especialidad seleccionada.";
                        btnGuardar.Enabled = false;
                        return;
                    }

                    txtNombre.Text = especialidad.Nombre;
                }
                catch (SqlException)
                {
                    lblMensaje.Text = "No se pudo cargar la especialidad. Revisá la conexión a la base de datos.";
                    btnGuardar.Enabled = false;
                }
            }
        }

        protected void btnGuardar_Click(object sender, EventArgs e)
        {
            Page.Validate();
            if (!Page.IsValid)
            {
                return;
            }

            int id = IdEdicion;
            if (id < 0)
            {
                lblMensaje.Text = "La especialidad seleccionada no es válida.";
                return;
            }

            Especialidad dato = new Especialidad
            {
                Id = id,
                Nombre = txtNombre.Text
            };

            try
            {
                EspecialidadNegocio negocio = new EspecialidadNegocio();
                if (id == 0)
                {
                    negocio.Agregar(dato);
                    Session["Mensaje"] = "Especialidad agregada correctamente.";
                }
                else
                {
                    negocio.Modificar(dato);
                    Session["Mensaje"] = "Especialidad modificada correctamente.";
                }

                Response.Redirect("Especialidades.aspx");
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

        private int IdEdicion
        {
            get
            {
                string valor = Request.QueryString["id"];
                if (String.IsNullOrWhiteSpace(valor))
                {
                    return 0;
                }

                int id;
                if (!int.TryParse(valor, out id) || id <= 0)
                {
                    return -1;
                }

                return id;
            }
        }
    }
}
