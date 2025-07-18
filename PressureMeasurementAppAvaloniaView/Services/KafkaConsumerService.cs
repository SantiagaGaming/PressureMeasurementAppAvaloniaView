using Confluent.Kafka;
using PressureMeasurementAppAvaloniaView.Interfaces;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace PressureMeasurementAppAvaloniaView.Services
{
    public class KafkaConsumerService : IKafkaConsumerService
    {
        private readonly IConsumer<Ignore, string> _consumer;
        private bool _disposed;

        public event Action<string> OnMessageReceived;

        public KafkaConsumerService(string bootstrapServers, string groupId)
        {
            var config = new ConsumerConfig
            {
                BootstrapServers = bootstrapServers,
                GroupId = groupId,
                AutoOffsetReset = AutoOffsetReset.Earliest,
                EnableAutoCommit = false
            };

            _consumer = new ConsumerBuilder<Ignore, string>(config).Build();
        }

        public async Task StartConsumingAsync(string topic, CancellationToken cancellationToken = default)
        {
            _consumer.Subscribe(topic);

            await Task.Run(() =>
            {
                while (!cancellationToken.IsCancellationRequested)
                {
                    try
                    {
                        var result = _consumer.Consume(cancellationToken);
                        OnMessageReceived?.Invoke(result.Message.Value);
                        _consumer.StoreOffset(result);
                    }
                    catch (OperationCanceledException)
                    {
                        // Игнорируем, так как это ожидаемо при отмене
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"Kafka consume error: {ex.Message}");
                    }
                }
            }, cancellationToken);
        }

        public void Dispose()
        {
            if (_disposed) return;

            _consumer?.Close();
            _consumer?.Dispose();
            _disposed = true;
        }
    }
}