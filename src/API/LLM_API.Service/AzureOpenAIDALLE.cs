using Azure;
using Azure.AI.OpenAI;
using LLM_API.Service.Interfaces;
using Microsoft.Extensions.Configuration;
using System.Text.Json;

namespace LLM_API.Service
{
    public class AzureOpenAIDALLE : IAzureOpenAIDALLE
    {
        private readonly IConfigurationRoot _config;

        public AzureOpenAIDALLE()
        {
            var config = new ConfigurationBuilder()
                .SetBasePath(AppDomain.CurrentDomain.BaseDirectory)
                .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true);

            _config = config.Build();
        }
        public async Task<string> AzureOpenAIVisionCall(string query)
        {
            string azureOpenAIVisionEndpoint = _config["AzureOpenAIDALLEEndpoint"];
            string azureOpenAIVisionAPIKey = _config["AzureOpenAIDALLEAPIKey"];
            string azureOpenAIDeploymentName = _config["AzureOpenAIDALLEDeploymentName"];
            Uri azureOpenAIResourceUri = new(azureOpenAIVisionEndpoint);
            AzureKeyCredential azureKeyCredential = new(azureOpenAIVisionAPIKey);
            OpenAIClient client = new(azureOpenAIResourceUri, azureKeyCredential);

            var imageGenerationOptions = new ImageGenerationOptions()
            {
                DeploymentName = azureOpenAIDeploymentName,
                Prompt = query,
                Size = ImageSize.Size1024x1024
            };
            Response<ImageGenerations> imageGenerations = await client.GetImageGenerationsAsync(imageGenerationOptions);

            Uri imageUri = imageGenerations.Value.Data[0].Url;

            return imageUri.ToString();




            //var gpt4vKey = "9489fd13b4764e53834fde0379d402a8";
            //var imagePath = "C:\\Users\\ArunkumarR2\\source\\repos\\Food-App-Using-LLM-Azure-OpenAI-API-Repo\\src\\API\\LLM_API.Service\\Outlook-Logo.png";
            //var encodedImage = Convert.ToBase64String(await System.IO.File.ReadAllBytesAsync(imagePath));

            //using var httpClient = new HttpClient();
            //httpClient.DefaultRequestHeaders.Add("api-key", gpt4vKey);
            ////httpClient.DefaultRequestHeaders.Add("Content-Type", "application/json");
            //httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

            //var payload = new
            //{
            //    enhancements = new
            //    {
            //        ocr = new { enabled = true },
            //        grounding = new { enabled = true }
            //    },
            //    messages = new object[]
            //    {
            //    new
            //    {
            //        role = "system",
            //        content = new object[]
            //        {
            //            new
            //            {
            //                type = "text",
            //                text = "You are an AI assistant that helps people find information."
            //            }
            //        }
            //    },
            //    new
            //    {
            //        role = "user",
            //        content = new object[]
            //        {
            //            new
            //            {
            //                type = "image_url",
            //                image_url = new { url = $"data:image/jpeg;base64,{encodedImage}" }
            //            },
            //            new
            //            {
            //                type = "text",
            //                text = "whats this"
            //            }
            //        }
            //    },
            //    new
            //    {
            //        role = "assistant",
            //        content = new object[]
            //        {
            //            new
            //            {
            //                type = "text",
            //                text = "This image appears to show a gourmet dish consisting of shrimp served on top of a round slice of what looks like potato or another type of root vegetable"
            //            }
            //        }
            //    }
            //    },
            //    temperature = 0.7,
            //    top_p = 0.95,
            //    max_tokens = 800
            //};

            //var gpt4vEndpoint = "https://insightaoaia01.openai.azure.com/openai/deployments/gpt4-vision/chat/completions?api-version=2024-02-15-preview";

            //try
            //{
            //    //var response = await httpClient.PostAsync(gpt4vEndpoint, new StringContent(JsonConvert.SerializeObject(payload), Encoding.UTF8, "application/json"));

            //    var jsonPayload = System.Text.Json.JsonSerializer.Serialize(payload);
            //    var httpContent = new StringContent(jsonPayload, Encoding.UTF8, "application/json");
            //    var response = await httpClient.PostAsync(gpt4vEndpoint, httpContent);

            //    response.EnsureSuccessStatusCode();
            //    var responseContent = await response.Content.ReadAsStringAsync();
            //    Console.WriteLine(responseContent);
            //    return responseContent;
            //}
            //catch (HttpRequestException e)
            //{
            //    Console.WriteLine($"Failed to make the request. Error: {e}");
            //    Environment.Exit(-1);
            //    return "";
            //}
        }

        //public string GenerateDishImage(string dishName)
        //{
        //    string query;
        //    try
        //    {
        //        query = $"Provide me an image of the dish: {dishName}.";
        //        Task<string> response = AzureOpenAIVisionCall(query);
        //        return response.Result;
        //    }
        //    catch (Exception e)
        //    {
        //        Console.Out.Write(e);
        //        throw;
        //    }
        // }

        // Define a class to represent a dish
        public async Task<string> GenerateDishImage(string dishNames)
        {
            List<string> dishNamesList = [.. dishNames.Split(",")];
            var tasks = new List<Task<string>>();

            foreach (var dishName in dishNamesList)
            {
                tasks.Add(Task.Run(async () =>
                {
                    string query = $"Provide me an image of the dish: {dishName}.";
                    string response = await AzureOpenAIVisionCall(query);
                    return response;
                }));
            }
            var res = await Task.WhenAll(tasks);

            string result = JsonSerializer.Serialize(res);
            return result;
        }
    }
}