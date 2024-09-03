using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Net.Http;
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

        public async Task<string> SignUpAsync(SignUpViewModel model)
        {
            _logger.LogInformation("MVC_AccountService_SignUp");
            var jsonStr = JsonConvert.SerializeObject(model);
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
    }
}