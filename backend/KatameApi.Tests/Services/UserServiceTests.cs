using System.Net;
using AutoMapper;
using KatameApi.DTOs.Users;
using KatameApi.Middleware;
using KatameApi.Services;
using Microsoft.Extensions.Logging.Abstractions;

namespace KatameApi.Tests.Services;

public class UserServiceTests
{
    private static IMapper CreateMapper()
    {
        var config = new MapperConfiguration(cfg => cfg.AddProfile<UserMappingProfile>(), NullLoggerFactory.Instance);
        return config.CreateMapper();
    }

    private static UserService CreateService(out FakeUserRepository repository)
    {
        repository = new FakeUserRepository();
        return new UserService(repository, CreateMapper());
    }

    private static CreateUserDto Sample(string username, string email, bool isAdmin = false) => new()
    {
        Username = username,
        FirstName = "Nombre",
        LastName = "Apellido",
        DocumentId = $"DOC-{username}",
        PhoneNumber = "0000000000",
        Email = email,
        Password = "Password123!",
        IsAdmin = isAdmin,
    };

    [Fact]
    public async Task CreateAsync_crea_el_usuario_con_password_hasheada()
    {
        var service = CreateService(out var repository);

        var created = await service.CreateAsync(Sample("nuevo", "nuevo@katame.local"));

        Assert.Equal("nuevo", created.Username);
        Assert.False(created.IsAdmin);
        var stored = await repository.GetByIdAsync(created.Id);
        Assert.NotNull(stored);
        Assert.NotEqual("Password123!", stored!.PasswordHash);
        Assert.True(BCrypt.Net.BCrypt.Verify("Password123!", stored.PasswordHash));
    }

    [Fact]
    public async Task CreateAsync_lanza_409_si_el_username_ya_existe()
    {
        var service = CreateService(out _);
        await service.CreateAsync(Sample("duplicado", "uno@katame.local"));

        var exception = await Assert.ThrowsAsync<ApiException>(() =>
            service.CreateAsync(Sample("duplicado", "dos@katame.local")));

        Assert.Equal(HttpStatusCode.Conflict, exception.StatusCode);
    }

    [Fact]
    public async Task CreateAsync_lanza_409_si_el_email_ya_existe()
    {
        var service = CreateService(out _);
        await service.CreateAsync(Sample("uno", "repetido@katame.local"));

        var exception = await Assert.ThrowsAsync<ApiException>(() =>
            service.CreateAsync(Sample("dos", "repetido@katame.local")));

        Assert.Equal(HttpStatusCode.Conflict, exception.StatusCode);
    }

    [Fact]
    public async Task GetAllAsync_devuelve_todos_los_usuarios()
    {
        var service = CreateService(out _);
        await service.CreateAsync(Sample("uno", "uno@katame.local"));
        await service.CreateAsync(Sample("dos", "dos@katame.local"));

        var users = await service.GetAllAsync();

        Assert.Equal(2, users.Count);
    }

    [Fact]
    public async Task UpdateAsync_actualiza_datos_y_no_cambia_password_si_viene_vacia()
    {
        var service = CreateService(out var repository);
        var created = await service.CreateAsync(Sample("original", "original@katame.local"));
        var originalHash = (await repository.GetByIdAsync(created.Id))!.PasswordHash;

        var updated = await service.UpdateAsync(created.Id, new UpdateUserDto
        {
            Username = "actualizado",
            FirstName = "Nombre",
            LastName = "Apellido",
            DocumentId = "DOC-actualizado",
            PhoneNumber = "0000000000",
            Email = "actualizado@katame.local",
            Password = null,
            IsAdmin = false,
        }, currentUserId: 999);

        Assert.Equal("actualizado", updated.Username);
        var stored = await repository.GetByIdAsync(created.Id);
        Assert.Equal(originalHash, stored!.PasswordHash);
    }

