using System.Net;
using Microsoft.Extensions.Options;
using KatameApi.DTOs.Auth;
using KatameApi.Middleware;
using KatameApi.Services;

namespace KatameApi.Tests.Services;

public class AuthServiceTests
{
    private static AuthService CreateService(out FakeUserRepository repository) =>
        CreateService(out repository, out _);

    private static AuthService CreateService(out FakeUserRepository repository, out FakeEmailService emailService)
    {
        repository = new FakeUserRepository();
        emailService = new FakeEmailService();
        var tokenService = new TokenService(Options.Create(new JwtSettings
        {
            Key = "test-signing-key-at-least-32-characters-long",
            Issuer = "KatameApi.Tests",
            Audience = "KatameApi.Tests",
            AccessTokenExpiryMinutes = 20,
            RefreshTokenExpiryDays = 7,
        }));
        return new AuthService(
            repository,
            tokenService,
            emailService,
            Options.Create(new JwtSettings { RefreshTokenExpiryDays = 7 }),
            Options.Create(new FrontendSettings { BaseUrl = "http://localhost:5173" }));
    }

    private static RegisterRequestDto SampleRegister(
        string email = "ana@correo.com",
        string documentId = "1701234567") => new()
    {
        FirstName = "Ana",
        LastName = "Pérez",
        DocumentId = documentId,
        Email = email,
        PhoneNumber = "0991234567",
        Password = "Password123!",
    };

    [Fact]
    public async Task RegisterAsync_usa_el_correo_completo_como_username()
    {
        var service = CreateService(out _);

        var result = await service.RegisterAsync(SampleRegister(email: "juan.perez@correo.com"));

        Assert.Equal("juan.perez@correo.com", result.Username);
    }

    [Fact]
    public async Task RegisterAsync_devuelve_el_firstName_junto_con_los_tokens()
    {
        var service = CreateService(out _);

        var result = await service.RegisterAsync(SampleRegister());

        Assert.Equal("Ana", result.FirstName);
        Assert.False(string.IsNullOrEmpty(result.AccessToken));
        Assert.False(string.IsNullOrEmpty(result.RefreshToken));
    }

    [Fact]
    public async Task RegisterAsync_guarda_los_datos_personales_del_usuario()
    {
        var service = CreateService(out var repository);

        await service.RegisterAsync(SampleRegister());

        var stored = await repository.GetByUsernameAsync("ana@correo.com");
        Assert.NotNull(stored);
        Assert.Equal("Ana", stored!.FirstName);
        Assert.Equal("Pérez", stored.LastName);
        Assert.Equal("1701234567", stored.DocumentId);
        Assert.Equal("0991234567", stored.PhoneNumber);
        Assert.True(BCrypt.Net.BCrypt.Verify("Password123!", stored.PasswordHash));
    }

    [Fact]
    public async Task RegisterAsync_lanza_409_si_el_correo_ya_esta_en_uso()
    {
        var service = CreateService(out _);
        await service.RegisterAsync(SampleRegister(email: "repetido@correo.com", documentId: "1701234567"));

        var exception = await Assert.ThrowsAsync<ApiException>(() =>
            service.RegisterAsync(SampleRegister(email: "repetido@correo.com", documentId: "1712345675")));

        Assert.Equal(HttpStatusCode.Conflict, exception.StatusCode);
    }

    [Fact]
    public async Task RegisterAsync_lanza_409_si_la_cedula_ya_esta_registrada()
    {
        var service = CreateService(out _);
        await service.RegisterAsync(SampleRegister(email: "uno@correo.com", documentId: "1701234567"));

        var exception = await Assert.ThrowsAsync<ApiException>(() =>
            service.RegisterAsync(SampleRegister(email: "dos@correo.com", documentId: "1701234567")));

        Assert.Equal(HttpStatusCode.Conflict, exception.StatusCode);
    }

