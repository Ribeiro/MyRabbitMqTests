using System.Collections.Concurrent;
using System.Text;

namespace MyRabbitMqTest.Server;

public class InMemoryRabbitMqServer : IDisposable
{
    private readonly ConcurrentDictionary<string, Queue<byte[]>> _queues;
    private bool _disposed = false;

    public InMemoryRabbitMqServer()
    {
        _queues = new ConcurrentDictionary<string, Queue<byte[]>>();
    }

    public void DeclareQueue(string queueName)
    {
        _queues.TryAdd(queueName, new Queue<byte[]>());
    }

    public void PublishMessage(string queueName, string message)
    {
        var body = Encoding.UTF8.GetBytes(message);
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
            return Encoding.UTF8.GetString(body);
        }

        // Wait for a message to be enqueued
        await Task.Delay(100); // Adjust delay as needed

        // Try dequeue again
        if (queue.TryDequeue(out body))
        {
            return Encoding.UTF8.GetString(body);
        }

        return null; // No message available
    }

    protected virtual void Dispose(bool disposing)
    {
        if (!_disposed)
        {
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