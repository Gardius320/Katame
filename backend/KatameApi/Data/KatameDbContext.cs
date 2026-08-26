using Microsoft.EntityFrameworkCore;
using KatameApi.Models;

namespace KatameApi.Data;

public class KatameDbContext : DbContext
{
    public KatameDbContext(DbContextOptions<KatameDbContext> options) : base(options)
    {
    }

    public DbSet<User> Users => Set<User>();
    public DbSet<TaskItem> Tasks => Set<TaskItem>();
    public DbSet<TrainingDay> TrainingDays => Set<TrainingDay>();
    public DbSet<Exercise> Exercises => Set<Exercise>();
    public DbSet<Transaction> Transactions => Set<Transaction>();
    public DbSet<SavingsGoal> SavingsGoals => Set<SavingsGoal>();
    public DbSet<Obligation> Obligations => Set<Obligation>();
    public DbSet<CreditCard> CreditCards => Set<CreditCard>();
    public DbSet<Subscription> Subscriptions => Set<Subscription>();
    public DbSet<Goal> Goals => Set<Goal>();
    public DbSet<Project> Projects => Set<Project>();
    public DbSet<Budget> Budgets => Set<Budget>();

    protected override void ConfigureConventions(ModelConfigurationBuilder configurationBuilder)
    {
        // decimal(18,2) por defecto para todo monto de dinero, en vez del decimal(65,30) de MySQL.
        configurationBuilder.Properties<decimal>().HavePrecision(18, 2);
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Query filter global de soft delete para toda entidad que herede de BaseEntity.
        foreach (var entityType in modelBuilder.Model.GetEntityTypes())
        {
            if (typeof(BaseEntity).IsAssignableFrom(entityType.ClrType))
            {
                var parameter = System.Linq.Expressions.Expression.Parameter(entityType.ClrType, "e");
                var property = System.Linq.Expressions.Expression.Property(parameter, nameof(BaseEntity.IsDeleted));
                var falseConstant = System.Linq.Expressions.Expression.Constant(false);
                var equalExpression = System.Linq.Expressions.Expression.Equal(property, falseConstant);
                var lambda = System.Linq.Expressions.Expression.Lambda(equalExpression, parameter);

                modelBuilder.Entity(entityType.ClrType).HasQueryFilter(lambda);
            }
        }

        modelBuilder.Entity<User>(entity =>
        {
            entity.HasIndex(u => u.Username).IsUnique();
            entity.HasIndex(u => u.Email).IsUnique();
            entity.HasIndex(u => u.DocumentId).IsUnique();
        });

        modelBuilder.Entity<User>().HasData(new User
        {
            Id = 1,
            Username = "admin",
            FirstName = "Admin",
            LastName = "Katame",
            DocumentId = "ADMIN-0001",
            PhoneNumber = "0000000000",
            Email = "admin@katame.local",
            // Hash BCrypt de una contraseña semilla SOLO para entornos locales/frescos
            // (ver README, sección de instalación). Deliberadamente NO es la contraseña
            // real de producción: esta línea es pública, así que la contraseña real se
            // rota directo en la base de datos (SQL), nunca se guarda acá ni en texto
            // plano ni como hash.
            PasswordHash = "$2a$11$ARlh7cu2CbBsZfvRSZl08.g.mUZm3QvsQGvlZzIHkpJeIgv6ozn5m",
            IsAdmin = true,
            CreatedAt = new DateTime(2026, 1, 1, 0, 0, 0, DateTimeKind.Utc)
        });

        modelBuilder.Entity<TrainingDay>()
            .HasMany(d => d.Exercises)
            .WithOne()
            .HasForeignKey(e => e.TrainingDayId)
            .OnDelete(DeleteBehavior.Cascade);

        // Vínculo opcional: no toda transacción es un gasto de tarjeta. Si se borra la
        // tarjeta, la transacción queda sin vínculo en vez de perderse (soft-delete aparte).
        modelBuilder.Entity<CreditCard>()
            .HasMany<Transaction>()
            .WithOne()
            .HasForeignKey(t => t.CreditCardId)
            .OnDelete(DeleteBehavior.SetNull);

        modelBuilder.Entity<TrainingDay>().HasData(
            new TrainingDay { Id = 1, DayOfWeek = DayOfWeek.Monday, Title = "Empuje" },
            new TrainingDay { Id = 2, DayOfWeek = DayOfWeek.Wednesday, Title = "Tirón" },
            new TrainingDay { Id = 3, DayOfWeek = DayOfWeek.Friday, Title = "Pierna" });

        modelBuilder.Entity<Exercise>().HasData(
            new Exercise { Id = 1, TrainingDayId = 1, Name = "Press banca", SetsReps = "4x8" },
            new Exercise { Id = 2, TrainingDayId = 1, Name = "Press militar", SetsReps = "3x10" },
            new Exercise { Id = 3, TrainingDayId = 2, Name = "Dominadas", SetsReps = "4x8" },
            new Exercise { Id = 4, TrainingDayId = 2, Name = "Remo con barra", SetsReps = "3x10" },
            new Exercise { Id = 5, TrainingDayId = 3, Name = "Sentadilla", SetsReps = "4x8" },
            new Exercise { Id = 6, TrainingDayId = 3, Name = "Peso muerto rumano", SetsReps = "3x10" });
    }
}
