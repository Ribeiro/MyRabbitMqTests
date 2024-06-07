using MyRabbitMqTest.Service;
using FluentAssertions;
using Moq;
using RabbitMQ.Client;
using System.Text;

namespace MyRabbitMqTest;

public class RabbitMqServiceTests
{
    private const string queueName = "testQueue";
    private const string message = "Hello, RabbitMQ!";

    [Fact]
    void SendMessage_ShouldPublishMessageToQueue()
    {
        // Arrange
        var mockConnection = new Mock<IConnection>();
        var mockChannel = new Mock<IRabbitMqChannel>();
        var mockModel = new Mock<IModel>();

        var mockBasicProperties = new Mock<IBasicProperties>();
        mockModel.Setup(m => m.CreateBasicProperties()).Returns(mockBasicProperties.Object);
        mockConnection.Setup(c => c.CreateModel()).Returns(mockModel.Object);

        var service = new RabbitMqService(mockConnection.Object, mockChannel.Object);

        // Act
        service.SendMessage(message, queueName);

        // Assert
        mockChannel.Verify(
            m => m.BasicPublish(
                It.IsAny<string>(),
                queueName,
                It.IsAny<IBasicProperties>(),
                It.Is<byte[]>(b => b.SequenceEqual(Encoding.UTF8.GetBytes(message)))),
            Times.Once);
    }

    [Fact]
    void ReceiveMessage_ShouldReturnMessageFromQueue()
    {
        // Arrange
        var mockConnection = new Mock<IConnection>();
        var mockChannel = new Mock<IRabbitMqChannel>();

        var mockBasicGetResult = new BasicGetResult(ulong.MaxValue, false, string.Empty, string.Empty, 0, null, Encoding.UTF8.GetBytes(message));
        mockChannel.Setup(m => m.BasicGet(queueName, false)).Returns(mockBasicGetResult);

        var service = new RabbitMqService(mockConnection.Object, mockChannel.Object);

        // Act
        var result = service.ReceiveMessage(queueName);

        // Assert
        result.Should().Be(message);
        mockChannel.Verify(m => m.BasicAck(mockBasicGetResult.DeliveryTag, false), Times.Once);
    }

}