    [Fact]
    public async Task UpdateAsync_cambia_password_si_viene_no_vacia()
    {
        var service = CreateService(out var repository);
        var created = await service.CreateAsync(Sample("original", "original@katame.local"));
        var originalHash = (await repository.GetByIdAsync(created.Id))!.PasswordHash;

        await service.UpdateAsync(created.Id, new UpdateUserDto
        {
            Username = "original",
            FirstName = "Nombre",
            LastName = "Apellido",
            DocumentId = "DOC-original",
            PhoneNumber = "0000000000",
            Email = "original@katame.local",
            Password = "NuevaPassword123!",
            IsAdmin = false,
        }, currentUserId: 999);

        var stored = await repository.GetByIdAsync(created.Id);
        Assert.NotEqual(originalHash, stored!.PasswordHash);
        Assert.True(BCrypt.Net.BCrypt.Verify("NuevaPassword123!", stored.PasswordHash));
    }

    [Fact]
    public async Task UpdateAsync_lanza_400_si_intenta_quitarle_admin_al_ultimo_admin()
    {
        var service = CreateService(out _);
        var admin = await service.CreateAsync(Sample("admin", "admin@katame.local", isAdmin: true));

        var exception = await Assert.ThrowsAsync<ApiException>(() =>
            service.UpdateAsync(admin.Id, new UpdateUserDto
            {
                Username = admin.Username,
                FirstName = admin.FirstName,
                LastName = admin.LastName,
                DocumentId = admin.DocumentId,
                PhoneNumber = admin.PhoneNumber,
                Email = admin.Email,
                IsAdmin = false,
            }, currentUserId: admin.Id));

        Assert.Equal(HttpStatusCode.BadRequest, exception.StatusCode);
    }

    [Fact]
    public async Task UpdateAsync_permite_quitar_admin_si_hay_otro_admin()
    {
        var service = CreateService(out _);
        var admin1 = await service.CreateAsync(Sample("admin1", "admin1@katame.local", isAdmin: true));
        await service.CreateAsync(Sample("admin2", "admin2@katame.local", isAdmin: true));

        var updated = await service.UpdateAsync(admin1.Id, new UpdateUserDto
        {
            Username = admin1.Username,
            FirstName = admin1.FirstName,
            LastName = admin1.LastName,
            DocumentId = admin1.DocumentId,
            PhoneNumber = admin1.PhoneNumber,
            Email = admin1.Email,
            IsAdmin = false,
        }, currentUserId: 999);

        Assert.False(updated.IsAdmin);
    }

    [Fact]
    public async Task DeleteAsync_lanza_400_si_el_usuario_se_intenta_eliminar_a_si_mismo()
    {
        var service = CreateService(out _);
        var user = await service.CreateAsync(Sample("uno", "uno@katame.local"));

        var exception = await Assert.ThrowsAsync<ApiException>(() =>
            service.DeleteAsync(user.Id, currentUserId: user.Id));

        Assert.Equal(HttpStatusCode.BadRequest, exception.StatusCode);
    }

    [Fact]
    public async Task DeleteAsync_lanza_400_si_es_el_ultimo_admin()
    {
        var service = CreateService(out _);
        var admin = await service.CreateAsync(Sample("admin", "admin@katame.local", isAdmin: true));

        var exception = await Assert.ThrowsAsync<ApiException>(() =>
            service.DeleteAsync(admin.Id, currentUserId: 999));

        Assert.Equal(HttpStatusCode.BadRequest, exception.StatusCode);
    }

    [Fact]
    public async Task DeleteAsync_elimina_si_no_es_el_ultimo_admin_ni_el_usuario_actual()
    {
        var service = CreateService(out var repository);
        var admin1 = await service.CreateAsync(Sample("admin1", "admin1@katame.local", isAdmin: true));
        await service.CreateAsync(Sample("admin2", "admin2@katame.local", isAdmin: true));

        await service.DeleteAsync(admin1.Id, currentUserId: 999);

        Assert.Null(await repository.GetByIdAsync(admin1.Id));
    }

    [Fact]
    public async Task DeleteAsync_lanza_404_si_el_usuario_no_existe()
    {
        var service = CreateService(out _);

        var exception = await Assert.ThrowsAsync<ApiException>(() => service.DeleteAsync(999, currentUserId: 1));

        Assert.Equal(HttpStatusCode.NotFound, exception.StatusCode);
    }
}
