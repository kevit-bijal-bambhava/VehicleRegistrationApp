using Newtonsoft.Json;
using System.Net.Http.Headers;
using System.Text;
using VehicleRegistrationMVC.Models;

namespace VehicleRegistrationMVC.Services
{
    public class AccountService
    {
        private readonly HttpClient _client;
        private readonly IConfiguration _configuration;
        private readonly ILogger<AccountService> _logger;

        public AccountService(HttpClient client, IConfiguration configuration, ILogger<AccountService> logger)
        {
            _client = client;
            _configuration = configuration;
            _logger = logger;
        }

        private void SetAuthorizationHeader(string jwtToken)
        {
            _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", jwtToken);
        }

        public async Task<string> SignUpAsync(SignUpViewModel model)
        {
            _logger.LogInformation("MVC_AccountService_SignUp");
            string jsonStr = JsonConvert.SerializeObject(model);
            var content = new StringContent(jsonStr, Encoding.UTF8, "application/json");
            HttpResponseMessage httpResponseMessage = await _client.PostAsync(_configuration["ApiBaseUrl"] + "api/account/signup", content);
            string response = await httpResponseMessage.Content.ReadAsStringAsync();
            return response;
        }
        public async Task<TokenResponse> LoginAsync(LoginViewModel model, HttpContext httpContext)
        {
            _logger.LogInformation("MVC_AccountService_Login");
            var jsonStr = JsonConvert.SerializeObject(model);
            var content = new StringContent(jsonStr, Encoding.UTF8, "application/json");
            HttpResponseMessage httpResponseMessage = await _client.PostAsync(_configuration["ApiBaseUrl"] + "api/account/login", content);
            if (httpResponseMessage.StatusCode == System.Net.HttpStatusCode.BadRequest)
            {
                string responseMessage = await httpResponseMessage.Content.ReadAsStringAsync();
                return new TokenResponse
                {
                    Message = responseMessage
                };
            }
            string response = await httpResponseMessage.Content.ReadAsStringAsync();

            if (string.IsNullOrEmpty(response) || response.Contains("Exception") || response.Contains("error"))
                return null;

            TokenResponse? loginResponse = JsonConvert.DeserializeObject<TokenResponse>(response);
            httpContext.Session.SetString("Token", loginResponse.JwtToken);
            return loginResponse;    
        }
        
        public async Task<string> AddProfilePhoto(IFormFile file, HttpContext httpContext)
        {
            if (file == null || file.Length == 0)
                throw new ArgumentException("File cannot be null or empty", nameof(file));

            using (var content = new MultipartFormDataContent())
            {
                using (var fileStream = file.OpenReadStream())
                {
                    var fileContent = new StreamContent(fileStream);
                    fileContent.Headers.ContentType = new MediaTypeHeaderValue(file.ContentType);

                    // Add file content to the multipart content
                    content.Add(fileContent, "profileImg", file.FileName);
                    string jwtToken = httpContext.Session.GetString("Token");
                    SetAuthorizationHeader(jwtToken);
                    // Send the POST request
                    var apiUrl = _configuration["ApiBaseUrl"] + "api/account/updateProfile";
                    HttpResponseMessage response = await _client.PostAsync(apiUrl, content);

                    var contentString = await response.Content.ReadAsStringAsync();
                    var imagePath = JsonConvert.DeserializeObject<ProfilePhoto>(contentString);
                    if (response.IsSuccessStatusCode)
                    {
                        var filePath = imagePath.FilePath;
                        return filePath;
                    }
                    return response.ToString();
                }
            }
        }
    }
}