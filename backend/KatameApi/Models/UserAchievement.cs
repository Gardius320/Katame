namespace KatameApi.Models;

// Un logro desbloqueado por el usuario. El catálogo de logros posibles (qué
// existe, cómo se llama, qué hay que hacer para desbloquearlo) vive en código
// -- ver AchievementCatalog -- así que esta tabla solo guarda CUÁLES ya se
// desbloquearon y CUÁNDO, una fila por logro por usuario.
public class UserAchievement : IUserOwned
{
    public int Id { get; set; }
    public int UserId { get; set; }
    public string Key { get; set; } = string.Empty;
    public DateTime UnlockedAt { get; set; }
}
