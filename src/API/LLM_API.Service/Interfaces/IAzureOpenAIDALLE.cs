namespace LLM_API.Service.Interfaces
{
    internal interface IAzureOpenAIDALLE
    {
        Task<string> AzureOpenAIVisionCall(string query);
        Task<string> GenerateDishImage(string dishNames);
    }
}