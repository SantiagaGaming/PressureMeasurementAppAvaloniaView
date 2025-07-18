using Avalonia.Controls;
using PressureMeasurementAppAvaloniaView.ViewModels;

namespace PressureMeasurementAppAvaloniaView.Views;

public partial class ConfirmDeleteWindow : Window
{
    public ConfirmDeleteWindow(int itemId, string itemTitle)
    {
        InitializeComponent();
        var vm = new ConfirmDeleteViewModel(itemId, itemTitle);
        vm.Close = result => Close(result);
        DataContext = vm;
    }
}