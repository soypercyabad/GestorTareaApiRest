namespace GestorTareaApiRest.Models
{
    public class Usuario
    {
        public int Id { get; set; }
        public string Nombre { get; set; }
        public string ApellidoPaterno { get; set; }
        public string ApellidoMaterno { get; set; }
        public string NombreUsuario { get; set; }
        public string ClaveSecreta { get; set; }
        public List<Tarea> Tareas { get; set; }
    }
}
