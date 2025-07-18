using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System;


namespace PressureMeasurementAppAvaloniaView.ViewModels
{
    public partial class ConfirmDeleteViewModel : ViewModelBase
    {
        [ObservableProperty]
        private string _confirmationText = string.Empty;

        public int ItemId { get; }

        public ConfirmDeleteViewModel(int itemId, string itemTitle)
        {
            ItemId = itemId;
            ConfirmationText = $"Are you sure you want to delete item '{itemTitle}' (ID: {itemId})?";
        }

        [RelayCommand]
        private void Delete() => Close(true);

        [RelayCommand]
        private void Cancel() => Close(false);

        public Action<bool> Close { get; set; }
    }
}
