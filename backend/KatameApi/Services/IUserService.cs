using KatameApi.DTOs.Users;

namespace KatameApi.Services;

public interface IUserService
{
    Task<List<UserDto>> GetAllAsync();
    Task<UserDto> CreateAsync(CreateUserDto request);
    Task<UserDto> UpdateAsync(int id, UpdateUserDto request, int currentUserId);
    Task DeleteAsync(int id, int currentUserId);
}
