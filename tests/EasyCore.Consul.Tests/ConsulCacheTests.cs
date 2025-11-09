using Consul;
using EasyCore.Consul.Cache;
using FluentAssertions;
using Microsoft.Extensions.Logging.Abstractions;
using Moq;
using System.Text;
using Xunit;

namespace EasyCore.Consul.Tests;

public class ConsulCacheTests
{
    [Fact]
    public async Task GetStringAsync_returns_null_when_missing()
    {
        var kv = new Mock<IKVEndpoint>();
        kv.Setup(x => x.Get(It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new QueryResult<KVPair?> { Response = null });

        var client = new Mock<IConsulClient>();
        client.SetupGet(x => x.KV).Returns(kv.Object);

        var cache = new ConsulCache(client.Object, NullLogger<ConsulCache>.Instance);
        var value = await cache.GetStringAsync("missing");
        value.Should().BeNull();
    }

    [Fact]
    public async Task GetAsync_string_does_not_json_deserialize()
    {
        var kv = new Mock<IKVEndpoint>();
        kv.Setup(x => x.Get("k", It.IsAny<CancellationToken>()))
            .ReturnsAsync(new QueryResult<KVPair?>
            {
                Response = new KVPair("k") { Value = Encoding.UTF8.GetBytes("plain-text") }
            });

        var client = new Mock<IConsulClient>();
        client.SetupGet(x => x.KV).Returns(kv.Object);

        var cache = new ConsulCache(client.Object, NullLogger<ConsulCache>.Instance);
        var value = await cache.GetAsync<string>("k");
        value.Should().Be("plain-text");
    }

    [Fact]
    public async Task PutAsync_object_serializes_json()
    {
        byte[]? written = null;
        var kv = new Mock<IKVEndpoint>();
        kv.Setup(x => x.Put(It.IsAny<KVPair>(), It.IsAny<CancellationToken>()))
            .Callback<KVPair, CancellationToken>((pair, _) => written = pair.Value)
            .ReturnsAsync(new WriteResult<bool> { Response = true });

        var client = new Mock<IConsulClient>();
        client.SetupGet(x => x.KV).Returns(kv.Object);

        var cache = new ConsulCache(client.Object, NullLogger<ConsulCache>.Instance);
        var ok = await cache.PutAsync("dto", new { Name = "abc" });

        ok.Should().BeTrue();
        Encoding.UTF8.GetString(written!).Should().Contain("abc");
    }
}
