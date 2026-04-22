using Microsoft.AspNetCore.Mvc;
using Portfolio.Model;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;

namespace Portfolio.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AiController : Controller
    {

        private readonly IConfiguration _config;

        public AiController(IConfiguration config)
        {
            _config = config;
        }

        [HttpPost("ask")]
        public async Task<IActionResult> Ask([FromBody] UserQuery query)
        {
            var apiKey = _config["OpenAI:ApiKey"];

            var client = new HttpClient();
            client.DefaultRequestHeaders.Authorization =
                new AuthenticationHeaderValue("Bearer", apiKey);
            var resumePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "resume.txt");
            var resumeText = System.IO.File.ReadAllText(resumePath);

            var requestBody = new
            {
                model = "gpt-4o-mini",
                messages = new[]
                {
                new {
                    role = "system",
                    content = $@"You are Himanshu's AI portfolio assistant.

                        Resume:
                        {resumeText}

                        Instructions:
                        - Answer like a confident developer
                        - Highlight achievements
                        - Be concise
                        - If asked 'why hire you' → give strong pitch
                        "
        },
                new { role = "user", content = query.Question }
            }
            };

            var response = await client.PostAsJsonAsync(
                "https://api.openai.com/v1/chat/completions",
                requestBody
            );

           // var result = "";
            var result = await response.Content.ReadAsStringAsync();

            return Ok(JsonDocument.Parse(result));
        }

      

    }
}
