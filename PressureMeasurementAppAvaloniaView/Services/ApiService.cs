using PressureMeasurementAppAvaloniaView.Data.Dto;
using PressureMeasurementAppAvaloniaView.Data.Entities;
using PressureMeasurementAppAvaloniaView.Interfaces;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;

namespace PressureMeasurementAppAvaloniaView.Services
{
    public class ApiService :IApiService
    {
        private readonly HttpClient _httpClient;

        public ApiService(HttpClient httpClient, string baseUrl)
        {
            _httpClient = httpClient;
            _httpClient.BaseAddress = new Uri(baseUrl);
        }

        public async Task<IEnumerable<Measurement>> GetLatestMeasurements()
        {
            return await _httpClient.GetFromJsonAsync<IEnumerable<Measurement>>("PressureMeasurement/latest");
        }

        public async Task<IEnumerable<Measurement>> GetMeasurementsByDate(DateTime from, DateTime till)
        {
            return await _httpClient.GetFromJsonAsync<IEnumerable<Measurement>>(
                $"PressureMeasurement/withDates?from={from:yyyy-MM-dd}&till={till:yyyy-MM-dd}");
        }

        public async Task<Measurement> AddMeasurement(CreateMeasurementRequest request)
        {
            var response = await _httpClient.PostAsJsonAsync("PressureMeasurement/", request);
            response.EnsureSuccessStatusCode();
            return await response.Content.ReadFromJsonAsync<Measurement>();
        }

        public async Task UpdateMeasurement(int id, Measurement measurement)
        {
            var response = await _httpClient.PutAsJsonAsync($"PressureMeasurement/{id}", measurement);
            response.EnsureSuccessStatusCode();
        }

        public async Task DeleteMeasurement(int id)
        {
            var response = await _httpClient.DeleteAsync($"PressureMeasurement/{id}");
            response.EnsureSuccessStatusCode();
        }
        public async Task<byte[]> ExportToXlsx(DateTime? fromDate, DateTime? tillDate)
        {
            string endpoint = fromDate.HasValue && tillDate.HasValue
                ? $"reports/xlsxWithDates?from={fromDate.Value:yyyy-MM-dd}&till={tillDate.Value:yyyy-MM-dd}"
                : "reports/xlsxLatest";

            var response = await _httpClient.GetAsync(endpoint);
            response.EnsureSuccessStatusCode();
            return await response.Content.ReadAsByteArrayAsync();
        }
    }
}