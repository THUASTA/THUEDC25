using EdcHost.SlaveServers;
using Xunit;

namespace EdcHost.Tests.UnitTests.SlaveServers;

public class PacketFromHostTests
{
    [Fact]
    public void PacketFromHost_CorrectlyInitialized()
    {
        int exp_gameStage = 0;
        int exp_elapsedTime = 12000;
        List<int> exp_heightOfChunks = Enumerable.Repeat(0, 64).ToList();
        bool exp_hasBed = true;
        bool exp_hasBedOpponent = true;
        float exp_positionX = 0.4f;
        float exp_positionY = 0.4f;
        float exp_positionOpponentX = 7.4f;
        float exp_positionOpponentY = 7.4f;
        int exp_agility = 0;
        int exp_health = 8;
        int exp_MaxHealth = 8;
        int exp_strength = 4;
        int exp_emeraldCount = 64;
        int exp_woolcount = 64;
        List<int> exp_owningOreKindOfChunks = Enumerable.Repeat(0, 64).ToList();
        List<int> exp_informationOfChunks = Enumerable.Repeat(0, 64).ToList();
        PacketFromHost packetFromHost = new(
            exp_gameStage, exp_elapsedTime, exp_heightOfChunks, exp_hasBed, exp_hasBedOpponent,
            exp_positionX, exp_positionY, exp_positionOpponentX, exp_positionOpponentY, exp_agility,
            exp_health, exp_MaxHealth, exp_strength, exp_emeraldCount, exp_woolcount,
            exp_owningOreKindOfChunks);
        Assert.Equal(exp_gameStage, packetFromHost.GameStage);
        Assert.Equal(exp_elapsedTime, packetFromHost.ElapsedTime);
        Assert.Equal(exp_informationOfChunks, packetFromHost.InformationOfChunks);
        Assert.Equal(exp_hasBed, packetFromHost.HasBed);
        Assert.Equal(exp_hasBedOpponent, packetFromHost.HasBedOpponent);
        Assert.Equal(exp_positionX, packetFromHost.PositionX);
        Assert.Equal(exp_positionY, packetFromHost.PositionY);
        Assert.Equal(exp_positionOpponentX, packetFromHost.PositionOpponentX);
        Assert.Equal(exp_positionOpponentY, packetFromHost.PositionOpponentY);
        Assert.Equal(exp_agility, packetFromHost.Agility);
        Assert.Equal(exp_health, packetFromHost.Health);
        Assert.Equal(exp_MaxHealth, packetFromHost.MaxHealth);
        Assert.Equal(exp_strength, packetFromHost.Strength);
        Assert.Equal(exp_strength, packetFromHost.Strength);
        Assert.Equal(exp_emeraldCount, packetFromHost.EmeraldCount);
        Assert.Equal(exp_woolcount, packetFromHost.WoolCount);
    }

    [Fact]
    public void ToBytes_CorrectlyCoverted()
    {
        int exp_gameStage = 0;
        int exp_elapsedTime = 12000;
        // len=64 content all zero
        List<int> exp_heightOfChunks = Enumerable.Repeat(0, 64).ToList();
        bool exp_hasBed = true;
        bool exp_hasBedOpponent = true;
        float exp_positionX = 0.4f;
        float exp_positionY = 0.4f;
        float exp_positionOpponentX = 7.4f;
        float exp_positionOpponentY = 7.4f;
        int exp_agility = 0;
        int exp_health = 8;
        int exp_MaxHealth = 8;
        int exp_strength = 4;
        int exp_emeraldCount = 64;
        int exp_woolcount = 64;
        List<int> exp_owningOreKindOfChunks = Enumerable.Repeat(0, 64).ToList();
        List<int> exp_informationOfChunks = Enumerable.Repeat(0, 64).ToList();
        PacketFromHost exp_Packet = new(
            exp_gameStage, exp_elapsedTime,
            exp_heightOfChunks, exp_hasBed, exp_hasBedOpponent,
            exp_positionX, exp_positionY, exp_positionOpponentX, exp_positionOpponentY,
            exp_agility, exp_health, exp_MaxHealth, exp_strength, exp_emeraldCount, exp_woolcount,
            exp_owningOreKindOfChunks
        );
        byte[] data = exp_Packet.ToBytes();
        PacketFromHost actualPacket = new(data);
        Assert.Equal(exp_Packet.GameStage, actualPacket.GameStage);
        Assert.Equal(exp_Packet.ElapsedTime, actualPacket.ElapsedTime);
        Assert.Equal(exp_Packet.InformationOfChunks.Count, actualPacket.InformationOfChunks.Count);
        for (int i = 0; i < exp_Packet.InformationOfChunks.Count; i++)
        {
            Assert.Equal(exp_Packet.InformationOfChunks[i], actualPacket.InformationOfChunks[i]);
        }

        Assert.Equal(exp_Packet.HasBed, actualPacket.HasBed);
        Assert.Equal(exp_Packet.HasBedOpponent, actualPacket.HasBedOpponent);
        Assert.Equal(exp_Packet.PositionX, actualPacket.PositionX);
        Assert.Equal(exp_Packet.PositionY, actualPacket.PositionY);
        Assert.Equal(exp_Packet.PositionOpponentX, actualPacket.PositionOpponentX);
        Assert.Equal(exp_Packet.PositionOpponentY, actualPacket.PositionOpponentY);
        Assert.Equal(exp_Packet.Agility, actualPacket.Agility);
        Assert.Equal(exp_Packet.Health, actualPacket.Health);
        Assert.Equal(exp_Packet.MaxHealth, actualPacket.MaxHealth);
        Assert.Equal(exp_Packet.Strength, actualPacket.Strength);
        Assert.Equal(exp_Packet.EmeraldCount, actualPacket.EmeraldCount);
        Assert.Equal(exp_Packet.WoolCount, actualPacket.WoolCount);
    }

}
