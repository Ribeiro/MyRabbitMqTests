using FluentAssertions;
using MyRabbitMqTest.Fixture;

namespace MyRabbitMqTest;

public class InMemoryRabbitMqServerTests : IClassFixture<RabbitMqServerFixture>
{
    private const string queueName = "testQueue";
    private const string message = "Hello, RabbitMQ!";

    private readonly RabbitMqServerFixture _fixture;

    public InMemoryRabbitMqServerTests(RabbitMqServerFixture fixture)
    {
        _fixture = fixture;
        _fixture.RabbitMqServer.DeclareQueue(queueName);
    }

    [Fact]
    async Task ReceiveMessage_ShouldBeNullDueToEptyQueue()
    {
        var messageFromQueue = await _fixture.RabbitMqServer.ConsumeMessageAsync(queueName);
        messageFromQueue.Should().BeNull();
    }

    [Fact]
    async Task ReceiveMessage_ShouldBeEqualtToSentMessage()
    {
        _fixture.RabbitMqServer.PublishMessage(queueName, message);

        var messageFromQueue = await _fixture.RabbitMqServer.ConsumeMessageAsync(queueName);
        messageFromQueue.Should().Be(message);
    }

}