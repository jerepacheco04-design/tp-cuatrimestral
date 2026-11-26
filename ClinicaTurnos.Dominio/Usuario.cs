namespace ClinicaTurnos.Dominio
{
    public class Usuario
    {
        public int Id { get; set; }
        public string NombreUsuario { get; set; }
        public string Nombre { get; set; }
        public string Apellido { get; set; }
        public string Email { get; set; }
        public PerfilUsuario Perfil { get; set; }
        public int? MedicoId { get; set; }
        public bool Activo { get; set; }
    }
}
