namespace KatameApi.Models;

/// <summary>
/// Marca las entidades que pertenecen a un usuario en particular (tareas,
/// transacciones, metas, tarjetas, etc.). KatameDbContext usa esta interfaz
/// para aplicar automáticamente un filtro global de "solo lo mío" a toda
/// consulta sobre estas entidades, y para asignar el dueño automáticamente
/// al crear una fila nueva (ver OnModelCreating y SaveChangesAsync).
/// </summary>
public interface IUserOwned
{
    int UserId { get; set; }
}
