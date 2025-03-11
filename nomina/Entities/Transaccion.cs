using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace nomina.Entities;

[Table("transaccion")]
public class Transaccion
{
    [Key] [Column("id")] public int Id { get; set; }

    [Column("empleado")] public int Empleado { get; set; }

    [Column("tipo")] public int Tipo { get; set; }

    [Column("fecha", TypeName = "timestamp")]
    public DateTime Fecha { get; set; }

    [Column("monto")] [Precision(10)] public decimal Monto { get; set; }

    [Column("estado", TypeName = "enum('ACTIVO','INACTIVO')")]
    public string Estado { get; set; } = null!;
}