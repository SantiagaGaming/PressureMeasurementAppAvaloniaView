using System.Threading.Tasks;

namespace PressureMeasurementAppAvaloniaView.Interfaces
{
    public interface IDialogService
    {
        Task<T> ShowDialog<T>(string viewModelName, object? parameter = null) where T : class;
    }
}
