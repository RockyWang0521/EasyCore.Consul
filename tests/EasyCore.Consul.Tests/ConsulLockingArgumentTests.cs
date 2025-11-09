using EasyCore.Consul.Locking;
using FluentAssertions;
using Microsoft.Extensions.Logging.Abstractions;
using Moq;
using Xunit;

namespace EasyCore.Consul.Tests;

public class ConsulLockingArgumentTests
{
    [Fact]
    public async Task TryAcquireAsync_rejects_non_positive_ttl()
    {
        var locking = new ConsulLocking(
            new Mock<global::Consul.IConsulClient>().Object,
            NullLogger<ConsulLocking>.Instance);

        var act = async () => await locking.TryAcquireAsync("key", 0);
        await act.Should().ThrowAsync<ArgumentOutOfRangeException>();
    }
}
