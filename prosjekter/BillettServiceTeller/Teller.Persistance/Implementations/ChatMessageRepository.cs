using System;
using System.Collections.Generic;
using System.Linq;
using Teller.Core.Entities;
using Teller.Core.Repository;

namespace Teller.Persistance.Implementations
{
    public class ChatMessageRepository : IChatMessageRepository
    {
        #region Fields

        private readonly TellerContext _context;

        #endregion

        public ChatMessage Get(long id)
        {
            return _context.ChatMessages.Find(id);
        }

        public IEnumerable<ChatMessage> GetAll()
        {
            return _context.ChatMessages;
        }

        public void Store(ChatMessage evt)
        {
            _context.ChatMessages.Add(evt);

            _context.SaveChanges();
        }

        public void SaveChanges()
        {
            _context.SaveChanges();
        }

        public void Delete(ChatMessage evt)
        {
            _context.ChatMessages.Remove(evt);
            _context.SaveChanges();
        }

        public IEnumerable<ChatMessage> GetPage(int oldestSeenMessage)
        {
            // TODO: BIVA-logikk bare for å komme igang. Verifiser om dette funker noe særlig.
            if (oldestSeenMessage == 0)
            {
                return _context.ChatMessages.OrderByDescending(cm => cm.Time).Take(100);
            }

            return
                _context.ChatMessages.OrderByDescending(cm => cm.Time).Where(cm => cm.Id < oldestSeenMessage).Take(100);
        }

        public ChatMessageRepository(TellerContext context)
        {
            _context = context;
        }
    }
}
