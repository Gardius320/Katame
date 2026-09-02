using FluentValidation;
using Microsoft.AspNetCore.Mvc;
using KatameApi.DTOs.Training;
using KatameApi.Services;

namespace KatameApi.Controllers;

[ApiController]
[Route("api/training")]
public class TrainingController : ControllerBase
{
    private readonly ITrainingService _trainingService;
    private readonly IValidator<CreateTrainingDayDto> _createDayValidator;
    private readonly IValidator<UpdateTrainingDayDto> _updateDayValidator;
    private readonly IValidator<CreateExerciseDto> _createExerciseValidator;
    private readonly IValidator<UpdateExerciseDto> _updateExerciseValidator;

    public TrainingController(
        ITrainingService trainingService,
        IValidator<CreateTrainingDayDto> createDayValidator,
        IValidator<UpdateTrainingDayDto> updateDayValidator,
        IValidator<CreateExerciseDto> createExerciseValidator,
        IValidator<UpdateExerciseDto> updateExerciseValidator)
    {
        _trainingService = trainingService;
        _createDayValidator = createDayValidator;
        _updateDayValidator = updateDayValidator;
        _createExerciseValidator = createExerciseValidator;
        _updateExerciseValidator = updateExerciseValidator;
    }

    [HttpGet("days")]
    public async Task<ActionResult<List<TrainingDayDto>>> GetAllDays()
    {
        var days = await _trainingService.GetAllDaysAsync();
        return Ok(days);
    }

    [HttpPost("days")]
    public async Task<ActionResult<TrainingDayDto>> CreateDay(CreateTrainingDayDto request)
    {
        await _createDayValidator.ValidateAndThrowAsync(request);
        var day = await _trainingService.CreateDayAsync(request);
        return CreatedAtAction(nameof(GetAllDays), new { id = day.Id }, day);
    }

    [HttpPut("days/{id:int}")]
    public async Task<ActionResult<TrainingDayDto>> UpdateDay(int id, UpdateTrainingDayDto request)
    {
        await _updateDayValidator.ValidateAndThrowAsync(request);
        var day = await _trainingService.UpdateDayAsync(id, request);
        return Ok(day);
    }

    [HttpDelete("days/{id:int}")]
    public async Task<IActionResult> DeleteDay(int id)
    {
        await _trainingService.DeleteDayAsync(id);
        return NoContent();
    }

    [HttpPost("days/{dayId:int}/exercises")]
    public async Task<ActionResult<ExerciseDto>> AddExercise(int dayId, CreateExerciseDto request)
    {
        await _createExerciseValidator.ValidateAndThrowAsync(request);
        var exercise = await _trainingService.AddExerciseAsync(dayId, request);
        return CreatedAtAction(nameof(GetAllDays), new { id = exercise.Id }, exercise);
    }

    [HttpPut("days/{dayId:int}/exercises/{exerciseId:int}")]
    public async Task<ActionResult<ExerciseDto>> UpdateExercise(int dayId, int exerciseId, UpdateExerciseDto request)
    {
        await _updateExerciseValidator.ValidateAndThrowAsync(request);
        var exercise = await _trainingService.UpdateExerciseAsync(dayId, exerciseId, request);
        return Ok(exercise);
    }

    [HttpDelete("days/{dayId:int}/exercises/{exerciseId:int}")]
    public async Task<IActionResult> DeleteExercise(int dayId, int exerciseId)
    {
        await _trainingService.DeleteExerciseAsync(dayId, exerciseId);
        return NoContent();
    }

    [HttpGet("streak")]
    public async Task<ActionResult<TrainingStreakDto>> GetStreak()
    {
        var streak = await _trainingService.GetStreakAsync();
        return Ok(streak);
    }

    [HttpPost("completions")]
    public async Task<ActionResult<TrainingStreakDto>> MarkTodayCompleted()
    {
        var streak = await _trainingService.MarkTodayCompletedAsync();
        return Ok(streak);
    }
}
