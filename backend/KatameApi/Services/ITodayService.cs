using KatameApi.DTOs.Today;

namespace KatameApi.Services;

public interface ITodayService
{
    Task<TodayDto> GetTodayAsync();
}
