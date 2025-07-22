using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using PressureMeasurementAppAvaloniaView.Data.Dto;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;


namespace PressureMeasurementAppAvaloniaView.ViewModels
{
    public partial class AddMeasurementViewModel : ViewModelBase
    {
        [ObservableProperty]
        private bool _multipleMeasurements = true;

        [ObservableProperty]
        private ObservableCollection<PressureDto> _pressures = new()
        {
            new PressureDto(),
            new PressureDto(),
            new PressureDto(),
            new PressureDto()
        };

        [ObservableProperty]
        private LifestyleDto _lifestyle = new();
        [RelayCommand]
        private void ToggleMultipleMeasurements()
        {
            MultipleMeasurements = !MultipleMeasurements;
        }

        [RelayCommand]
        private void Save()
        {
            var measurementsToValidate = MultipleMeasurements ? Pressures : Pressures.Take(1);
            
            if (measurementsToValidate.Any(p => p.UpperPressure <= 0 || p.LowerPressure <= 0 || p.Heartbeat <= 0))
                return;
            
            Close(new CreateMeasurementRequest
            {
                Pressures = new List<PressureDto>(measurementsToValidate),
                Lifestyle = Lifestyle
            });
        }

        [RelayCommand]
        private void Cancel() => Close?.Invoke(null);

        public Action<CreateMeasurementRequest?> Close { get; set; }
    }
}