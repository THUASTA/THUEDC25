using System.Text;
using EdcHost.SlaveServers;
using Xunit;

namespace EdcHost.Tests.IntegrationTests;

public partial class SlaveServersTests
{
    [Theory]
    [InlineData(2)]
    [InlineData(3)]
    [InlineData(5)]
    [InlineData(7)]
    [InlineData(11)]
    [InlineData(13)]
    [InlineData(17)]
    [InlineData(19)]
    [InlineData(23)]
    [InlineData(29)]
    [InlineData(31)]
    [InlineData(37)]
    [InlineData(41)]
    [InlineData(43)]
    [InlineData(47)]
    [InlineData(53)]
    [InlineData(59)]
    [InlineData(61)]
    [InlineData(67)]
    [InlineData(71)]
    [InlineData(73)]
    [InlineData(79)]
    [InlineData(83)]
    [InlineData(89)]
    [InlineData(97)]
    [InlineData(101)]
    [InlineData(103)]
    [InlineData(107)]
    [InlineData(109)]
    [InlineData(113)]
    [InlineData(127)]
    [InlineData(131)]
    [InlineData(137)]
    [InlineData(139)]
    [InlineData(149)]
    [InlineData(151)]
    [InlineData(157)]
    [InlineData(163)]
    [InlineData(167)]
    [InlineData(173)]
    [InlineData(179)]
    [InlineData(181)]
    [InlineData(191)]
    [InlineData(193)]
    [InlineData(197)]
    [InlineData(199)]
    public void Random(int randomSeed)
    {
        const int MaxLength = 100000;
        const int SlaveToHostBytesCount = 7;
        const int SlaveToHostDataBytesCount = 2;

        // Arrange
        Random random = new(randomSeed);
        string portName = Encoding.ASCII.GetString(Utils.GenerateRandomBytes(random, random.Next(0, MaxLength)));
        SerialPortWrapperMock serialPortWrapperMock = new(portName);
        SerialPortHubMock serialPortHubMock = new()
        {
            SerialPorts = {
                { portName, serialPortWrapperMock }
            }
        };
        ISlaveServer slaveServer = new SlaveServer(serialPortHubMock);

        // Act
        slaveServer.Start();
        slaveServer.OpenPort(portName, 0);

        Assert.True(serialPortWrapperMock.IsOpen);

        // Act: random bytes
        serialPortWrapperMock.MockReceive(Utils.GenerateRandomBytes(random, random.Next(0, MaxLength)));

        // Act: random body
        serialPortWrapperMock.MockReceive(MakePacket(Utils.GenerateRandomBytes(random, random.Next(0, MaxLength))));

        // Act: random value
        byte[] readBufferBytes = new byte[SlaveToHostBytesCount];
        readBufferBytes[5] = (byte)random.Next(0, 255);
        readBufferBytes[6] = (byte)random.Next(0, 255);
        // Headers should be generated after data bytes are set.
        readBufferBytes[0] = 0x55;
        readBufferBytes[1] = 0xAA;
        BitConverter.GetBytes((short)SlaveToHostDataBytesCount).CopyTo(readBufferBytes, 2);
        readBufferBytes[4] = CalculateChecksum(readBufferBytes.ToArray()[5..SlaveToHostBytesCount]);
        serialPortWrapperMock.MockReceive(readBufferBytes);
    }
}
