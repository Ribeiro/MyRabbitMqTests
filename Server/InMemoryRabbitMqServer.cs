using System.Collections.Concurrent;
using System.Text;

namespace MyRabbitMqTest.Server;

public class InMemoryRabbitMqServer : IDisposable
{
    private readonly ConcurrentDictionary<string, ConcurrentQueue<Stream>> _queues;
    private bool _disposed = false;

    public InMemoryRabbitMqServer()
    {
        _queues = new ConcurrentDictionary<string, ConcurrentQueue<Stream>>();
    }

    public void DeclareQueue(string queueName)
    {
        _queues.TryAdd(queueName, new ConcurrentQueue<Stream>());
    }

    public void PublishMessage(string queueName, string message)
    {
        var body = new MemoryStream(Encoding.UTF8.GetBytes(message));
        _queues[queueName].Enqueue(body);
    }

public async Task<string?> ConsumeMessageAsync(string queueName)
    {
        if (!_queues.TryGetValue(queueName, out var queue))
        {
            throw new InvalidOperationException($"Queue '{queueName}' does not exist.");
        }

        if (queue.TryDequeue(out var body))
        {
            using (var reader = new StreamReader(body, Encoding.UTF8))
            {
                return await reader.ReadToEndAsync();
            }
        }

        // Wait for a message to be enqueued
        await Task.Delay(500); // Adjust delay as needed

        // Try dequeue again
        if (queue.TryDequeue(out body))
        {
            using (var reader = new StreamReader(body, Encoding.UTF8))
            {
                return await reader.ReadToEndAsync();
            }
        }

        return null; // No message available
    }

    protected virtual void Dispose(bool disposing)
    {
        if (!_disposed)
        {
            if (disposing)
            {
                // Dispose managed resources
                foreach (var queue in _queues.Values)
                {
                    while (queue.TryDequeue(out var stream))
                    {
                        stream.Dispose();
                    }
                }
                _queues.Clear();
            }

            // Dispose unmanaged resources
            _disposed = true;
        }
    }

    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }

}