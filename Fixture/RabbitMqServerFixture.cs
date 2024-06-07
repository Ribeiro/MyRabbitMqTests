
using MyRabbitMqTest.Server;

namespace MyRabbitMqTest.Fixture;

public class RabbitMqServerFixture : IDisposable
{
    private bool _disposed = false;
    public InMemoryRabbitMqServer RabbitMqServer { get; }

    public RabbitMqServerFixture()
    {
        RabbitMqServer = new InMemoryRabbitMqServer();
    }

    protected virtual void Dispose(bool disposing)
    {
        if (!_disposed)
        {
            if (disposing)
            {
                // Dispose managed resources
                RabbitMqServer.Dispose();
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