using Xunit;

namespace Conexa.IntegrationTests;

[CollectionDefinition(Name)]
public class IntegrationTestCollection : ICollectionFixture<ConexaApiFactory>
{
    public const string Name = "Integration tests";
}