    [Fact]
    public async Task LoginAsync_devuelve_tokens_con_credenciales_correctas()
    {
        var service = CreateService(out _);
        await service.RegisterAsync(SampleRegister());

        var result = await service.LoginAsync(new LoginRequestDto { Username = "ana@correo.com", Password = "Password123!" });

        Assert.Equal("ana@correo.com", result.Username);
        Assert.False(string.IsNullOrEmpty(result.AccessToken));
    }

    [Fact]
    public async Task LoginAsync_lanza_401_si_el_usuario_no_existe()
    {
        var service = CreateService(out _);

        var exception = await Assert.ThrowsAsync<ApiException>(() =>
            service.LoginAsync(new LoginRequestDto { Username = "nadie", Password = "Password123!" }));

        Assert.Equal(HttpStatusCode.Unauthorized, exception.StatusCode);
    }

    [Fact]
    public async Task LoginAsync_lanza_401_si_la_password_es_incorrecta()
    {
        var service = CreateService(out _);
        await service.RegisterAsync(SampleRegister());

        var exception = await Assert.ThrowsAsync<ApiException>(() =>
            service.LoginAsync(new LoginRequestDto { Username = "ana@correo.com", Password = "Incorrecta123!" }));

        Assert.Equal(HttpStatusCode.Unauthorized, exception.StatusCode);
    }

    [Fact]
    public async Task RefreshAsync_devuelve_nuevos_tokens_con_un_refresh_token_valido()
    {
        var service = CreateService(out _);
        var registered = await service.RegisterAsync(SampleRegister());

        var refreshed = await service.RefreshAsync(new RefreshRequestDto { RefreshToken = registered.RefreshToken });

        Assert.Equal("ana@correo.com", refreshed.Username);
        Assert.NotEqual(registered.RefreshToken, refreshed.RefreshToken);
    }

    [Fact]
    public async Task RefreshAsync_lanza_401_si_el_refresh_token_no_existe()
    {
        var service = CreateService(out _);

        var exception = await Assert.ThrowsAsync<ApiException>(() =>
            service.RefreshAsync(new RefreshRequestDto { RefreshToken = "no-existe" }));

        Assert.Equal(HttpStatusCode.Unauthorized, exception.StatusCode);
    }

    [Fact]
    public async Task RefreshAsync_lanza_401_si_el_refresh_token_ya_expiro()
    {
        var service = CreateService(out var repository);
        var registered = await service.RegisterAsync(SampleRegister());
        var user = await repository.GetByUsernameAsync("ana@correo.com");
        user!.RefreshTokenExpiry = DateTime.UtcNow.AddDays(-1);

        var exception = await Assert.ThrowsAsync<ApiException>(() =>
            service.RefreshAsync(new RefreshRequestDto { RefreshToken = registered.RefreshToken }));

        Assert.Equal(HttpStatusCode.Unauthorized, exception.StatusCode);
    }

    [Fact]
    public async Task ForgotPasswordAsync_envia_un_correo_con_un_token_de_reseteo_valido()
    {
        var service = CreateService(out var repository, out var emailService);
        await service.RegisterAsync(SampleRegister(email: "ana@correo.com"));

        await service.ForgotPasswordAsync(new ForgotPasswordRequestDto { Email = "ana@correo.com" });

        var sent = Assert.Single(emailService.SentPasswordResetEmails);
        Assert.Equal("ana@correo.com", sent.ToEmail);
        Assert.Equal("Ana", sent.FirstName);
        Assert.Contains("/reset-password?token=", sent.ResetLink);

        var stored = await repository.GetByUsernameAsync("ana@correo.com");
        Assert.NotNull(stored!.PasswordResetToken);
        Assert.True(stored.PasswordResetTokenExpiry > DateTime.UtcNow);
    }

    [Fact]
    public async Task ForgotPasswordAsync_no_lanza_error_ni_envia_correo_si_el_email_no_existe()
    {
        var service = CreateService(out _, out var emailService);

        await service.ForgotPasswordAsync(new ForgotPasswordRequestDto { Email = "nadie@correo.com" });

        Assert.Empty(emailService.SentPasswordResetEmails);
    }

