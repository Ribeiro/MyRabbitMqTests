namespace MyRabbitMqTest.Fixture;

[CollectionDefinition("RabbitMqCollection")]
public class RabbitMqCollection : ICollectionFixture<RabbitMqServerFixture>
{
    // This class has no code, and is never created. Its purpose is simply
    // to be the place to apply [CollectionDefinition] and all the
    // ICollectionFixture<> interfaces.
}
