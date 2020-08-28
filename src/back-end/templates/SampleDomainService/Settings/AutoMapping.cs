using Alidu.CQRS;
using AutoMapper;
using SampleDomainService.Data.Schemas;
using System;

namespace SampleDomainService.Settings
{
    public class AutoMapping : Profile
    {
        public AutoMapping()
        {
            CreateMap<EventStoreSchema, EventStore>()
                .ForMember(d => d.State, opt => opt.MapFrom(s => Enum.Parse(typeof(EventStateEnum), s.State)))
                .ReverseMap()
                .ForMember(d => d.State, opt => opt.MapFrom(s => s.State.ToString()));
        }
    }
}