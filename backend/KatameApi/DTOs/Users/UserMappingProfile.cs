using AutoMapper;
using KatameApi.Models;

namespace KatameApi.DTOs.Users;

public class UserMappingProfile : Profile
{
    public UserMappingProfile()
    {
        CreateMap<User, UserDto>();
    }
}
