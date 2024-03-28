using System.Text;

using EdcHost.SlaveServers;

using Xunit;

namespace EdcHost.Tests.IntegrationTests;
public partial class SlaveServersTests
{
    [Theory]
    [InlineData(new byte[] { 0x01, 0x01 }, "COM1", 9600)]
    [InlineData(new byte[] { 0xFF, 0xFF }, "COM1", 9600)]
    [InlineData(new byte[] { 0x00, 0xFF }, "COM1", 9600)]
    [InlineData(new byte[] { 0x10, 0x01 }, "COM1", 9600)]
    [InlineData(new byte[] { 0xAA, 0x20 }, "COM1", 9600)]
    public void Reception(byte[] packetData, string portName, int baudRate)
    {
        SerialPortHubMock serialPortHubMock = new()
        {
            SerialPorts = {
                { portName, new SerialPortWrapperMock(portName) }
            }
        };
        var slaveServer = new SlaveServer(serialPortHubMock);
        slaveServer.Start();

        byte[] bytes = MakePacket(packetData);

        PacketFromSlave packet = new(bytes);

        var serialPortWrapperMock = (SerialPortWrapperMock)serialPortHubMock.Get(portName, baudRate);
        serialPortWrapperMock.AfterReceive += (sender, args) =>
        {
            PacketFromSlave packetReceived = new(args.Bytes);
            // Assertion
            Assert.Equal(bytes, args.Bytes);
            Assert.Equal(packetReceived.ActionType, packet.ActionType);
            Assert.Equal(packetReceived.Param, packet.Param);

            slaveServer.Stop();
        };
        serialPortWrapperMock.MockReceive(bytes);
    }
}
