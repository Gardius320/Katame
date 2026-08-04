using Microsoft.AspNetCore.Mvc;
using KatameApi.DTOs.Today;
using KatameApi.Services;

namespace KatameApi.Controllers;

[ApiController]
[Route("api/today")]
public class TodayController : ControllerBase
{
    private readonly ITodayService _todayService;

    public TodayController(ITodayService todayService)
    {
        _todayService = todayService;
    }

    [HttpGet]
    public async Task<ActionResult<TodayDto>> Get()
    {
        var today = await _todayService.GetTodayAsync();
        return Ok(today);
    }
}
