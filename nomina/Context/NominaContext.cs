using Microsoft.EntityFrameworkCore;
using nomina.Entities;

namespace nomina.Context;

public class NominaContext(DbContextOptions<NominaContext> options) : DbContext(options)
{
    public virtual DbSet<AsientoContable> AsientoContable { get; set; }

    public virtual DbSet<Departamento> Departamento { get; set; }

    public virtual DbSet<Empleado> Empleado { get; set; }

    public virtual DbSet<Puesto> Puesto { get; set; }

    public virtual DbSet<TipoDeTransaccion> TipoDeTransaccion { get; set; }

    public virtual DbSet<Transaccion> Transaccion { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<AsientoContable>(entity => { entity.HasKey(e => e.Id).HasName("PRIMARY"); });

        modelBuilder.Entity<Departamento>(entity => { entity.HasKey(e => e.Id).HasName("PRIMARY"); });

        modelBuilder.Entity<Empleado>(entity => { entity.HasKey(e => e.Id).HasName("PRIMARY"); });

        modelBuilder.Entity<Puesto>(entity => { entity.HasKey(e => e.Id).HasName("PRIMARY"); });

        modelBuilder.Entity<TipoDeTransaccion>(entity => { entity.HasKey(e => e.Id).HasName("PRIMARY"); });

        modelBuilder.Entity<Transaccion>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PRIMARY");

            entity.Property(e => e.Fecha).HasDefaultValueSql("CURRENT_TIMESTAMP");
        });
    }
}