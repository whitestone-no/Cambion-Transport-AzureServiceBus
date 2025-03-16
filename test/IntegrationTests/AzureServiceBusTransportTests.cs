using System;
using System.Threading;
using System.Threading.Tasks;
using RandomTestValues;
using Xunit;

namespace Whitestone.Cambion.Transport.AzureServiceBus.IntegrationTests
{
    public class AzureServiceBusTransportTests(AzureServiceBusFixture fixture) : IClassFixture<AzureServiceBusFixture>
    {
        [Fact]
        public async Task PublishAsync_NullValue_ThrowsArgumentNullException()
        {
            await Assert.ThrowsAsync<ArgumentNullException>(async () => { await fixture.Transport.PublishAsync(null); });
        }

        [Fact]
        public async Task PublishAsync_SameDataReceived_Success()
        {
            // Arrange

            ManualResetEvent mre = new(false);
            byte[] expectedBytes = RandomValue.Array<byte>();

            byte[] actualBytes = null;
            fixture.Transport.MessageReceived += (_, e) =>
            {
                actualBytes = e.MessageBytes;
                mre.Set();
            };

            // Act

            await fixture.Transport.PublishAsync(expectedBytes);

            // Assert

            bool eventFired = mre.WaitOne(TimeSpan.FromSeconds(5));

            Assert.True(eventFired);
            Assert.Equal(expectedBytes, actualBytes);
        }
    }
}
