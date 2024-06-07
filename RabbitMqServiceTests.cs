using MyRabbitMqTest.Service;
using FluentAssertions;
using Moq;
using RabbitMQ.Client;
using System.Text;

namespace MyRabbitMqTest;

public class RabbitMqServiceTests
{
    [Fact]
    public void SendMessage_ShouldPublishMessageToQueue()
    {
        // Arrange
        var mockConnection = new Mock<IConnection>();
        var mockChannel = new Mock<IRabbitMqChannel>();
        var mockModel = new Mock<IModel>();
        
        var service = new RabbitMqService(mockConnection.Object, mockChannel.Object);
        var queueName = "testQueue";
        var message = "Hello, RabbitMQ!";
        var body = Encoding.UTF8.GetBytes(message);

        var mockBasicProperties = new Mock<IBasicProperties>();
        mockModel.Setup(m => m.CreateBasicProperties()).Returns(mockBasicProperties.Object);
        mockConnection.Setup(c => c.CreateModel()).Returns(mockModel.Object);

        // Act
        service.SendMessage(message, queueName);

        // Assert
        mockChannel.Verify(
            m => m.BasicPublish(
                It.IsAny<string>(),
                queueName,
                It.IsAny<IBasicProperties>(),
                It.Is<byte[]>(b => b.SequenceEqual(body))),
            Times.Once);
    }

    [Fact]
    public void ReceiveMessage_ShouldReturnMessageFromQueue()
    {
        // Arrange
        var mockConnection = new Mock<IConnection>();
        var mockChannel = new Mock<IRabbitMqChannel>();

        var service = new RabbitMqService(mockConnection.Object, mockChannel.Object);
        var queueName = "testQueue";
        var message = "Hello, RabbitMQ!";
        var body = Encoding.UTF8.GetBytes(message);

        var mockBasicGetResult = new BasicGetResult(ulong.MaxValue, false, "", "", 0, null, body);
        mockChannel.Setup(m => m.BasicGet(queueName, false)).Returns(mockBasicGetResult);

        // Act
        var result = service.ReceiveMessage(queueName);

        // Assert
        result.Should().Be(message);
    }
}