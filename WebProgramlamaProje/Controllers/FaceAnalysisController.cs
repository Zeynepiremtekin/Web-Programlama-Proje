using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.IO;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace WebProgramlamaProje.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class FaceAnalysisController : ControllerBase
    {
        private readonly ILogger<FaceAnalysisController> _logger;

        // Google Cloud Vision API Anahtarı
        private readonly string _googleCloudApiKey = "AIzaSyBs6HVm_yjoaiQhiqGfCL7pmNwEIKoKrrw"; 

        public FaceAnalysisController(ILogger<FaceAnalysisController> logger)
        {
            _logger = logger;
        }

        [HttpPost("analyze")]
        public async Task<IActionResult> AnalyzeHairStyleAndColor(IFormFile file)
        {
            if (file == null || file.Length == 0)
            {
                _logger.LogError("No file uploaded.");
                return BadRequest("No file uploaded.");
            }

            try
            {
                var analysisResult = await AnalyzeImageWithGoogleVisionAPI(file);

                if (analysisResult == null)
                {
                    _logger.LogError("No face detected in the image.");
                    return BadRequest("No face detected in the image.");
                }

                var suggestions = GenerateHairStyleSuggestions(analysisResult);

                return Ok(suggestions);
            }
            catch (System.Exception ex)
            {
                _logger.LogError($"Error analyzing the image: {ex.Message}");
                return StatusCode(500, $"Internal Server Error: {ex.Message}");
            }
        }

        private async Task<FaceAttributes> AnalyzeImageWithGoogleVisionAPI(IFormFile file)
        {
            try
            {
                var client = new HttpClient();
                var url = $"https://vision.googleapis.com/v1/images:annotate?key={_googleCloudApiKey}";

                using var memoryStream = new MemoryStream();
                await file.CopyToAsync(memoryStream);
                var base64Image = Convert.ToBase64String(memoryStream.ToArray());

                var requestBody = $@"
                {{
                    'requests': [
                        {{
                            'image': {{
                                'content': '{base64Image}'
                            }},
                            'features': [
                                {{
                                    'type': 'FACE_DETECTION'
                                }}
                            ]
                        }}
                    ]
                }}";

                var content = new StringContent(requestBody, Encoding.UTF8, "application/json");

                var response = await client.PostAsync(url, content);
                if (response.IsSuccessStatusCode)
                {
                    var responseContent = await response.Content.ReadAsStringAsync();
                    _logger.LogInformation("Google Vision API Response: " + responseContent);  // Veriyi Loglama

                    return ParseFaceAttributes(responseContent);
                }

                _logger.LogError("Google Vision API request failed.");
                return null;
            }
            catch (System.Exception ex)
            {
                _logger.LogError($"Error during Google Vision API call: {ex.Message}");
                throw;
            }
        }

        private FaceAttributes ParseFaceAttributes(string responseContent)
        {
            try
            {
                var jsonResponse = JObject.Parse(responseContent);
                var faceAnnotations = jsonResponse["responses"]?[0]?["faceAnnotations"];

                if (faceAnnotations != null && faceAnnotations.HasValues)
                {
                    var firstFace = faceAnnotations[0];

                    var joyLikelihood = firstFace["joyLikelihood"]?.ToString() ?? "UNLIKELY";
                    var sorrowLikelihood = firstFace["sorrowLikelihood"]?.ToString() ?? "UNLIKELY";
                    var tiltAngle = firstFace["tiltAngle"]?.Value<float>() ?? 0;
                    var rollAngle = firstFace["rollAngle"]?.Value<float>() ?? 0;
                    var panAngle = firstFace["panAngle"]?.Value<float>() ?? 0;

                    return new FaceAttributes
                    {
                        JoyLikelihood = joyLikelihood,
                        SorrowLikelihood = sorrowLikelihood,
                        TiltAngle = tiltAngle,
                        RollAngle = rollAngle,
                        PanAngle = panAngle
                    };
                }

                return null;
            }
            catch (System.Exception ex)
            {
                _logger.LogError($"Error parsing the response: {ex.Message}");
                return null;
            }
        }

        private string GenerateHairStyleSuggestions(FaceAttributes attributes)
        {
            string hairStyle = "";
            string hairColor = "";

            if (attributes.JoyLikelihood == "LIKELY")
            {
                hairStyle = "Modern, voluminous hairstyle";
                hairColor = "Bright colors like blonde or light brown";
            }
            else if (attributes.SorrowLikelihood == "LIKELY")
            {
                hairStyle = "Natural, soft hairstyle";
                hairColor = "Dark shades like brown or black";
            }
            else
            {
                hairStyle = "Casual and relaxed hairstyle";
                hairColor = "Natural colors like dark brown or deep red";
            }

            if (attributes.TiltAngle < -5)
            {
                hairStyle += " with a side part";
            }
            else if (attributes.RollAngle < -2)
            {
                hairStyle += " with soft curls";
            }
            else if (attributes.PanAngle > 5)
            {
                hairStyle += " with a messy, tousled look";
            }

            var result = new
            {
                HairStyle = hairStyle,
                HairColor = hairColor
            };

            return JsonConvert.SerializeObject(result);
        }

        public class FaceAttributes
        {
            public string JoyLikelihood { get; set; }
            public string SorrowLikelihood { get; set; }
            public float TiltAngle { get; set; }
            public float RollAngle { get; set; }
            public float PanAngle { get; set; }
        }
    }
}
