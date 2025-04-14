using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace nomina.Entities;

[Table("transaccion")]
public class Transaccion
{
    [Key] [Column("id")] public int Id { get; set; }

    [Required(ErrorMessage = "Este campo es obligatorio.")]
    [Column("empleado")] public int EmpleadoId { get; set; }

    [Required(ErrorMessage = "Este campo es obligatorio.")]
    [Column("tipo")] public int TipoDeTransaccionId { get; set; }

    [Required(ErrorMessage = "Este campo es obligatorio.")]
    [Column("fecha", TypeName = "timestamp")]
    public DateTime Fecha { get; set; }

    [Required(ErrorMessage = "Este campo es obligatorio.")]
    [Column("monto")] [Precision(10)] public decimal Monto { get; set; }
    
    [Column("id_asiento")] public int? IdAsiento { get; set; }

    [Required(ErrorMessage = "Este campo es obligatorio.")]
    [Column("estado", TypeName = "enum('ACTIVO','INACTIVO')")]
    public string Estado { get; set; } = null!;
    
    public Empleado? Empleado { get; set; }
    
    public TipoDeTransaccion? TipoDeTransaccion { get; set; }
}