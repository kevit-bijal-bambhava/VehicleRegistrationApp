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

        public AccountService(HttpClient client, IConfiguration configuration)
        {
            _client = client;
            _configuration = configuration;
        }

        public async Task<string> SignUpAsync(SignUpViewModel model)
        {
            var jsonStr = JsonConvert.SerializeObject(model);
            var content = new StringContent(jsonStr, Encoding.UTF8, "application/json");
            HttpResponseMessage httpResponseMessage = await _client.PostAsync(_configuration["ApiBaseUrl"] + "api/account/signup", content);
            string response = await httpResponseMessage.Content.ReadAsStringAsync();
            return response;
        }
        public async Task<TokenResponse> LoginAsync(LoginViewModel model, HttpContext httpContext)
        {
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

            TokenResponse loginResponse = JsonConvert.DeserializeObject<TokenResponse>(response);
            httpContext.Session.SetString("Token", loginResponse.JwtToken);
            return loginResponse;
        }
    }
}