using Avalonia.Controls;
using PressureMeasurementAppAvaloniaView.Data.Entities;
using PressureMeasurementAppAvaloniaView.ViewModels;

namespace PressureMeasurementAppAvaloniaView.Views;

public partial class EditMeasurementWindow : Window
{
    public EditMeasurementWindow(Measurement measurement)
    {
        InitializeComponent();
        var viewModel = new EditMeasurementViewModel(measurement);
        viewModel.Close = result => Close(result);
        DataContext = viewModel;
    }
}