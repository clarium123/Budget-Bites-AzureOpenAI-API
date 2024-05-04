namespace LLM_API.Service.Interfaces
{
    internal interface IAzureOpenAIChat
    {
        Task<string> AzureOpenAICall(string query);
        string GenerateDishesList(string cuisineName, string dietaryPreference, string mealType, List<string>? allergiesList);
        string GenerateIngredientsListWithQuantities(string dishName, int numberOfPersons);
        string GenerateRecipe(string dishName);
    }
}