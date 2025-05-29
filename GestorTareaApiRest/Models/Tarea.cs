namespace GestorTareaApiRest.Models
{
    public class Tarea
    {
        public int Id { get; set; }
        public string Titulo { get; set; }
        public string Descripcion { get; set; }
        public bool Completado { get; set; }
        public int UsuarioId { get; set; }
    }
}
