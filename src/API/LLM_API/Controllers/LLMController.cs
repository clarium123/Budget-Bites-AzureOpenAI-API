using LLM_API.Service;
using Microsoft.AspNetCore.Mvc;

namespace LLM_API.Controllers
{
    public class LLMController : Controller
    {
        private readonly AzureOpenAIChat _azureOpenAI;
        private readonly AzureOpenAIDALLE _azureOpenAIVision;
        public LLMController()
        {
            AzureOpenAIChat azureOpenAI = new();
            _azureOpenAI = azureOpenAI;
            AzureOpenAIDALLE azureOpenAIVision = new();
            _azureOpenAIVision = azureOpenAIVision;
        }

        [HttpGet]
        [Route("/GenerateDishesList")]
        public IActionResult GenerateDishesList([FromQuery] string cuisineName, [FromQuery] string dietaryPreference, [FromQuery] string mealType, [FromQuery] string allergiesOrExclusions)
        {
            if (cuisineName == null || dietaryPreference == null || mealType == null)
                return Ok("An error occurred.");
            List<string> allergiesListGet = null;
            string response;
            try
            {
                if (allergiesOrExclusions != null)
                {
                    allergiesListGet = [.. allergiesOrExclusions.Split(",")];
                }
                response = _azureOpenAI.GenerateDishesList(cuisineName, dietaryPreference, mealType, allergiesListGet);
                return Content(response, "application/json");
            }
            catch (Exception e)
            {
                Console.Out.Write(e);
                return Ok("An error occurred.");
            }
        }

        [HttpGet]
        [Route("/GenerateIngredientsListWithQuantities/")]
        public IActionResult GenerateIngredientsListWithQuantities([FromQuery] string dishName, [FromQuery] int numberOfPersons)
        {
            if (dishName == null || numberOfPersons == null)
                return Ok("An error occurred.");
            try
            {
                string response = _azureOpenAI.GenerateIngredientsListWithQuantities(dishName, numberOfPersons);
                return Content(response, "application/json");
            }
            catch (Exception e)
            {
                Console.Out.Write(e);
                return Ok("An error occurred.");
            }
        }

        [HttpGet]
        [Route("/GenerateRecipe")]
        public IActionResult GenerateRecipe([FromQuery] string dishName)
        {
            if (dishName == null)
                return Ok("An error occurred.");
            try
            {
                string response = _azureOpenAI.GenerateRecipe(dishName);
                return Content(response, "application/json");
            }
            catch (Exception e)
            {
                Console.Out.Write(e);
                return Ok("An error occurred.");
            }
        }

        [HttpGet]
        [Route("/GenerateDishImage")]
        public IActionResult GenerateDishImage([FromQuery] string dishNames)
        {
            if (dishNames == null)
                return Ok("An error occurred.");
            try
            {
                Task<string> response = _azureOpenAIVision.GenerateDishImage(dishNames);
                return Content(response.Result, "application/json");
            }
            catch (Exception e)
            {
                Console.Out.Write(e);
                return Ok("An error occurred.");
            }
        }
    }
}