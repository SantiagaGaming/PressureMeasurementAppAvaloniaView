using PressureMeasurementAppAvaloniaView.Data.Dto;
using PressureMeasurementAppAvaloniaView.Data.Entities;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace PressureMeasurementAppAvaloniaView.Interfaces
{
    public interface IApiService
    {
        Task<IEnumerable<Measurement>> GetLatestMeasurements();
        Task<IEnumerable<Measurement>> GetMeasurementsByDate(DateTime from, DateTime till);
        Task<Measurement> AddMeasurement(CreateMeasurementRequest request);
        Task UpdateMeasurement(int id, Measurement measurement);
        Task DeleteMeasurement(int id);
        Task<byte[]> ExportToXlsx(DateTime? fromDate, DateTime? tillDate);
    }
}
