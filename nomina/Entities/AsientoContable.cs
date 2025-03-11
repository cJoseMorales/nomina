using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace nomina.Entities;

[Table("asiento_contable")]
[Index("Periodo", Name = "periodo", IsUnique = true)]
public class AsientoContable
{
    [Key] [Column("id")] public int Id { get; set; }

    [Column("descripcion", TypeName = "text")]
    public string? Descripcion { get; set; }

    [Column("cuenta")] [StringLength(36)] public string Cuenta { get; set; } = null!;

    [Column("tipo_de_movimiento", TypeName = "enum('DEBITO','CREDITO')")]
    public string TipoDeMovimiento { get; set; } = null!;

    [Column("periodo")] [StringLength(7)] public string Periodo { get; set; } = null!;

    [Column("monto")] [Precision(10)] public decimal Monto { get; set; }

    [Column("estado", TypeName = "enum('ACTIVO','INACTIVO')")]
    public string Estado { get; set; } = null!;
}