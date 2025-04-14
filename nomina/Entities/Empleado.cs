using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;
using nomina.Attributes;

namespace nomina.Entities;

[Table("empleado")]
[Index("Cedula", Name = "cedula", IsUnique = true)]
public class Empleado
{
    [Key] [Column("id")] public int Id { get; set; }

    [Required(ErrorMessage = "Este campo es obligatorio.", AllowEmptyStrings = false)]
    [DominicanIdCard]
    [Column("cedula")]
    public string Cedula { get; set; } = null!;

    [Required(ErrorMessage = "Este campo es obligatorio.", AllowEmptyStrings = false)]
    [Column("nombre")]
    [StringLength(36)]
    public string Nombre { get; set; } = null!;

    [Required(ErrorMessage = "Este campo es obligatorio.")]
    [Column("departamento")]
    public int DepartamentoId { get; set; }

    [Required(ErrorMessage = "Este campo es obligatorio.")]
    [Column("puesto")]
    public int PuestoId { get; set; }

    [Required(ErrorMessage = "Este campo es obligatorio.")]
    [Column("salario_mensual")]
    [Precision(10)]
    public decimal SalarioMensual { get; set; }
    
    public Departamento? Departamento { get; set; }
    
    public Puesto? Puesto { get; set; }
}