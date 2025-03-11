using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace nomina.Entities;

[Table("tipo_de_transaccion")]
[Index("Nombre", Name = "nombre", IsUnique = true)]
public class TipoDeTransaccion
{
    [Key] [Column("id")] public int Id { get; set; }

    [Required(ErrorMessage = "Este campo es obligatorio.", AllowEmptyStrings = false)]
    [Column("nombre")] [StringLength(36)] public string Nombre { get; set; } = null!;

    [Required(ErrorMessage = "Este campo es obligatorio.")]
    [Column("operacion", TypeName = "enum('INGRESO','DEDUCCION')")]
    public string Operacion { get; set; } = null!;

    [Required(ErrorMessage = "Este campo es obligatorio.")]
    [Column("depende_de_salario")] public bool DependeDeSalario { get; set; }

    [Required(ErrorMessage = "Este campo es obligatorio.")]
    [Column("estado", TypeName = "enum('ACTIVO','INACTIVO')")]
    public string Estado { get; set; } = null!;
}