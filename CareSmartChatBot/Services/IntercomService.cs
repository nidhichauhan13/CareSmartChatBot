using CareSmartChatBot.Models;
using CareSmartChatBot.Repositories;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json.Linq;
using System.Net.Http.Headers;
using System.Text.Json;
using System.Web;

namespace CareSmartChatBot.Services
{
    public interface IIntercomService
    {
        Task<string> GetConversations();
        Task<string> GetConversationById(string conversationId, int totalPage);
    }
    public class IntercomService : IIntercomService
    {
        private readonly HttpClient _httpClient;
        private const string IntercomApiUrl = "https://api.intercom.io/conversations";
        private const string BearerToken = "dG9rOjY1NmJmMWFlX2QzMGNfNGVmZl85NDU3X2I2ZTZmN2NiZjYxYzoxOjA=";
        private IIntercomRepo _intercomRepo { get; set; }

        public IntercomService(HttpClient httpClient, IIntercomRepo intercomRepo)
        {
            _httpClient = httpClient;
            _httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", BearerToken);
            _intercomRepo = intercomRepo;
        }

        public async Task<string> GetConversations()
        {
            var response = await _httpClient.GetAsync(IntercomApiUrl);
            response.EnsureSuccessStatusCode();
            string jsonResponse = await response.Content.ReadAsStringAsync();

            var jsonObject = JObject.Parse(jsonResponse);

            int totalPages = (int)jsonObject["pages"]["total_pages"];

            var builder = new UriBuilder(IntercomApiUrl);
            var query = HttpUtility.ParseQueryString(builder.Query);
            query["per_page"] = "150"; 
            var lastConversation = _intercomRepo.GetLastConversation();

            if (Convert.ToUInt32( lastConversation.LastTotalValue) <= totalPages)
            {
                for (int i = (int)Convert.ToUInt32(lastConversation.LastTotalValue); i <= totalPages; i++)
                {
                    string originalJsonResponse = lastConversation.OriginalJsonResponse;
                    JsonDocument doc = JsonDocument.Parse(originalJsonResponse);
                    JsonElement root =  doc.RootElement;

                    long createdAt = root.GetProperty("created_at").GetInt64();
                    long updatedAt = root.GetProperty("updated_at").GetInt64();

                    DateTime createdAtDateTime = DateTimeOffset.FromUnixTimeSeconds(createdAt).DateTime;
                    DateTime updatedAtDateTime = DateTimeOffset.FromUnixTimeSeconds(updatedAt).DateTime;
                    //if (createdAtDateTime >= DateTime.Now || updatedAtDateTime >= DateTime.Now) {
                        query["page"] = i.ToString();
                        builder.Query = query.ToString();
                        var url = builder.ToString();

                        var APIresponse = await _httpClient.GetAsync(url);
                        APIresponse.EnsureSuccessStatusCode();

                        var content = await APIresponse.Content.ReadAsStringAsync();
                        JObject json = JObject.Parse(content);
                        var conversationIds = new List<string>();
                        var conversations = json["conversations"];
                        if (conversations != null)
                        {
                            foreach (var conversation in conversations)
                            {
                                if ((string)conversation["type"] == "conversation")
                                {
                                    string id = (string)conversation["id"];
                                    conversationIds.Add(id);
                                }
                            }
                        }

                        foreach (var id in conversationIds)
                        {
                            await GetConversationById(id, i);
                        }
                    //}
                }
            }
            return "";
        }
        public async Task<string> GetConversationById(string conversationId,int totalPage)
        {
            var endpoint = $"{IntercomApiUrl}/{conversationId}";
            var response = await _httpClient.GetAsync(endpoint);
            response.EnsureSuccessStatusCode();

            var content = await response.Content.ReadAsStringAsync();
            await _intercomRepo.UpdateConversation(content, conversationId,totalPage);
            return content;
        }
    }
}
