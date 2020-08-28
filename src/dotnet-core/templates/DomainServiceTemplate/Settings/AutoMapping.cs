using Alidu.CQRS;
using AutoMapper;
using DomainServiceTemplate.Data.Schemas;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DomainServiceTemplate.Settings
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
