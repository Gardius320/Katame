namespace KatameApi.Services;

/// <summary>
/// Da acceso al Id del usuario autenticado en la request actual (extraído del
/// JWT). KatameDbContext lo usa para filtrar y para asignar el dueño en cada
/// entidad "propia" de un usuario (ver IUserOwned).
/// </summary>
public interface ICurrentUserService
{
    int UserId { get; }
}
