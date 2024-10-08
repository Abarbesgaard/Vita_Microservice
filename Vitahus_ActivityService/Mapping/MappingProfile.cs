using AutoMapper;
using Vitahus_ActivityService_Shared;
using Vitahus_ActivityService.Dto;

namespace Vitahus_ActivityService.Mapping;

public class MappingProfile : Profile
{ 
    public MappingProfile()
    {
        CreateMap<Activity, ActivityDto>()
            .ReverseMap();
    }
}