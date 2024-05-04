using Azure;
using Azure.AI.OpenAI;
using LLM_API.Service.Interfaces;
using Microsoft.Extensions.Configuration;

namespace LLM_API.Service
{
    public class AzureOpenAIChat : IAzureOpenAIChat
    {
        private readonly IConfigurationRoot _config;
        public AzureOpenAIChat()
        {
            var config = new ConfigurationBuilder()
                .SetBasePath(AppDomain.CurrentDomain.BaseDirectory)
                .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true);

            _config = config.Build();
        }

        public async Task<string> AzureOpenAICall(string query)
        {
            string azureOpenAIEndpoint = _config["AzureOpenAIChatEndpoint"];
            string azureOpenAIAPIKey = _config["AzureOpenAIChatAPIKey"];
            string azureOpenAIDeploymentName = _config["AzureOpenAIChatDeploymentName"];
            Uri azureOpenAIResourceUri = new(azureOpenAIEndpoint);
            AzureKeyCredential azureKeyCredential = new(azureOpenAIAPIKey);
            OpenAIClient client = new(azureOpenAIResourceUri, azureKeyCredential);

            var chatCompletionsOptions = new ChatCompletionsOptions()
            {
                DeploymentName = azureOpenAIDeploymentName,
                Messages = {
                        new ChatRequestSystemMessage("You are a helpful meal planning assistant."),
                        new ChatRequestUserMessage(query)
                    }
            };

            Response<ChatCompletions> azureAIResponse = await client.GetChatCompletionsAsync(chatCompletionsOptions);
            ChatResponseMessage responseMessage = azureAIResponse.Value.Choices[0].Message;
            string response = responseMessage.Content;
            return response;
        }

        public string GenerateDishesList(string cuisineName, string dietaryPreference, string mealType, List<string>? allergiesList = null)
        {
            string query;
            string allergies = String.Empty;
            try
            {
                if (allergiesList != null)
                {
                    foreach (string allergy in allergiesList)
                    {
                        allergies = allergies + allergy + ",";
                    }
                    query = $"Generate a JSON response containing a list of 10 dishes and their average price for 1 servings in US dollars for {mealType} in {cuisineName} cuisine, which are exclusively for {dietaryPreference}, exclude the dishes containing these ingredients: {allergies}. " + "Use this JSON format only: {\"dishes\": [{\"name\": \"<dish_name>\",\"price\": \"$<price>\"}]}. Ensure the response is formatted correctly for an API response in JSON format only and strip out all other explanations like sure and notes.";
                }
                else
                {
                    query = $"Generate a JSON response containing a list of 10 dishes and their average price 1 servings in US dollars for {mealType} in {cuisineName} cuisine, which are exclusively for {dietaryPreference}. " + "Use this JSON format only: {\"dishes\": [{\"name\": \"<dish_name>\",\"price\": \"$<price>\"}]. Ensure the response is formatted correctly for an API response in JSON format only and strip out all other explanations like sure and notes.";
                }

                Task<string> response = AzureOpenAICall(query);
                return response.Result;
            }
            catch (Exception e)
            {
                Console.Out.Write(e);
                throw;
            }
        }

        public string GenerateIngredientsListWithQuantities(string dishName, int numberOfPersons)
        {
            string query;
            try
            {
                query = $"Generate a JSON response containing a list of ingredients, their quantities in appropriate units like g, kg, l, ml or whole counts and their prices in US dollars, as like used for ordering items online for the given dish. Dish name: {dishName}, Persons: {numberOfPersons}. " + "Use this JSON format only:{\"name\": \"<item_name>\",\"ingredients\": {\"<ingredient_name>\": {\"quantity\": \"<quantity>\",\"price\": \"$<price>\"}}}. Ensure the response is formatted correctly for an API response and make sure response includes 'gms' according to context in each ingredient.";

                Task<string> response = AzureOpenAICall(query);
                return response.Result;
            }
            catch (Exception e)
            {
                Console.Out.Write(e);
                throw;
            }
        }

        public string GenerateRecipe(string dishName)
        {
            string query;
            try
            {
                query = $"Generate a JSON response containing a steps for preparing a dish. Dish name: {dishName}. " + "Use this JSON format only: {\"name\": \"<dish_name>\",\"steps\": [{\"step_number\": \"<step_number>\",\"instruction\": \"<instruction>\"}]}. Ensure the response is formatted correctly for an API response and make sure response includes 'gms' according to context in each ingredient.";
                Task<string> response = AzureOpenAICall(query);
                return response.Result;
            }
            catch (Exception e)
            {
                Console.Out.Write(e);
                throw;
            }
        }
    }
}