using System.Net;
using AutoMapper;
using KatameApi.DTOs.Users;
using KatameApi.Middleware;
using KatameApi.Models;
using KatameApi.Repositories;

namespace KatameApi.Services;

public class UserService : IUserService
{
    private readonly IUserRepository _userRepository;
    private readonly IMapper _mapper;

    public UserService(IUserRepository userRepository, IMapper mapper)
    {
        _userRepository = userRepository;
        _mapper = mapper;
    }

    public async Task<List<UserDto>> GetAllAsync()
    {
        var users = await _userRepository.GetAllAsync();
        return _mapper.Map<List<UserDto>>(users);
    }

    public async Task<UserDto> CreateAsync(CreateUserDto request)
    {
        await EnsureUsernameAndEmailAreUniqueAsync(request.Username, request.Email);
        await EnsureDocumentIdIsUniqueAsync(request.DocumentId);

        var user = new User
        {
            Username = request.Username,
            FirstName = request.FirstName,
            LastName = request.LastName,
            DocumentId = request.DocumentId,
            PhoneNumber = request.PhoneNumber,
            Email = request.Email,
            PasswordHash = BCrypt.Net.BCrypt.HashPassword(request.Password),
            IsAdmin = request.IsAdmin,
            CreatedAt = DateTime.UtcNow,
        };

        await _userRepository.AddAsync(user);
        await _userRepository.SaveChangesAsync();

        return _mapper.Map<UserDto>(user);
    }

    public async Task<UserDto> UpdateAsync(int id, UpdateUserDto request, int currentUserId)
    {
        var user = await GetUserOrThrowAsync(id);
        await EnsureUsernameAndEmailAreUniqueAsync(request.Username, request.Email, excludeId: id);
        await EnsureDocumentIdIsUniqueAsync(request.DocumentId, excludeId: id);

        if (user.IsAdmin && !request.IsAdmin)
        {
            await EnsureNotLastAdminAsync();
        }

        user.Username = request.Username;
        user.FirstName = request.FirstName;
        user.LastName = request.LastName;
        user.DocumentId = request.DocumentId;
        user.PhoneNumber = request.PhoneNumber;
        user.Email = request.Email;
        user.IsAdmin = request.IsAdmin;

        if (!string.IsNullOrWhiteSpace(request.Password))
        {
            user.PasswordHash = BCrypt.Net.BCrypt.HashPassword(request.Password);
        }

        await _userRepository.SaveChangesAsync();

        return _mapper.Map<UserDto>(user);
    }

    public async Task DeleteAsync(int id, int currentUserId)
    {
        if (id == currentUserId)
        {
            throw new ApiException("No podés eliminar tu propia cuenta.");
        }

        var user = await GetUserOrThrowAsync(id);

        if (user.IsAdmin)
        {
            await EnsureNotLastAdminAsync();
        }

        _userRepository.Remove(user);
        await _userRepository.SaveChangesAsync();
    }

    private async Task EnsureUsernameAndEmailAreUniqueAsync(string username, string email, int? excludeId = null)
    {
        if (await _userRepository.ExistsByUsernameAsync(username, excludeId))
        {
            throw new ApiException("Ese nombre de usuario ya está en uso.", HttpStatusCode.Conflict);
        }

        if (await _userRepository.ExistsByEmailAsync(email, excludeId))
        {
            throw new ApiException("Ese email ya está en uso.", HttpStatusCode.Conflict);
        }
    }

    private async Task EnsureDocumentIdIsUniqueAsync(string documentId, int? excludeId = null)
    {
        if (await _userRepository.ExistsByDocumentIdAsync(documentId, excludeId))
        {
            throw new ApiException("Esa cédula ya está registrada.", HttpStatusCode.Conflict);
        }
    }

    private async Task EnsureNotLastAdminAsync()
    {
        var adminCount = await _userRepository.CountAdminsAsync();
        if (adminCount <= 1)
        {
            throw new ApiException("No podés quitar al último administrador del sistema.");
        }
    }

    private async Task<User> GetUserOrThrowAsync(int id)
    {
        var user = await _userRepository.GetByIdAsync(id);
        if (user is null)
        {
            throw new ApiException("El usuario no existe.", HttpStatusCode.NotFound);
        }

        return user;
    }
}
