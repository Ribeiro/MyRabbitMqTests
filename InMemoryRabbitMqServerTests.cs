using FluentAssertions;
using MyRabbitMqTest.Server;

namespace MyRabbitMqTest;

public class InMemoryRabbitMqServerTests
{
    private const string queueName = "testQueue";
    private const string message = "Hello, RabbitMQ!";

    private readonly InMemoryRabbitMqServer _inMemoryRabbitMqServer;

    public InMemoryRabbitMqServerTests()
    {
        using (_inMemoryRabbitMqServer = new InMemoryRabbitMqServer())
        {
            _inMemoryRabbitMqServer.DeclareQueue(queueName);
        }
    }
    
    [Fact]
    async Task ReceiveMessage_ShouldBeNullDueToEptyQueue()
    {
        var messageFromQueue = await _inMemoryRabbitMqServer.ConsumeMessageAsync(queueName);
        messageFromQueue.Should().BeNull();
    }

    [Fact]
    async Task ReceiveMessage_ShouldBeEqualtToSentMessage()
    {
        _inMemoryRabbitMqServer.PublishMessage(queueName, message);

        var messageFromQueue = await _inMemoryRabbitMqServer.ConsumeMessageAsync(queueName);
        messageFromQueue.Should().Be(message);
    }

}