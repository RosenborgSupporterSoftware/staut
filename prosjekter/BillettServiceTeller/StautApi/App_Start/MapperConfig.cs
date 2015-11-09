using AutoMapper;
using StautApi.Models;
using Teller.Core.Entities;

namespace StautApi
{
    public static class MapperConfig
    {
        public static void Configure()
        {
            Mapper.CreateMap<CreateEvent, BillettServiceEvent>()
                .ForMember(dest => dest.Id, opt => opt.Ignore())
                .ForMember(dest => dest.Measurements, opt => opt.Ignore());

            Mapper.CreateMap<CreateMeasurement, Measurement>()
                .ForMember(dest => dest.MeasurementId, opt => opt.Ignore())
                .ForMember(dest => dest.BillettServiceEventId, opt => opt.Ignore())
                .ForMember(dest => dest.BillettServiceEvent, opt => opt.Ignore())
                .ForMember(dest => dest.TotalAmountSold, opt => opt.Ignore());

            Mapper.CreateMap<BillettServiceEvent, EventDto>();

            Mapper.CreateMap<Measurement, MeasurementDto>()
                .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.MeasurementId));
        }
    }
}