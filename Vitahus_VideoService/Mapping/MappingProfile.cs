using AutoMapper;
using Vitahus_VideoService_Shared;
using Vitahus_VideoService.Dto;

namespace Vitahus_VideoService.Mapping;

public class MappingProfile : Profile
{
    public MappingProfile()
    {
        CreateMap<Video, VideoDto>()
            .ReverseMap();
    }
}