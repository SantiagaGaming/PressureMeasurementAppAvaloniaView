using System;

namespace PressureMeasurementAppAvaloniaView.Data.Entities
{
    public class Measurement
    {
        public int Id { get; set; }
        public int UpperPressure { get; set; }
        public int LowerPressure { get; set; }
        public int Heartbeat { get; set; }
        public DateTime MeasureDate { get; set; }
        public string? Description { get; set; }
        public bool Smoking { get; set; }
        public bool Alcohol { get; set; }
        public bool Sport { get; set; }
        public bool Stretching { get; set; }
        public PressureState? PressureState { get; set; }
    }
}
