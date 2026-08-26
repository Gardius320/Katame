using Microsoft.EntityFrameworkCore;
using KatameApi.Models;
using KatameApi.Services;

namespace KatameApi.Data;

public class KatameDbContext : DbContext
{
    private readonly ICurrentUserService _currentUserService;

    public KatameDbContext(DbContextOptions<KatameDbContext> options, ICurrentUserService currentUserService)
        : base(options)
    {
        _currentUserService = currentUserService;
    }

    /// <summary>
    /// Id del usuario autenticado en la request actual. Se usa en el filtro
    /// global de aislamiento por usuario (ver OnModelCreating) y para asignar
    /// el dueño automáticamente al guardar una entidad nueva (ver
    /// SaveChangesAsync). Es una propiedad de instancia -- no un valor fijo --
    /// para que el mismo modelo (compilado una sola vez) sirva para todas las
    /// requests, cada una con su propio usuario.
    /// </summary>
    public int CurrentUserId => _currentUserService.UserId;

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

        // Filtro global combinado: soft delete (toda entidad que hereda de BaseEntity)
        // + aislamiento por usuario (toda entidad que implementa IUserOwned). Se arma
        // dinámicamente para no repetir la misma configuración en cada entidad -- si
        // mañana se agrega una entidad nueva, con solo heredar/implementar ya queda
        // protegida automáticamente.
        foreach (var entityType in modelBuilder.Model.GetEntityTypes())
        {
            var clrType = entityType.ClrType;
            var parameter = System.Linq.Expressions.Expression.Parameter(clrType, "e");
            System.Linq.Expressions.Expression? filter = null;

            if (typeof(BaseEntity).IsAssignableFrom(clrType))
            {
                var isDeletedProperty = System.Linq.Expressions.Expression.Property(parameter, nameof(BaseEntity.IsDeleted));
                filter = System.Linq.Expressions.Expression.Equal(isDeletedProperty, System.Linq.Expressions.Expression.Constant(false));
            }

            if (typeof(IUserOwned).IsAssignableFrom(clrType))
            {
                // e.UserId == this.CurrentUserId -- al referenciar una propiedad de la
                // instancia del contexto (no un valor fijo), EF Core evalúa esta parte
                // del filtro con el usuario de cada request, no con uno solo grabado
                // al armar el modelo.
                var userIdProperty = System.Linq.Expressions.Expression.Property(parameter, nameof(IUserOwned.UserId));
                var currentUserId = System.Linq.Expressions.Expression.Property(
                    System.Linq.Expressions.Expression.Constant(this), nameof(CurrentUserId));
                var ownedByCurrentUser = System.Linq.Expressions.Expression.Equal(userIdProperty, currentUserId);

                filter = filter is null ? ownedByCurrentUser : System.Linq.Expressions.Expression.AndAlso(filter, ownedByCurrentUser);
            }

            if (filter is not null)
            {
                var lambda = System.Linq.Expressions.Expression.Lambda(filter, parameter);
                modelBuilder.Entity(clrType).HasQueryFilter(lambda);
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

        // Cada entidad "propia" de un usuario apunta a su dueño en Users. Si se borra
        // el usuario, se borra en cascada todo lo suyo (tareas, transacciones, etc.).
        modelBuilder.Entity<Budget>().HasOne<User>().WithMany().HasForeignKey(b => b.UserId).OnDelete(DeleteBehavior.Cascade);
        modelBuilder.Entity<CreditCard>().HasOne<User>().WithMany().HasForeignKey(c => c.UserId).OnDelete(DeleteBehavior.Cascade);
        modelBuilder.Entity<Goal>().HasOne<User>().WithMany().HasForeignKey(g => g.UserId).OnDelete(DeleteBehavior.Cascade);
        modelBuilder.Entity<Obligation>().HasOne<User>().WithMany().HasForeignKey(o => o.UserId).OnDelete(DeleteBehavior.Cascade);
        modelBuilder.Entity<Project>().HasOne<User>().WithMany().HasForeignKey(p => p.UserId).OnDelete(DeleteBehavior.Cascade);
        modelBuilder.Entity<SavingsGoal>().HasOne<User>().WithMany().HasForeignKey(s => s.UserId).OnDelete(DeleteBehavior.Cascade);
        modelBuilder.Entity<Subscription>().HasOne<User>().WithMany().HasForeignKey(s => s.UserId).OnDelete(DeleteBehavior.Cascade);
        modelBuilder.Entity<TaskItem>().HasOne<User>().WithMany().HasForeignKey(t => t.UserId).OnDelete(DeleteBehavior.Cascade);
        modelBuilder.Entity<TrainingDay>().HasOne<User>().WithMany().HasForeignKey(d => d.UserId).OnDelete(DeleteBehavior.Cascade);
        modelBuilder.Entity<Transaction>().HasOne<User>().WithMany().HasForeignKey(t => t.UserId).OnDelete(DeleteBehavior.Cascade);

        // Plantilla inicial de entrenamiento: se sigue sembrando para el usuario admin
        // (Id = 1). Cada usuario nuevo arranca sin días de entrenamiento y arma los
        // suyos desde cero.
        modelBuilder.Entity<TrainingDay>().HasData(
            new TrainingDay { Id = 1, UserId = 1, DayOfWeek = DayOfWeek.Monday, Title = "Empuje" },
            new TrainingDay { Id = 2, UserId = 1, DayOfWeek = DayOfWeek.Wednesday, Title = "Tirón" },
            new TrainingDay { Id = 3, UserId = 1, DayOfWeek = DayOfWeek.Friday, Title = "Pierna" });

        modelBuilder.Entity<Exercise>().HasData(
            new Exercise { Id = 1, TrainingDayId = 1, Name = "Press banca", SetsReps = "4x8" },
            new Exercise { Id = 2, TrainingDayId = 1, Name = "Press militar", SetsReps = "3x10" },
            new Exercise { Id = 3, TrainingDayId = 2, Name = "Dominadas", SetsReps = "4x8" },
            new Exercise { Id = 4, TrainingDayId = 2, Name = "Remo con barra", SetsReps = "3x10" },
            new Exercise { Id = 5, TrainingDayId = 3, Name = "Sentadilla", SetsReps = "4x8" },
            new Exercise { Id = 6, TrainingDayId = 3, Name = "Peso muerto rumano", SetsReps = "3x10" });
    }

    public override int SaveChanges(bool acceptAllChangesOnSuccess)
    {
        StampCurrentUserOnNewEntities();
        return base.SaveChanges(acceptAllChangesOnSuccess);
    }

    public override Task<int> SaveChangesAsync(bool acceptAllChangesOnSuccess, CancellationToken cancellationToken = default)
    {
        StampCurrentUserOnNewEntities();
        return base.SaveChangesAsync(acceptAllChangesOnSuccess, cancellationToken);
    }

    /// <summary>
    /// Asigna automáticamente el dueño de toda entidad "propia" de un usuario
    /// que se está creando (UserId = 0, el default cuando el código que la
    /// arma en el Service nunca lo tocó). Así ningún Service necesita acordarse
    /// de setear el UserId a mano -- basta con implementar IUserOwned.
    /// </summary>
    private void StampCurrentUserOnNewEntities()
    {
        foreach (var entry in ChangeTracker.Entries<IUserOwned>())
        {
            if (entry.State == EntityState.Added && entry.Entity.UserId == 0)
            {
                entry.Entity.UserId = CurrentUserId;
            }
        }
    }
}
