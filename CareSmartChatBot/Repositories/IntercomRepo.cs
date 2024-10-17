using CareSmartChatBot.DBContext;
using CareSmartChatBot.Models;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Collections.Generic;
using System.Text.Json;

namespace CareSmartChatBot.Repositories
{
    public interface IIntercomRepo
    {
        Task<string> UpdateConversation(string content, string id, int totalPage);
        public Conversation GetLastConversation();
    }

    public class IntercomRepo: IIntercomRepo
    {
        private readonly ApplicationDBContext _dbContext;
        public IntercomRepo(ApplicationDBContext applicationDBContext) { 
        _dbContext= applicationDBContext;
        }


        public Conversation GetLastConversation()
        {
            var lastConversation =  _dbContext.Conversations
                                 .OrderByDescending(c => c.ConversationId) 
                                 .FirstOrDefault();
            return lastConversation;
        }
        public async Task<string> UpdateConversation(string content, string id, int totalPage)
        {
            try
            {
                var jsonDoc = JsonDocument.Parse(content);

                var conversation = _dbContext.Conversations.FirstOrDefault(x => x.ConversationId == id);
                var body = jsonDoc.RootElement.GetProperty("source").GetProperty("body").GetString();

                var admins = jsonDoc.RootElement.GetProperty("teammates").GetProperty("admins");
                var newJsonObject = new
                {
                    Body = body,
                    Admins = admins
                };

                string newJsonString = System.Text.Json.JsonSerializer.Serialize(newJsonObject, new JsonSerializerOptions { WriteIndented = true });
                Conversation con = new Conversation
                {
                    ConversationId = id,
                    OriginalJsonResponse = content,
                    MetaDataValue = newJsonString,
                    LastTotalValue =Convert.ToString(totalPage)
                };
                if (conversation == null)
                {
                    string jsonString = JsonConvert.SerializeObject(con.OriginalJsonResponse, Formatting.Indented);
                    var jsonObject = JsonConvert.DeserializeObject(jsonString);
                    //string filePath = Path.Combine("D:\\CareSmart\\ConversationFiles", id + ".json");
                    //File.WriteAllText(filePath, Convert.ToString( jsonObject));

                    string filePath = Path.Combine(Directory.GetCurrentDirectory(), "ConversationFiles");
                    if (!Directory.Exists(filePath))
                    {
                        Directory.CreateDirectory(filePath);
                    }
                    string fullFilePath = Path.Combine(filePath, id + ".json");
                    File.WriteAllText(fullFilePath, Convert.ToString(jsonObject));


                    await _dbContext.Conversations.AddAsync(con);
                    _dbContext.SaveChanges();
                }
                return con.ToString();
            }
            catch (Exception ex)
            {
                return "";
            }
        }
    }
}
