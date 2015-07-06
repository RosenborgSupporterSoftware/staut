using System;
using System.Threading.Tasks;
using System.Web.Http;
using Teller.Core.Repository;

namespace StautApi.Controllers
{
    [RoutePrefix("api/event")]
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
                  
                    return Ok(result);
                }
                catch (Exception ex)
                {
                    return BadRequest(ex.Message);
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

                    return Ok(result);
                }
                catch (Exception ex)
                {
                    return BadRequest(ex.Message);
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

                    return Ok(result);
                }
                catch (Exception ex)
                {
                    return BadRequest(ex.Message);
                }
            });
        }


    }


}