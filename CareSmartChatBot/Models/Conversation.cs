using System.ComponentModel.DataAnnotations;

namespace CareSmartChatBot.Models
{
    public class Conversation
    {
        public string ConversationId { get; set; }
        public string OriginalJsonResponse { get; set; }
        public string MetaDataValue { get; set; }
        public string LastTotalValue { get; set; }
    }
}
