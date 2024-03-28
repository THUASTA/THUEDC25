using EdcHost.SlaveServers;
using Xunit;

namespace EdcHost.Tests.UnitTests.SlaveServers;
public class IPacketTests
{
    [Fact]
    public void CalculateChecksum_ReturnCorrectValue()
    {
        byte[] bytes = new byte[] { 0x01, 0x02, 0x03, 0x04 };
        byte checksum = IPacket.CalculateChecksum(bytes);
        Assert.Equal(0x04, checksum);
    }

    [Fact]
    public void GetPacketData_ReturnCorrectValue()
    {
        byte[] data = new byte[] { 0x01, 0x02, 0x03, 0x04 };
        byte[] header = IPacket.GeneratePacketHeader(data);
        Assert.Equal(0x55, header[0]);
        Assert.Equal(0xAA, header[1]);
        Assert.Equal(data.Length, BitConverter.ToInt16(header, 2));
    }

}
