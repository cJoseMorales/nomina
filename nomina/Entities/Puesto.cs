using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace nomina.Entities;

[Table("puesto")]
[Index("Nombre", Name = "nombre", IsUnique = true)]
public class Puesto
{
    [Key] [Column("id")] public int Id { get; set; }

    [Required(ErrorMessage = "Este campo es obligatorio.", AllowEmptyStrings = false)]
    [Column("nombre")] [StringLength(36)] public string Nombre { get; set; } = null!;

    [Column("descripcion", TypeName = "text")]
    public string? Descripcion { get; set; }
}