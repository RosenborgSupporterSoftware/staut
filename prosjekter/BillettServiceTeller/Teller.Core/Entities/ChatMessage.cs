using System;

namespace Teller.Core.Entities
{
    /// <summary>
    /// En chat-melding i web-interfacet
    /// </summary>
    public class ChatMessage
    {
        public int Id { get; set; }
        public string SenderNickname { get; set; }
        public DateTime Time { get; set; }
        public string Message { get; set; }
    }
}