    [Fact]
    public async Task ResetPasswordAsync_actualiza_la_password_y_permite_loguearse_con_la_nueva()
    {
        var service = CreateService(out var repository, out var emailService);
        await service.RegisterAsync(SampleRegister(email: "ana@correo.com"));
        await service.ForgotPasswordAsync(new ForgotPasswordRequestDto { Email = "ana@correo.com" });
        var token = (await repository.GetByUsernameAsync("ana@correo.com"))!.PasswordResetToken!;

        await service.ResetPasswordAsync(new ResetPasswordRequestDto { Token = token, NewPassword = "NuevaPassword123!" });

        var result = await service.LoginAsync(new LoginRequestDto { Username = "ana@correo.com", Password = "NuevaPassword123!" });
        Assert.Equal("ana@correo.com", result.Username);
    }

    [Fact]
    public async Task ResetPasswordAsync_invalida_el_token_despues_de_usarlo()
    {
        var service = CreateService(out var repository, out _);
        await service.RegisterAsync(SampleRegister(email: "ana@correo.com"));
        await service.ForgotPasswordAsync(new ForgotPasswordRequestDto { Email = "ana@correo.com" });
        var token = (await repository.GetByUsernameAsync("ana@correo.com"))!.PasswordResetToken!;
        await service.ResetPasswordAsync(new ResetPasswordRequestDto { Token = token, NewPassword = "NuevaPassword123!" });

        var exception = await Assert.ThrowsAsync<ApiException>(() =>
            service.ResetPasswordAsync(new ResetPasswordRequestDto { Token = token, NewPassword = "OtraPassword123!" }));

        Assert.Equal(HttpStatusCode.Unauthorized, exception.StatusCode);
    }

    [Fact]
    public async Task ResetPasswordAsync_lanza_401_con_un_token_inexistente()
    {
        var service = CreateService(out _, out _);

        var exception = await Assert.ThrowsAsync<ApiException>(() =>
            service.ResetPasswordAsync(new ResetPasswordRequestDto { Token = "no-existe", NewPassword = "NuevaPassword123!" }));

        Assert.Equal(HttpStatusCode.Unauthorized, exception.StatusCode);
    }

    [Fact]
    public async Task ResetPasswordAsync_lanza_401_con_un_token_expirado()
    {
        var service = CreateService(out var repository, out _);
        await service.RegisterAsync(SampleRegister(email: "ana@correo.com"));
        await service.ForgotPasswordAsync(new ForgotPasswordRequestDto { Email = "ana@correo.com" });
        var user = await repository.GetByUsernameAsync("ana@correo.com");
        var token = user!.PasswordResetToken!;
        user.PasswordResetTokenExpiry = DateTime.UtcNow.AddMinutes(-1);

        var exception = await Assert.ThrowsAsync<ApiException>(() =>
            service.ResetPasswordAsync(new ResetPasswordRequestDto { Token = token, NewPassword = "NuevaPassword123!" }));

        Assert.Equal(HttpStatusCode.Unauthorized, exception.StatusCode);
    }

    [Fact]
    public async Task ResetPasswordAsync_invalida_la_sesion_activa()
    {
        var service = CreateService(out var repository, out _);
        var registered = await service.RegisterAsync(SampleRegister(email: "ana@correo.com"));
        await service.ForgotPasswordAsync(new ForgotPasswordRequestDto { Email = "ana@correo.com" });
        var token = (await repository.GetByUsernameAsync("ana@correo.com"))!.PasswordResetToken!;

        await service.ResetPasswordAsync(new ResetPasswordRequestDto { Token = token, NewPassword = "NuevaPassword123!" });

        var exception = await Assert.ThrowsAsync<ApiException>(() =>
            service.RefreshAsync(new RefreshRequestDto { RefreshToken = registered.RefreshToken }));
        Assert.Equal(HttpStatusCode.Unauthorized, exception.StatusCode);
    }
}
