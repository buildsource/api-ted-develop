using AutoMapper;
using TED.API.DTOs;
using TED.API.Entities;


namespace TED.API.Mappers
{
    public class ProfileMapper : Profile
    {
        public ProfileMapper() {
            CreateMap<Ted, ClienteTedResponseDto>().ReverseMap();
            CreateMap<Ted, ClienteTedRequestDto>().ReverseMap();

            CreateMap<Ted, AdminTedResponseDto>().ReverseMap();
            CreateMap<Ted, AdminTedRequestDto>().ReverseMap();

            CreateMap<LimiteTed, LimiteTedResponseDto>().ReverseMap();
            CreateMap<LimiteTed, LimiteTedRequestDto>().ReverseMap();
        }
    }
}
