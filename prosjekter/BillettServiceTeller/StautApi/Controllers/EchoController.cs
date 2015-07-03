using System.Web.Http;

namespace StautApi.Controllers
{
    //[Route("api/echo")]
    public class EchoController : ApiController
    {
        //public EchoController()
        //{
            
        //}

        [Route("api/echo/{input}")]
        [HttpGet]
        public IHttpActionResult Echo(string input)
        {
            return Ok(input);
        }
    }
}