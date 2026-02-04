using AirRouteManagementSystem.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace AirRouteManagementSystem.Areas.Identity.Controllers
{
    [Area(nameof(Identity))]
    [Route("[Area]/[controller]")]
    [ApiController]
    public class ChatController : ControllerBase
    {
        private readonly IOpenAiService _openAiService;

        public ChatController(IOpenAiService openAiService)
        {
            _openAiService = openAiService;
        }

        [HttpPost]
        public async Task<IActionResult> Ask([FromBody] ChatRequestDto chatRequestDto)
        {
            var response = await _openAiService.AskChatGpt(chatRequestDto.Question);
            return Ok(response);
        }
    }
}
