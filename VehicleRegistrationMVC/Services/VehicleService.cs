using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using OfficeOpenXml;
using System.Net.Http.Headers;
using System.Text;
using VehicleRegistrationMVC.Models;

public class VehicleService
{
    private readonly IConfiguration _configuration;
    private readonly HttpClient _httpClient;
    private readonly ILogger<VehicleService> _logger;

    public VehicleService(IConfiguration configuration, HttpClient httpClient, ILogger<VehicleService> logger)
    {
        _configuration = configuration;
        _httpClient = httpClient;
        _logger = logger;
    }

    private void SetAuthorizationHeader(string jwtToken)
    {
        _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", jwtToken);
    }

    public async Task<List<VehicleViewModel>> GetVehicles(string jwtToken)
    {
        _logger.LogInformation("MVC_VehicleService_GetVehicles");
        SetAuthorizationHeader(jwtToken);
        var response = await _httpClient.GetAsync(new Uri(_configuration["ApiBaseUrl"] + "api/Vehicle/getAllVehicle"));

        if (!response.IsSuccessStatusCode)
        {
            // Handle error response
            throw new HttpRequestException($"Error: {response.StatusCode}");
        }

        var responseString = await response.Content.ReadAsStringAsync();
        return JsonConvert.DeserializeObject<List<VehicleViewModel>>(responseString);
    }

    public async Task<string> AddVehicle(VehicleViewModel vehicleModel, string jwtToken)
    {
        _logger.LogInformation("MVC_VehicleService_AddVehicle");
        SetAuthorizationHeader(jwtToken);
        var jsonStr = JsonConvert.SerializeObject(vehicleModel);
        var content = new StringContent(jsonStr, Encoding.UTF8, "application/json");

        var response = await _httpClient.PostAsync(new Uri(_configuration["ApiBaseUrl"] + "api/Vehicle/add"), content);

        if (!response.IsSuccessStatusCode)
        {
            // Handle error response
            throw new HttpRequestException($"Error: {response.StatusCode}");
        }

        return await response.Content.ReadAsStringAsync();
    }

    public async Task<string> UpdateVehicles(VehicleViewModel vehicleModel, Guid Id, string jwtToken)
    {
        _logger.LogInformation("MVC_VehicleService_UpdateVehicle with VehicleId: " + Id);
        SetAuthorizationHeader(jwtToken);
        var jsonStr = JsonConvert.SerializeObject(vehicleModel);
        var content = new StringContent(jsonStr, Encoding.UTF8, "application/json");
        var response = await _httpClient.PutAsync(new Uri(_configuration["ApiBaseUrl"] + $"api/Vehicle/edit/{Id}"), content);

        if (!response.IsSuccessStatusCode)
        {
            // Handle error response
            throw new HttpRequestException($"Error: {response.StatusCode}");
        }

        return await response.Content.ReadAsStringAsync();
    }

    public async Task<VehicleViewModel> GetVehicleById(Guid id, string jwtToken)
    {
        _logger.LogInformation("MVC_VehicleService_GetVehicleById with VehicleId: " + id);
        SetAuthorizationHeader(jwtToken);
        var response = await _httpClient.GetAsync(new Uri(_configuration["ApiBaseUrl"] + $"api/Vehicle/get/{id}"));

        if (!response.IsSuccessStatusCode)
        {
            // Handle error response
            throw new HttpRequestException($"Error: {response.StatusCode}");
        }

        var responseString = await response.Content.ReadAsStringAsync();
        return JsonConvert.DeserializeObject<VehicleViewModel>(responseString);
    }

    public async Task<string> DeleteVehicle(Guid id, string jwtToken)
    {
        _logger.LogInformation("MVC_VehicleService_DeleteVehicle with VehicleId: " + id);
        SetAuthorizationHeader(jwtToken);
        var response = await _httpClient.DeleteAsync(new Uri(_configuration["ApiBaseUrl"] + $"api/Vehicle/delete/{id}"));

        if (!response.IsSuccessStatusCode)
        {
            // Handle error response
            throw new HttpRequestException($"Error: {response.StatusCode}");
        }

        return await response.Content.ReadAsStringAsync();
    }
    public async Task<MemoryStream> GetVehicleExcel(string jwtToken)
    {
        MemoryStream memoryStream = new MemoryStream();
        using (ExcelPackage excelPackage = new ExcelPackage(memoryStream))
        {
            ExcelWorksheet workSheet = excelPackage.Workbook.Worksheets.Add("VehiclesSheet");
            workSheet.Cells["A1"].Value = "VehicleNumber";
            workSheet.Cells["B1"].Value = "Description";
            workSheet.Cells["C1"].Value = "OwnerName";
            workSheet.Cells["D1"].Value = "OwnerAddress";
            workSheet.Cells["E1"].Value = "Contact No.";
            workSheet.Cells["F1"].Value = "Email";
            workSheet.Cells["G1"].Value = "VehicleClass";
            workSheet.Cells["H1"].Value = "FuelType";

            using (ExcelRange headerCells = workSheet.Cells["A1:H1"])
            {
                headerCells.Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;
                headerCells.Style.Fill.BackgroundColor.SetColor(System.Drawing.Color.LightBlue);
                headerCells.Style.Font.Bold = true;
            }

            int row = 2;
            List<VehicleViewModel> vehicles = await GetVehicles(jwtToken);
            foreach (var vehicle in vehicles)
            {
                workSheet.Cells[row, 1].Value = vehicle.VehicleNumber;
                workSheet.Cells[row, 2].Value = vehicle.Description;
                workSheet.Cells[row, 3].Value = vehicle.VehicleOwnerName;
                workSheet.Cells[row, 4].Value = vehicle.OwnerAddress;
                workSheet.Cells[row, 5].Value = vehicle.OwnerContactNumber;
                workSheet.Cells[row, 6].Value = vehicle.Email;
                workSheet.Cells[row, 7].Value = vehicle.VehicleClass;
                workSheet.Cells[row, 8].Value = vehicle.FuelType;

                row++;
            }

            workSheet.Cells[$"A1:H{row}"].AutoFitColumns();
            await excelPackage.SaveAsync();
        }

        memoryStream.Position = 0;
        return memoryStream;
    }
}
