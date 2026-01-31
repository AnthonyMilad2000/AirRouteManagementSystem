namespace AirRouteManagementSystem.Services.Interface
{
    public interface IOpenAiService
    {
        public Task<string> AskChatGpt(string question);
    }
}
