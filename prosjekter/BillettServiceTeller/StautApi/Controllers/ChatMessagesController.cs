using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Web.Http;
using AutoMapper;
using StautApi.Models;
using Teller.Core.Repository;

namespace StautApi.Controllers
{
    [RoutePrefix("api/chatMessages")]
    public class ChatMessagesController : ApiController
    {
        #region Fields

        private readonly IChatMessageRepository _chatRepository;

        #endregion

        public ChatMessagesController(IChatMessageRepository chatRepository)
        {
            _chatRepository = chatRepository;
        }

        [Route("")]
        [HttpGet]
        public async Task<IHttpActionResult> GetChatPage(int lastSeenMessage = 0)
        {
            return await Task.Factory.StartNew<IHttpActionResult>(() =>
            {
                try
                {
                    var result = Mapper.Map<IEnumerable<CreateChatMessage>>(_chatRepository.GetPage(lastSeenMessage));

                    var returnValue = new {chatMessages = result};

                    return Ok(returnValue);
                }
                catch (Exception ex)
                {
                    return InternalServerError(ex);
                }
            });
        }
    }
}
