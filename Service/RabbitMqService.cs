using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System.Text;
using RabbitMQ.Client.Framing;

namespace MyRabbitMqTest.Service;

public class RabbitMqService
{
    private readonly IConnection _connection;
    private readonly IRabbitMqChannel _channel;

    private const string emptyString = "";

    public RabbitMqService(IConnection connection, IRabbitMqChannel channel)
    {
        _connection = connection;
        _channel = channel;
    }

    public void SendMessage(string message, string queueName, IBasicProperties? basicProperties = null, string exchange = emptyString)
    {
        var body = Encoding.UTF8.GetBytes(message);
        if (basicProperties == null)
        {
            var model = _connection.CreateModel();
            basicProperties = model.CreateBasicProperties();
        }
        _channel.BasicPublish(exchange: exchange, routingKey: queueName, basicProperties: basicProperties, body: body);
    }

    public string ReceiveMessage(string queueName)
    {
        var result = _channel.BasicGet(queueName, false);
        if (result == null) return string.Empty;

        var message = Encoding.UTF8.GetString(result.Body.ToArray());

        _channel.BasicAck(result.DeliveryTag, false);

        return message;
    }
}