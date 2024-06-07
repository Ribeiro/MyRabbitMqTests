using RabbitMQ.Client;

namespace MyRabbitMqTest.Service;

public class RabbitMqChannel : IRabbitMqChannel
{
    private readonly IModel _channel;

    public RabbitMqChannel(IModel channel)
    {
        _channel = channel;
    }

    public void BasicPublish(string exchange, string routingKey, IBasicProperties basicProperties, byte[] body)
    {
        _channel.BasicPublish(exchange, routingKey, basicProperties, body);
    }

    public BasicGetResult BasicGet(string queue, bool autoAck)
    {
        return _channel.BasicGet(queue, autoAck);
    }
}