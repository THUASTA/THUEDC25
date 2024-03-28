using System.Collections.Concurrent;
using System.Text;

using EdcHost.SlaveServers;

using Xunit;

namespace EdcHost.Tests.IntegrationTests;

public partial class SlaveServersTests
{
    [Theory]
    [InlineData(new int[] { 1, 1 }, 10000, "COM1", 9600)]
    [InlineData(new int[] { 255, 255 }, 10000, "COM1", 9600)]
    [InlineData(new int[] { 0, 255 }, 10000, "COM1", 9600)]
    [InlineData(new int[] { 16, 1 }, 10000, "COM1", 9600)]
    [InlineData(new int[] { 170, 32 }, 10000, "COM1", 9600)]
    public void Concurrency(int[] data, int clientCount, string portName, int baudRate)
    {
        SerialPortHubMock serialPortHubMock = new()
        {
            SerialPorts = {
                { portName, new SerialPortWrapperMock(portName) }
            }
        };
        var slaveServer = new SlaveServer(serialPortHubMock);
        slaveServer.Start();
        slaveServer.OpenPort(portName, baudRate);

        //data preparation
        byte[] byteData = new byte[data.Length];
        for (int i = 0; i < data.Length; i++)
        {
            byteData[i] = Convert.ToByte(data[i]);
        }
        byte[] header = IPacket.GeneratePacketHeader(byteData);
        byte[] bytes = new byte[header.Length + byteData.Length];
        header.CopyTo(bytes, 0);
        byteData.CopyTo(bytes, header.Length);
        ConcurrentBag<int> count = new();
        //do concurrency test
        var tasks = new List<Task>();
        var serialPortWrapperMock = (SerialPortWrapperMock)serialPortHubMock.Get(portName, baudRate);
        serialPortWrapperMock.AfterReceive += (sender, args) =>
        {
            PacketFromSlave packetReceived = new(args.Bytes);
            // Assertion
            Assert.Equal(bytes, args.Bytes);
            Assert.Equal(packetReceived.ActionType, packetReceived.ActionType);
            Assert.Equal(packetReceived.Param, packetReceived.Param);
            count.Add(1);
        };
        for (int i = 0; i < clientCount; i++)
        {
            tasks.Add(Task.Run(() =>
            {
                serialPortWrapperMock.MockReceive(bytes);
            }));
        }
        Task.WhenAll(tasks).Wait();
        Assert.Equal(clientCount, count.Count);
        slaveServer.Stop();
    }
}
