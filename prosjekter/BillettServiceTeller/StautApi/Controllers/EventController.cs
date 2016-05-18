using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Web.Http;
using AutoMapper;
using StautApi.Models;
using Teller.Core.Entities;
using Teller.Core.Repository;

namespace StautApi.Controllers
{
    // runej@20032016: Bruker litt andre router for å gjøre dette mer spiselig for Ember.
    [RoutePrefix("api/matches")]
    public class EventController : ApiController
    {
        private readonly IEventRepository _eventRepository;

        public EventController(IEventRepository eventRepository)
        {
            _eventRepository = eventRepository;
        }

        [Route("")]
        [HttpGet]
        public async Task<IHttpActionResult> GetAllEvents()
        {
            return await Task.Factory.StartNew<IHttpActionResult>(() =>
            {
                try
                {
                    var result = _eventRepository.GetAll();

                    var data = Mapper.Map<IEnumerable<MatchDto>>(result);

                    var returnValue = new { matches = data};
                  
                    return Ok(returnValue);
                }
                catch (Exception ex)
                {
                    return InternalServerError(ex);
                }
            });
        }

        [Route("year/{year:int}")]
        [HttpGet]
        public async Task<IHttpActionResult> GetByYear(int year)
        {
            return await Task.Factory.StartNew<IHttpActionResult>(() =>
            {
                try
                {
                    var result = _eventRepository.GetByYear(year);

                    return Ok(Mapper.Map<IEnumerable<EventDto>>(result));
                }
                catch (Exception ex)
                {
                    return InternalServerError(ex);
                }
            });
        }
    
        [Route("{id:long}")]
        [HttpGet]
        public async Task<IHttpActionResult> GetEvent(long id)
        {
            return await Task.Factory.StartNew<IHttpActionResult>(() =>
            {
                try
                {
                    var result = _eventRepository.Get(id);
                    if (result == null)
                        return NotFound();

                    return Ok(Mapper.Map<EventDto>(result));
                }
                catch (Exception ex)
                {
                    return InternalServerError(ex);
                }
            });
        }

        [Route("")]
        [HttpPost]
        public async Task<IHttpActionResult> CreateEvent(CreateEvent @event)
        {
            return await Task.Factory.StartNew<IHttpActionResult>(() =>
            {
                try
                {
                    var e = Mapper.Map<BillettServiceEvent>(@event);
                    _eventRepository.Store(e);

                    return Ok();
                }
                catch (Exception ex)
                {
                    return InternalServerError(ex);
                }
            });
        }

        [Route("{id:long}/measurement")]
        [HttpPost]
        public async Task<IHttpActionResult> AddMeasurement(long id, Models.CreateMeasurement measurement)
        {
            return await Task.Factory.StartNew<IHttpActionResult>(() =>
            {
                try
                {
                    var e = _eventRepository.Get(id);
                    if (e == null)
                        return NotFound();

                    var m = Mapper.Map<Teller.Core.Entities.Measurement>(measurement);
                    e.Measurements.Add(m);

                    _eventRepository.SaveChanges();

                    return Ok();
                }
                catch (Exception ex)
                {
                    return InternalServerError(ex);
                }
            });
        } 
    }
}