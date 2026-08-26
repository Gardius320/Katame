using System.Security.Claims;

namespace KatameApi.Services;

public class CurrentUserService : ICurrentUserService
{
    public int UserId { get; }

    public CurrentUserService(IHttpContextAccessor httpContextAccessor)
    {
        // Fuera de una request HTTP (por ejemplo, cuando `dotnet ef migrations add`
        // arranca la app para leer el modelo) no hay usuario autenticado. En ese
        // caso UserId queda en 0: no representa a nadie, pero evita que el arranque
        // falle por un NullReferenceException.
        var idClaim = httpContextAccessor.HttpContext?.User?.FindFirst(ClaimTypes.NameIdentifier);
        UserId = idClaim is not null && int.TryParse(idClaim.Value, out var id) ? id : 0;
    }
}
