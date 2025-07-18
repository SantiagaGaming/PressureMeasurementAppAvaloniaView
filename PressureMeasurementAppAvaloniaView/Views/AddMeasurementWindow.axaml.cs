using Avalonia.Controls;
using PressureMeasurementAppAvaloniaView.ViewModels;

namespace PressureMeasurementAppAvaloniaView.Views;

public partial class AddMeasurementWindow : Window
{
    public AddMeasurementWindow(AddMeasurementViewModel viewModel)
    {
        InitializeComponent();
        viewModel.Close = result => Close(result);
        DataContext = viewModel;
    }
}