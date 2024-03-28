using EdcHost.SlaveServers;
using Xunit;

namespace EdcHost.Tests.IntegrationTests;

public partial class SlaveServersTests
{
    [Fact]
    public void Simple()
    {
        const int HostToSlaveBytesCount = 98;
        const int HostToSlaveDataBytesCount = 93;
        const int SlaveToHostBytesCount = 7;
        const int SlaveToHostDataBytesCount = 2;
        const int ChunkCount = 64;
        const string PortName = "COM1";

        // Arrange
        SerialPortWrapperMock serialPortWrapperMock = new(PortName);
        SerialPortHubMock serialPortHubMock = new()
        {
            SerialPorts = {
                { PortName, serialPortWrapperMock }
            }
        };
        ISlaveServer slaveServer = new SlaveServer(serialPortHubMock);

        // Act
        slaveServer.Start();

        Assert.Single(serialPortHubMock.SerialPorts);
        Assert.False(serialPortWrapperMock.IsOpen);
        Assert.Empty(serialPortWrapperMock.WriteBuffer);

        // Act
        slaveServer.OpenPort(PortName, 0);

        Assert.Single(serialPortHubMock.SerialPorts);
        Assert.True(serialPortWrapperMock.IsOpen);
        Assert.Empty(serialPortWrapperMock.WriteBuffer);

        // Act
        slaveServer.Publish(
            PortName, 0, 0, Enumerable.Repeat<int>(0, ChunkCount).ToList(), false, false,
            0.0, 0.0, 0.0, 0.0, 0, 0, 0, 0, 0, 0, Enumerable.Repeat<int>(0, ChunkCount).ToList()
            );

        Assert.Single(serialPortHubMock.SerialPorts);
        Assert.True(serialPortWrapperMock.IsOpen);
        Assert.Equal(HostToSlaveBytesCount, serialPortWrapperMock.WriteBuffer.Count);

        List<byte> writeBuffer = serialPortWrapperMock.WriteBuffer;
        byte checksum = CalculateChecksum(writeBuffer.ToArray()[5..HostToSlaveBytesCount]);

        Assert.Equal((byte)0x55, writeBuffer[0]);
        Assert.Equal((byte)0xAA, writeBuffer[1]);
        Assert.Equal(HostToSlaveDataBytesCount,
            BitConverter.ToInt16(writeBuffer.ToArray(), 2));
        Assert.Equal(checksum, writeBuffer[4]);
        Assert.Equal((byte)0x00, writeBuffer[5]);
        Assert.Equal(0, BitConverter.ToInt32(writeBuffer.ToArray(), 6));
        for (int i = 0; i < 64; ++i)
        {
            Assert.Equal((byte)0x00, writeBuffer[10 + i]);
        }
        Assert.Equal((byte)0x00, writeBuffer[74]);
        Assert.Equal((byte)0x00, writeBuffer[75]);
        Assert.Equal(0.0f, BitConverter.ToSingle(writeBuffer.ToArray(), 76));
        Assert.Equal(0.0f, BitConverter.ToSingle(writeBuffer.ToArray(), 80));
        Assert.Equal(0.0f, BitConverter.ToSingle(writeBuffer.ToArray(), 84));
        Assert.Equal(0.0f, BitConverter.ToSingle(writeBuffer.ToArray(), 88));
        Assert.Equal((byte)0x00, writeBuffer[92]);
        Assert.Equal((byte)0x00, writeBuffer[93]);
        Assert.Equal((byte)0x00, writeBuffer[94]);
        Assert.Equal((byte)0x00, writeBuffer[95]);
        Assert.Equal((byte)0x00, writeBuffer[96]);
        Assert.Equal((byte)0x00, writeBuffer[97]);

        // Act
        bool isPlayerTryAttackEventRaised = false;
        slaveServer.PlayerTryAttackEvent += (sender, e) =>
        {
            Assert.Equal(PortName, e.PortName);
            Assert.Equal(0, e.TargetChunkId);
            isPlayerTryAttackEventRaised = true;
        };

        byte[] readBufferBytes = new byte[SlaveToHostBytesCount];
        readBufferBytes[5] = (byte)0x00;
        readBufferBytes[6] = (byte)0x00;
        // Headers should be generated after data bytes are set.
        readBufferBytes[0] = (byte)0x55;
        readBufferBytes[1] = (byte)0xAA;
        BitConverter.GetBytes((short)SlaveToHostDataBytesCount).CopyTo(readBufferBytes, 2);
        readBufferBytes[4] = CalculateChecksum(readBufferBytes.ToArray()[5..SlaveToHostBytesCount]);
        serialPortWrapperMock.MockReceive(readBufferBytes);

        Assert.Single(serialPortHubMock.SerialPorts);
        Assert.True(serialPortWrapperMock.IsOpen);
        Assert.Equal(HostToSlaveBytesCount, serialPortWrapperMock.WriteBuffer.Count);
        Assert.True(isPlayerTryAttackEventRaised);

        // Act
        bool isPlayerTryPlaceBlockEventRaised = false;
        slaveServer.PlayerTryPlaceBlockEvent += (sender, e) =>
        {
            Assert.Equal(PortName, e.PortName);
            Assert.Equal(0, e.TargetChunkId);
            isPlayerTryPlaceBlockEventRaised = true;
        };

        readBufferBytes = new byte[SlaveToHostBytesCount];
        readBufferBytes[5] = (byte)0x01;
        readBufferBytes[6] = (byte)0x00;
        // Headers should be generated after data bytes are set.
        readBufferBytes[0] = (byte)0x55;
        readBufferBytes[1] = (byte)0xAA;
        BitConverter.GetBytes((short)SlaveToHostDataBytesCount).CopyTo(readBufferBytes, 2);
        readBufferBytes[4] = CalculateChecksum(readBufferBytes.ToArray()[5..SlaveToHostBytesCount]);
        serialPortWrapperMock.MockReceive(readBufferBytes);

        Assert.Single(serialPortHubMock.SerialPorts);
        Assert.True(serialPortWrapperMock.IsOpen);
        Assert.Equal(HostToSlaveBytesCount, serialPortWrapperMock.WriteBuffer.Count);
        Assert.True(isPlayerTryPlaceBlockEventRaised);

        // Act
        bool isPlayerTryTradeEventRaised = false;
        slaveServer.PlayerTryTradeEvent += (sender, e) =>
        {
            Assert.Equal(PortName, e.PortName);
            Assert.Equal(0, e.Item);
            isPlayerTryTradeEventRaised = true;
        };

        readBufferBytes = new byte[SlaveToHostBytesCount];
        readBufferBytes[5] = (byte)0x02;
        readBufferBytes[6] = (byte)0x00;
        // Headers should be generated after data bytes are set.
        readBufferBytes[0] = (byte)0x55;
        readBufferBytes[1] = (byte)0xAA;
        BitConverter.GetBytes((short)SlaveToHostDataBytesCount).CopyTo(readBufferBytes, 2);
        readBufferBytes[4] = CalculateChecksum(readBufferBytes.ToArray()[5..SlaveToHostBytesCount]);
        serialPortWrapperMock.MockReceive(readBufferBytes);

        Assert.Single(serialPortHubMock.SerialPorts);
        Assert.True(serialPortWrapperMock.IsOpen);
        Assert.Equal(HostToSlaveBytesCount, serialPortWrapperMock.WriteBuffer.Count);
        Assert.True(isPlayerTryTradeEventRaised);

        // Act
        slaveServer.ClosePort(PortName);

        Assert.Single(serialPortHubMock.SerialPorts);
        Assert.False(serialPortWrapperMock.IsOpen);
        Assert.Equal(HostToSlaveBytesCount, serialPortWrapperMock.WriteBuffer.Count);

        // Act
        slaveServer.Stop();

        Assert.Single(serialPortHubMock.SerialPorts);
        Assert.False(serialPortWrapperMock.IsOpen);
        Assert.Equal(HostToSlaveBytesCount, serialPortWrapperMock.WriteBuffer.Count);
    }
}
