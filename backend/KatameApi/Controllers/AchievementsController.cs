using Microsoft.AspNetCore.Mvc;
using KatameApi.DTOs.Achievements;
using KatameApi.Services;

namespace KatameApi.Controllers;

[ApiController]
[Route("api/achievements")]
public class AchievementsController : ControllerBase
{
    private readonly IAchievementService _achievementService;

    public AchievementsController(IAchievementService achievementService)
    {
        _achievementService = achievementService;
    }

    [HttpGet]
    public async Task<ActionResult<List<AchievementDto>>> GetAll()
    {
        var achievements = await _achievementService.GetAllAsync();
        return Ok(achievements);
    }

    // El frontend llama esto después de acciones relevantes (aportar a una
    // meta, marcar un entrenamiento, abrir Presupuestos o la propia pantalla
    // de Logros) para revisar si algo se acaba de desbloquear. Devuelve solo
    // lo nuevo, para poder mostrar la celebración sin repetirla.
    [HttpPost("evaluate")]
    public async Task<ActionResult<List<AchievementDto>>> Evaluate()
    {
        var newlyUnlocked = await _achievementService.EvaluateAndUnlockAsync();
        return Ok(newlyUnlocked);
    }
}
