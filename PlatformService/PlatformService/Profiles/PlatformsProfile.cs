using AutoMapper;
using PlatformService.Dtos;
using PlatformService.Models;

namespace PlatformService.Profiles;
public class PlatformsProfile : Profile
{
    public PlatformsProfile()
    {
        CreateMap<Platform, PlatformReadDto>();
        CreateMap<PlatformCreateDto, Platform>();
        CreateMap<PlatformReadDto, PlatformPublishedDto>();
        CreateMap<Platform, GrpcPlatformModel>()
            .ForMember(dest => dest.PlatformId, opt => opt.MapFrom(x => x.Id))
            .ForMember(dest => dest.Name, opt => opt.MapFrom(x => x.Name))
            .ForMember(dest => dest.Publisher, opt => opt.MapFrom(x => x.Publisher));
    }
}
