
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using PressureMeasurementAppAvaloniaView.Data.Entities;
using System;

namespace PressureMeasurementAppAvaloniaView.ViewModels
{
    public partial class EditMeasurementViewModel : ViewModelBase
    {
        private readonly Measurement _originalMeasurement;

        [ObservableProperty]
        private int _upperPressure;

        [ObservableProperty]
        private int _lowerPressure;

        [ObservableProperty]
        private int _heartbeat;

        public DateTime MeasureDate => _originalMeasurement.MeasureDate;

        [ObservableProperty]
        private string? _description;

        [ObservableProperty]
        private bool _smoking;

        [ObservableProperty]
        private bool _alcohol;

        [ObservableProperty]
        private bool _sport;

        [ObservableProperty]
        private bool _stretching;

        public PressureState? PressureState => _originalMeasurement.PressureState;

        public EditMeasurementViewModel(Measurement measurement)
        {
            _originalMeasurement = measurement ?? throw new ArgumentNullException(nameof(measurement));

            UpperPressure = measurement.UpperPressure;
            LowerPressure = measurement.LowerPressure;
            Heartbeat = measurement.Heartbeat;
            Description = measurement.Description;
            Smoking = measurement.Smoking;
            Alcohol = measurement.Alcohol;
            Sport = measurement.Sport;
            Stretching = measurement.Stretching;
        }

        [RelayCommand]
        private void Save()
        {
            var updatedMeasurement = new Measurement
            {
                Id = _originalMeasurement.Id,
                UpperPressure = this.UpperPressure,
                LowerPressure = this.LowerPressure,
                Heartbeat = this.Heartbeat,
                MeasureDate = this.MeasureDate, 
                Description = this.Description,
                Smoking = this.Smoking,
                Alcohol = this.Alcohol,
                Sport = this.Sport,
                Stretching = this.Stretching,
                PressureState = _originalMeasurement.PressureState
            };
            Close(updatedMeasurement);
        }

        [RelayCommand]
        private void Cancel() => Close(null);

        public Action<Measurement?> Close { get; set; }
    }
}
