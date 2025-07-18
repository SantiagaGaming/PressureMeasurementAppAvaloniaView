using System;
using System.Threading;
using System.Threading.Tasks;

namespace PressureMeasurementAppAvaloniaView.Interfaces
{
    public interface IKafkaConsumerService : IDisposable
    {
        event Action<string> OnMessageReceived;
        Task StartConsumingAsync(string topic, CancellationToken cancellationToken = default);
    }
}
