using System.Text;

using EdcHost.SlaveServers;

using Xunit;

namespace EdcHost.Tests.IntegrationTests;
public partial class SlaveServersTests
{
    [Theory]
    [InlineData("COM1", 9600)]
    public void Transmission(string portName, int baudRate)
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

        var serialPort = (SerialPortWrapperMock)serialPortHubMock.Get(portName, baudRate);

        int gameStage = 1;
        int elapsedTime = 100;
        List<int> heightOfChunks = Enumerable.Repeat(0, 64).ToList();
        for (int i = 0; i < heightOfChunks.Count; i++)
        {
            heightOfChunks[i] = i % 8;
        }
        heightOfChunks[0] = 8;

        bool hasBed = true;
        bool hasBedOpponent = true;
        double positionX = 1.0;
        double positionY = 1.0;
        double positionOpponentX = 1.0;
        double positionOpponentY = 1.0;
        int agility = 1;
        int health = 2;
        int maxHealth = 3;
        int strength = 4;
        int emeraldCount = 5;
        int woolCount = 6;
        List<int> owningOreKindOfChunks = Enumerable.Repeat(0, 64).ToList();
        for (int i = 0; i < owningOreKindOfChunks.Count; i++)
        {
            owningOreKindOfChunks[i] = i % 4;
        }
        owningOreKindOfChunks[0] = 3;

        PacketFromHost packet = new(
            gameStage, elapsedTime,
            heightOfChunks,
            hasBed, hasBedOpponent,
            (float)positionX, (float)positionY,
            (float)positionOpponentX, (float)positionOpponentY,
            agility, health, maxHealth, strength,
            emeraldCount, woolCount,
            owningOreKindOfChunks
        );

        byte[] bytes = packet.ToBytes();

        slaveServer.Publish(
            portName,
            gameStage, elapsedTime,
            heightOfChunks,
            hasBed, hasBedOpponent,
            positionX, positionY, positionOpponentX, positionOpponentY,
            agility, health, maxHealth, strength,
            emeraldCount, woolCount,
            owningOreKindOfChunks
        );

        byte[] bytesToSend = serialPort.WriteBuffer.ToArray();

        //Assertion
        Assert.Equal(bytes, bytesToSend);
    }
}
