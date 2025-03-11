using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace nomina.Entities;

[Table("empleado")]
[Index("Cedula", Name = "cedula", IsUnique = true)]
public class Empleado
{
    [Key] [Column("id")] public int Id { get; set; }

    [Column("cedula")] [StringLength(11)] public string Cedula { get; set; } = null!;

    [Column("nombre")] [StringLength(36)] public string Nombre { get; set; } = null!;

    [Column("departamento")] public int Departamento { get; set; }

    [Column("puesto")] public int Puesto { get; set; }

    [Column("salario_mensual")]
    [Precision(10)]
    public decimal SalarioMensual { get; set; }
}