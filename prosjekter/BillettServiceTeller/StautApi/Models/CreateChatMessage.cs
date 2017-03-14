using System;

namespace StautApi.Models
{
    public class CreateChatMessage
    {
        public int Id { get; set; }
        public string Sender { get; set; }
        public DateTime Time { get; set; }
        public string Message { get; set; }
    }
}