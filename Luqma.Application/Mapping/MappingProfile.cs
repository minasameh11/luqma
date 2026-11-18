using AutoMapper;
using Luqma.Application.Features.Dtos.Responses.AccountResponses;
using Luqma.Domain.AccountsDtos;

namespace Luqma.Application.Mapping
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            CreateMap<AuthUserResponseDto, AuthResponse>().ReverseMap();
            CreateMap<UserReadDto, UserResponse>().ReverseMap();
        }
    }
}
