using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.ApplicationLifetimes;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Confluent.Kafka;
using PressureMeasurementAppAvaloniaView.Data.Dto;
using PressureMeasurementAppAvaloniaView.Data.Entities;
using PressureMeasurementAppAvaloniaView.Interfaces;
using PressureMeasurementAppAvaloniaView.Views;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace PressureMeasurementAppAvaloniaView.ViewModels;

public partial class MainViewModel : ViewModelBase, IDisposable
{
    private readonly IApiService _apiService;
    private readonly IConsumer<Ignore, string> _kafkaConsumer;
    private readonly CancellationTokenSource _cts = new();

    [ObservableProperty]
    private ObservableCollection<Measurement> _measurements = new();

    [ObservableProperty]
    private DateTimeOffset? _fromDate;

    [ObservableProperty]
    private DateTimeOffset? _tillDate;

    public MainViewModel(IApiService apiService, string kafkaServer)
    {
        _apiService = apiService;
        FromDate = DateTimeOffset.Now.AddDays(-7);
        TillDate = DateTimeOffset.Now;

        var kafkaConfig = new ConsumerConfig
        {
            BootstrapServers = kafkaServer,
            GroupId = "pressure-measurement-group",
            AutoOffsetReset = AutoOffsetReset.Earliest,
            EnableAutoCommit = false
        };

        _kafkaConsumer = new ConsumerBuilder<Ignore, string>(kafkaConfig).Build();
        _kafkaConsumer.Subscribe("pressure-events");

        Task.Run(ConsumeKafkaMessages);
    }
    private async Task ConsumeKafkaMessages()
    {
        while (!_cts.IsCancellationRequested)
        {
            try
            {
                var result = _kafkaConsumer.Consume(_cts.Token);

                await LoadMeasurements();

                _kafkaConsumer.StoreOffset(result);
            }
            catch (OperationCanceledException)
            {
             
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Kafka consume error: {ex.Message}");
            }
        }
    }

    public async Task InitializeAsync()
    {
        await LoadMeasurements();
    }

    [RelayCommand]
    private async Task LoadMeasurements()
    {
        try
        {
            var items = await _apiService.GetLatestMeasurements();
            Measurements = new ObservableCollection<Measurement>(items);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error loading measurements: {ex.Message}");
        }
    }

    [RelayCommand]
    private async Task Filter()
    {
        if (FromDate == null || TillDate == null) return;

        try
        {
            DateTime from = FromDate.Value.Date;
            DateTime till = TillDate.Value.Date;
            var items = await _apiService.GetMeasurementsByDate(from, till);
            Measurements = new ObservableCollection<Measurement>(items);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error filtering measurements: {ex.Message}");
        }
    }

    [RelayCommand]
    private async Task AddMeasurement()
    {
        var viewModel = new AddMeasurementViewModel();
        var dialog = new AddMeasurementWindow(viewModel);

        var result = await dialog.ShowDialog<CreateMeasurementRequest?>(GetMainWindow());

        if (result != null)
        {
            try
            {
                var created = await _apiService.AddMeasurement(result);
                Measurements.Insert(0, created);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error adding measurement: {ex.Message}");
            }
        }
    }

    [RelayCommand]
    private async Task EditMeasurement(Measurement measurement)
    {
        var dialog = new EditMeasurementWindow(measurement);
        var result = await dialog.ShowDialog<Measurement?>(GetMainWindow());

        if (result != null)
        {
            try
            {
                await _apiService.UpdateMeasurement(result.Id, result);

                var index = Measurements.IndexOf(measurement);
                if (index >= 0)
                {
                    Measurements[index] = result;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error updating measurement: {ex.Message}");
            }
        }
    }

    [RelayCommand]
    private async Task DeleteMeasurement(Measurement measurement)
    {
        var dialog = new ConfirmDeleteWindow(measurement.Id, $"Measurement from {measurement.MeasureDate:g}");
        var result = await dialog.ShowDialog<bool>(GetMainWindow());

        if (result)
        {
            try
            {
                await _apiService.DeleteMeasurement(measurement.Id);
                Measurements.Remove(measurement);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error deleting measurement: {ex.Message}");
            }
        }
    }
    [RelayCommand]
    private async Task ExportToExcel()
    {
        try
        {
            DateTime from = FromDate.Value.Date;
            DateTime till = TillDate.Value.Date;
            var fileBytes = await _apiService.ExportToXlsx(from, till);

            var saveFileDialog = new SaveFileDialog
            {
                Title = "Save Excel File",
                Filters = new List<FileDialogFilter>
            {
                new() { Name = "Excel Files", Extensions = { "xlsx" } }
            },
                InitialFileName = GenerateFileName()
            };

            var result = await saveFileDialog.ShowAsync((Application.Current.ApplicationLifetime as IClassicDesktopStyleApplicationLifetime)?.MainWindow);

            if (result != null)
            {
                await File.WriteAllBytesAsync(result, fileBytes);
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error exporting to Excel: {ex.Message}");
 
        }
    }

    private string GenerateFileName()
    {
        if (FromDate.HasValue && TillDate.HasValue)
        {
            return $"measurements_{FromDate.Value:yyyyMMdd}_to_{TillDate.Value:yyyyMMdd}.xlsx";
        }
        return $"measurements_{DateTime.Now:yyyyMMddHHmmss}.xlsx";
    }

    private Window GetMainWindow() =>
        (Application.Current.ApplicationLifetime as IClassicDesktopStyleApplicationLifetime)?.MainWindow
        ?? throw new InvalidOperationException("Main window not found");
    public void Dispose()
    {
        _cts.Cancel();
        _kafkaConsumer?.Close();
        _kafkaConsumer?.Dispose();
        _cts.Dispose();
    }
}
