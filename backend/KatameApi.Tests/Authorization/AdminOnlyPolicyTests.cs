using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.DependencyInjection;

namespace KatameApi.Tests.Authorization;

// UsersController protege sus 4 endpoints con [Authorize(Policy = "AdminOnly")] (mismo policy
// para todos). En vez de repetir un test HTTP por endpoint, se prueba el mecanismo compartido:
// la policy en sí, tal como se registra en Program.cs.
public class AdminOnlyPolicyTests
{
    private static IAuthorizationService BuildAuthorizationService()
    {
        var services = new ServiceCollection();
        services.AddLogging();
        services.AddAuthorizationCore(options =>
            options.AddPolicy("AdminOnly", policy => policy.RequireClaim("isAdmin", "true")));

        return services.BuildServiceProvider().GetRequiredService<IAuthorizationService>();
    }

    private static ClaimsPrincipal UserWith(params Claim[] claims) =>
        new(new ClaimsIdentity(claims, authenticationType: "TestAuth"));

    [Fact]
    public async Task Rechaza_a_un_usuario_sin_el_claim_isAdmin()
    {
        var authorizationService = BuildAuthorizationService();
        var user = UserWith(new Claim(ClaimTypes.NameIdentifier, "1"));

        var result = await authorizationService.AuthorizeAsync(user, "AdminOnly");

        Assert.False(result.Succeeded);
    }

    [Fact]
    public async Task Rechaza_a_un_usuario_con_isAdmin_en_false()
    {
        var authorizationService = BuildAuthorizationService();
        var user = UserWith(new Claim(ClaimTypes.NameIdentifier, "1"), new Claim("isAdmin", "false"));

        var result = await authorizationService.AuthorizeAsync(user, "AdminOnly");

        Assert.False(result.Succeeded);
    }

    [Fact]
    public async Task Permite_a_un_usuario_con_isAdmin_en_true()
    {
        var authorizationService = BuildAuthorizationService();
        var user = UserWith(new Claim(ClaimTypes.NameIdentifier, "1"), new Claim("isAdmin", "true"));

        var result = await authorizationService.AuthorizeAsync(user, "AdminOnly");

        Assert.True(result.Succeeded);
    }
}
