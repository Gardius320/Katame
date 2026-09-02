using FluentValidation;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;
using KatameApi.DTOs.Auth;
using KatameApi.Services;

namespace KatameApi.Controllers;

[ApiController]
[Route("api/auth")]
[AllowAnonymous]
public class AuthController : ControllerBase
{
    private readonly IAuthService _authService;
    private readonly IValidator<RegisterRequestDto> _registerValidator;
    private readonly IValidator<LoginRequestDto> _loginValidator;
    private readonly IValidator<RefreshRequestDto> _refreshValidator;
    private readonly IValidator<ForgotPasswordRequestDto> _forgotPasswordValidator;
    private readonly IValidator<ResetPasswordRequestDto> _resetPasswordValidator;

    public AuthController(
        IAuthService authService,
        IValidator<RegisterRequestDto> registerValidator,
        IValidator<LoginRequestDto> loginValidator,
        IValidator<RefreshRequestDto> refreshValidator,
        IValidator<ForgotPasswordRequestDto> forgotPasswordValidator,
        IValidator<ResetPasswordRequestDto> resetPasswordValidator)
    {
        _authService = authService;
        _registerValidator = registerValidator;
        _loginValidator = loginValidator;
        _refreshValidator = refreshValidator;
        _forgotPasswordValidator = forgotPasswordValidator;
        _resetPasswordValidator = resetPasswordValidator;
    }

    [HttpPost("register")]
    [EnableRateLimiting("register")]
    public async Task<ActionResult<AuthResponseDto>> Register(RegisterRequestDto request)
    {
        await _registerValidator.ValidateAndThrowAsync(request);
        var result = await _authService.RegisterAsync(request);
        return Ok(result);
    }

    [HttpPost("login")]
    [EnableRateLimiting("login")]
    public async Task<ActionResult<AuthResponseDto>> Login(LoginRequestDto request)
    {
        await _loginValidator.ValidateAndThrowAsync(request);
        var result = await _authService.LoginAsync(request);
        return Ok(result);
    }

    [HttpPost("refresh")]
    public async Task<ActionResult<AuthResponseDto>> Refresh(RefreshRequestDto request)
    {
        await _refreshValidator.ValidateAndThrowAsync(request);
        var result = await _authService.RefreshAsync(request);
        return Ok(result);
    }

    [HttpPost("forgot-password")]
    [EnableRateLimiting("password-reset")]
    public async Task<IActionResult> ForgotPassword(ForgotPasswordRequestDto request)
    {
        await _forgotPasswordValidator.ValidateAndThrowAsync(request);
        await _authService.ForgotPasswordAsync(request);
        // Misma respuesta exista o no el correo, para no filtrar qué cuentas están registradas.
        return Ok(new { message = "Si el correo está registrado, vas a recibir un enlace para restablecer tu contraseña." });
    }

    [HttpPost("reset-password")]
    [EnableRateLimiting("password-reset")]
    public async Task<IActionResult> ResetPassword(ResetPasswordRequestDto request)
    {
        await _resetPasswordValidator.ValidateAndThrowAsync(request);
        await _authService.ResetPasswordAsync(request);
        return Ok(new { message = "Tu contraseña se actualizó correctamente." });
    }

    [HttpPost("logout")]
    public async Task<IActionResult> Logout(RefreshRequestDto request)
    {
        await _refreshValidator.ValidateAndThrowAsync(request);
        await _authService.LogoutAsync(request);
        return NoContent();
    }
}
