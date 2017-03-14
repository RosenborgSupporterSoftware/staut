using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Web.Http;
using Autofac;
using Autofac.Integration.WebApi;
using AutoMapper;
using Teller.Core.Entities;
using Teller.Core.Repository;
using Teller.Persistance;
using Teller.Persistance.Implementations;
using Measurement = Teller.Core.Entities.Measurement;

namespace StautApi
{
    public static class IocConfig
    {
        public static void Bootstrap(HttpConfiguration config)
        {
            var mapperConfig = MapperConfig.Configure();

            var builder = new ContainerBuilder();
            builder.RegisterApiControllers(Assembly.GetExecutingAssembly());

            // Dummy implementation
            //builder.RegisterType<DummyEventRepo>().As<IEventRepository>().InstancePerLifetimeScope();

            // Real implementation
            builder.RegisterType<EventRepository>().As<IEventRepository>().InstancePerLifetimeScope();
            builder.RegisterType<MeasurementRepository>().As<IMeasurementRepository>().InstancePerLifetimeScope();
            builder.RegisterType<ChatMessageRepository>().As<IChatMessageRepository>().InstancePerLifetimeScope();
            builder.RegisterType<TellerContext>().InstancePerLifetimeScope();

            builder.RegisterInstance(mapperConfig).As<MapperConfiguration>();

            var container = builder.Build();

            config.DependencyResolver = new AutofacWebApiDependencyResolver(container);
        }    
    }


    //TODO: ngs@06072015 Skal fjernes
    public class DummyEventRepo : IEventRepository
    {
        private readonly List<BillettServiceEvent> _events;

        public DummyEventRepo()
        {
            _events = new List<BillettServiceEvent>
            {
                new BillettServiceEvent
                {
                    Id = 1,
                    EventNumber = 1,
                    EventCode = "EC",
                    DisplayName = "Kamp 1",
                    Tournament = "UCL",
                    Round = 1,
                    Season = "2015",
                    Location = "Trh",
                    Opponent = "Strindheim",
                    Start = DateTime.Now,
                    EventUrl = "URL",
                    AvailibilityUrl = "URL",
                    GeometryUrl = "URL",
                    FinalEstimatedSeatCount = 20101,
                    OfficialSeatCount = 21010,
                    Measurements = new List<Measurement>
                    {
                        new Measurement
                        {
                            MeasurementId = 1,
                            MeasurementTime = DateTime.Now,
                            AmountSold = 18234,
                            AmountSeasonTicket = 4567,
                            AmountAvailable = 6789,
                            AmountReserved = 3211,
                            AmountUnavailable = 2323,
                            AmountTicketMaster = 1111,
                            AmountUnknown = 42
                        }                   
                    }
                }
            };
        }
        
        public BillettServiceEvent Get(long id)
        {
            return _events.First();
        }

        public IEnumerable<BillettServiceEvent> GetAll()
        {
            return _events;
        }

        public void Store(BillettServiceEvent evt)
        {
            throw new System.NotImplementedException();
        }

        public void SaveChanges()
        {
            throw new System.NotImplementedException();
        }

        public void Delete(BillettServiceEvent evt)
        {
            throw new System.NotImplementedException();
        }

        public BillettServiceEvent GetByBillettServiceId(int id)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<BillettServiceEvent> GetByYear(int year)
        {
            return _events;
        }
    }
}