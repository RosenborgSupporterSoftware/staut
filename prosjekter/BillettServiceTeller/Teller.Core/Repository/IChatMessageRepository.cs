using System.Collections.Generic;
using Teller.Core.Entities;

namespace Teller.Core.Repository
{
    public interface IChatMessageRepository : IRepository<ChatMessage>
    {
        /// <summary>
        /// Hent en side med meldinger basert på den eldste som er sett, eller bare de hundre nyeste om oldestSeenMessage == 0
        /// </summary>
        /// <param name="oldestSeenMessage">Id på siste melding vi så</param>
        /// <returns>En "side" med meldinger</returns>
        IEnumerable<ChatMessage> GetPage(int oldestSeenMessage);
    }
}
