using EdcHost.SlaveServers;
using Xunit;

namespace EdcHost.Tests.UnitTests.SlaveServers;

public class PacketFromSlaverTests
{
    [Fact]
    public void PacketFromSlave_ValidByteArray_ParsesDataCorrectly()
    {
        byte[] bytes = new byte[] { 0x55, 0xAA, 0x02, 0x00, 0x07, 0x03, 0x04 };
        var packet = new PacketFromSlave(bytes);
        Assert.Equal(3, packet.ActionType);
        Assert.Equal(4, packet.Param);
    }

    [Fact]
    public void PacketFromSlave_InvalidHeader_ThrowsException()
    {
        byte[] bytes1 = new byte[] { 0x55, 0xAB, 0x00, 0x02, 0x01, 0x03, 0x04 };
        byte[] bytes2 = new byte[] { 0x50, 0xAA, 0x00, 0x02, 0x01, 0x03, 0x04 };
        Assert.Throws<Exception>(() => new PacketFromSlave(bytes1));
        Assert.Throws<Exception>(() => new PacketFromSlave(bytes2));
    }

    [Fact]
    public void PacketFromSlave_InvalidDataLength_ThrowsException()
    {
        byte[] bytes = new byte[] { 0x55, 0xAA, 0x00 };
        Assert.Throws<Exception>(() => new PacketFromSlave(bytes));
    }

    [Fact]
    public void PacketFromSlave_InvalidChecksum_ThrowsException()
    {
        byte[] bytes = new byte[] { 0x55, 0xAA, 0x00, 0x02, 0x01, 0x03, 0x05 };
        Assert.Throws<Exception>(() => new PacketFromSlave(bytes));
    }

    [Theory]
    [InlineData(0, 0)]
    [InlineData(1, 1)]
    [InlineData(2, 2)]
    [InlineData(8, 8)]
    public void PacketFromSlave_CorrectlyInitilized(int exp_actionType, int exp_param)
    {
        PacketFromSlave packet = new(exp_actionType, exp_param);
        Assert.Equal(exp_actionType, packet.ActionType);
        Assert.Equal(exp_param, packet.Param);
    }

    [Fact]
    public void ToBytes_ReturnsCorrectByteArray()
    {
        int exp_actionType = 1;
        int exp_param = 2;
        PacketFromSlave packet = new(exp_actionType, exp_param);
        byte[] result = packet.ToBytes();
        byte[] expectedBytes = new byte[]
        {
            0x55, 0xAA, // Header
            0x02, 0x00, // Data length (2 bytes)
            0x03,       // Checksum
            0x01,       // ActionType
            0x02        // Param
        };
        Assert.Equal(expectedBytes, result);
    }




}
