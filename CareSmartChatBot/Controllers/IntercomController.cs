using CareSmartChatBot.Services;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace CareSmartChatBot.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class IntercomController : ControllerBase
    {
        private readonly IIntercomService _intercomService;
        public IntercomController(IIntercomService intercomService)
        {
            _intercomService = intercomService;
        }

        [HttpGet("conversations")]
        public async Task<IActionResult> GetConversations()
        {
            var data = await _intercomService.GetConversations();
            var jsonData = JsonConvert.DeserializeObject<dynamic>(data);
            var conversations = jsonData.conversations;
            return Ok(conversations); 
        }

    }
}
