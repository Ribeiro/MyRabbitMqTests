
using RabbitMQ.Client;

namespace MyRabbitMqTest.Service;

public interface IRabbitMqChannel
{
    void BasicPublish(string exchange, string routingKey, IBasicProperties basicProperties, byte[] body);
    BasicGetResult BasicGet(string queue, bool autoAck);
    void BasicAck(ulong deliveryTag, bool multiple);
}