using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace GestionTurnos.Models;

public partial class ApplicationDbContext : DbContext
{
    public ApplicationDbContext()
    {
    }

    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Cliente> Clientes { get; set; }

    public virtual DbSet<DetalleTurno> DetalleTurnos { get; set; }

    public virtual DbSet<Servicio> Servicios { get; set; }

    public virtual DbSet<Turno> Turnos { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        if (!optionsBuilder.IsConfigured)
        {
            IConfigurationRoot configuration = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json")
                .Build();

            optionsBuilder.UseSqlServer(configuration.GetConnectionString("MyDatabaseConnection"));
        }
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Cliente>(entity =>
        {
            entity.HasKey(e => e.Idcliente).HasName("PK__Cliente__9B8553FCB6C85E61");

            entity.ToTable("Cliente");

            entity.Property(e => e.Idcliente).HasColumnName("IDCliente");
            entity.Property(e => e.Apellido)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.CorreoElectronico)
                .HasMaxLength(100)
                .IsUnicode(false);
            entity.Property(e => e.Nombre)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.Telefono)
                .HasMaxLength(15)
                .IsUnicode(false);
        });

        modelBuilder.Entity<DetalleTurno>(entity =>
        {
            entity.HasKey(e => e.IdDetalleTurno).HasName("PK__DetalleT__616F590584EA821D");

            entity.ToTable("DetalleTurno");

            entity.Property(e => e.Idservicio).HasColumnName("IDServicio");
            entity.Property(e => e.Idturno).HasColumnName("IDTurno");
            entity.Property(e => e.Importe).HasColumnType("decimal(10, 2)");
        });

        modelBuilder.Entity<Servicio>(entity =>
        {
            entity.HasKey(e => e.IDServicio).HasName("PK__Servicio__3214EC273049877F");

            entity.Property(e => e.IDServicio)
                .ValueGeneratedNever()
                .HasColumnName("IDServicio");
            entity.Property(e => e.Costo).HasColumnType("decimal(10, 2)");
            entity.Property(e => e.Descripcion)
                .HasMaxLength(255)
                .IsUnicode(false);
            entity.Property(e => e.Nombre)
                .HasMaxLength(100)
                .IsUnicode(false);
        });

        modelBuilder.Entity<Turno>(entity =>
        {
            entity.HasKey(e => e.Idturno).HasName("PK__Turnos__9763AA1E655FEA22");

            entity.Property(e => e.Idturno)
                .ValueGeneratedNever()
                .HasColumnName("IDTurno");
            entity.Property(e => e.Estado)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.IdDetalleTurno).ValueGeneratedOnAdd();
            entity.Property(e => e.Idcliente).HasColumnName("IDCliente");
            entity.Property(e => e.Idservicio).HasColumnName("IDServicio");

            entity.HasOne(d => d.IdclienteNavigation).WithMany(p => p.Turnos)
                .HasForeignKey(d => d.Idcliente)
                .HasConstraintName("FK__Turnos__IDClient__3E52440B");

            //entity.HasOne(d => d.IdservicioNavigation).WithMany(p => p.Turnos)
            //    .HasForeignKey(d => d.Idservicio)
            //    .HasConstraintName("FK__Turnos__IDServic__3F466844");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
