using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System.Text;
using RabbitMQ.Client.Framing;

namespace MyRabbitMqTest.Service;

public class RabbitMqService
{
    private readonly IConnection _connection;
    private readonly IRabbitMqChannel _channel;

    public RabbitMqService(IConnection connection, IRabbitMqChannel channel)
    {
        _connection = connection;
        _channel = channel;
    }

    public void SendMessage(string message, string queueName, IBasicProperties? basicProperties = null)
    {
        var body = Encoding.UTF8.GetBytes(message);
        if (basicProperties == null)
        {
            var model = _connection.CreateModel();
            basicProperties = model.CreateBasicProperties();
        }
        _channel.BasicPublish(exchange: "", routingKey: queueName, basicProperties: basicProperties, body: body);
    }

    public string ReceiveMessage(string queueName)
    {
        var result = _channel.BasicGet(queueName, true);
        if (result == null) return string.Empty;

        var message = Encoding.UTF8.GetString(result.Body.ToArray());
        return message;
    }
}