using Microsoft.EntityFrameworkCore;
using SistemaRepository.Domain;
using SistemaRepositroy.Domain;

namespace SistemaAcademico.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {
        }

        // Una propiedad DbSet por cada tabla
        public DbSet<Usuario> Usuarios { get; set; }
        public DbSet<Alumno> Alumnos { get; set; }
        public DbSet<Profesor> Profesores { get; set; }
        public DbSet<Carrera> Carreras { get; set; }
        public DbSet<Materia> Materias { get; set; }
        public DbSet<Inscripcion> Inscripciones { get; set; }
        public DbSet<Nota> Notas { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // ---------------- USUARIOS ----------------
            modelBuilder.Entity<Usuario>(e =>
            {
                e.Property(u => u.Activo).HasDefaultValue(true);
                e.ToTable(t => t.HasCheckConstraint("CK_Usuarios_Rol", "Rol IN ('ALUMNO', 'ENCARGADO')"));

                // Solo puede existir un ENCARGADO
                e.HasIndex(u => u.Rol)
                 .IsUnique()
                 .HasFilter("[Rol] = 'ENCARGADO'")
                 .HasDatabaseName("UX_Usuarios_UnSoloEncargado");
            });

            // ---------------- ALUMNOS ----------------
            modelBuilder.Entity<Alumno>(e =>
            {
                // Legajo como clave primaria
                e.HasKey(a => a.Legajo);

                // Relación 1 a 1: cada alumno tiene un único usuario
                e.HasOne(a => a.Usuario)
                 .WithOne(u => u.Alumno)
                 .HasForeignKey<Alumno>(a => a.UsuarioId)
                 .OnDelete(DeleteBehavior.Restrict);
            });

            // ---------------- MATERIAS ----------------
            modelBuilder.Entity<Materia>(e =>
            {
                e.HasOne(m => m.Carrera)
                 .WithMany(c => c.Materias)
                 .HasForeignKey(m => m.CarreraId)
                 .OnDelete(DeleteBehavior.Restrict);

                e.HasOne(m => m.Profesor)
                 .WithMany(p => p.Materias)
                 .HasForeignKey(m => m.ProfesorId)
                 .OnDelete(DeleteBehavior.Restrict);
            });

            // ---------------- INSCRIPCIONES ----------------
            modelBuilder.Entity<Inscripcion>(e =>
            {
                e.Property(i => i.Fecha).HasDefaultValueSql("GETDATE()");

                e.HasOne(i => i.Alumno)
                 .WithMany(a => a.Inscripciones)
                 .HasForeignKey(i => i.Legajo)
                 .OnDelete(DeleteBehavior.Restrict);

                e.HasOne(i => i.Carrera)
                 .WithMany(c => c.Inscripciones)
                 .HasForeignKey(i => i.CarreraId)
                 .OnDelete(DeleteBehavior.Restrict);
            });

            // ---------------- NOTAS ----------------
            modelBuilder.Entity<Nota>(e =>
            {
                e.Property(n => n.Fecha).HasDefaultValueSql("GETDATE()");

                e.ToTable(t =>
                {
                    t.HasCheckConstraint("CK_Notas_Tipo", "Tipo IN ('PARCIAL 1', 'PARCIAL 2', 'RECUPERATORIO', 'FINAL')");
                    t.HasCheckConstraint("CK_Notas_Nota", "Nota BETWEEN 1 AND 10");
                });

                e.HasOne(n => n.Alumno)
                 .WithMany(a => a.Notas)
                 .HasForeignKey(n => n.Legajo)
                 .OnDelete(DeleteBehavior.Restrict);

                e.HasOne(n => n.Materia)
                 .WithMany(m => m.Notas)
                 .HasForeignKey(n => n.MateriaId)
                 .OnDelete(DeleteBehavior.Restrict);
            });
        }
    }
}