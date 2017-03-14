using AutoMapper;
using StautApi.Models;
using Teller.Core.Entities;

namespace StautApi
{

    // TODO: Siden AutoMapper ikke lenger er statisk, finn ut hvordan vi skal f� dette inn i controllerne. Sannsynligvis via dependency injection.

    public static class MapperConfig
    {
        public static MapperConfiguration Configure()
        {
            var config = new MapperConfiguration(cfg =>
            {
                cfg.CreateMap<CreateEvent, BillettServiceEvent>()
                   .ForMember(dest => dest.Id, opt => opt.Ignore())
                   .ForMember(dest => dest.Measurements, opt => opt.Ignore());

                cfg.CreateMap<CreateMeasurement, Measurement>()
                   .ForMember(dest => dest.MeasurementId, opt => opt.Ignore())
                   .ForMember(dest => dest.BillettServiceEventId, opt => opt.Ignore())
                   .ForMember(dest => dest.BillettServiceEvent, opt => opt.Ignore())
                   .ForMember(dest => dest.TotalAmountSold, opt => opt.Ignore());

                cfg.CreateMap<BillettServiceEvent, EventDto>();

                cfg.CreateMap<Measurement, MeasurementDto>()
                   .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.MeasurementId));
            });

            return config;
        }
    }
}