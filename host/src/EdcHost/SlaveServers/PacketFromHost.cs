namespace EdcHost.SlaveServers;

public class PacketFromHost : IPacketFromHost
{
    const int ChunkCount = 64;

    public int GameStage { get; private set; }
    public int ElapsedTime { get; private set; }
    public List<int> InformationOfChunks { get; private set; } = new();
    public bool HasBed { get; private set; }
    public bool HasBedOpponent { get; private set; }
    public float PositionX { get; private set; }
    public float PositionY { get; private set; }
    public float PositionOpponentX { get; private set; }
    public float PositionOpponentY { get; private set; }
    public int Agility { get; private set; }
    public int Health { get; private set; }
    public int MaxHealth { get; private set; }
    public int Strength { get; private set; }
    public int EmeraldCount { get; private set; }
    public int WoolCount { get; private set; }

    public PacketFromHost(byte[] bytes)
    {
        int datalength = (
           1 +                  // GameStage
           4 +                  // ElapsedTime
           1 * ChunkCount +     // InformationOfChunks
           1 +                  // HasBed
           1 +                  // HasBedOpponet
           4 * 4 +              // Position
           1 * 6                //agility health maxHealth strength emeraldCount woolCount
        );

        if (bytes.Length != datalength + 5)
        {
            throw new ArgumentException("bytes.Length != datalength + 5");
        }

        byte[] data = IPacket.GetPacketData(bytes);
        byte dataChecksum = IPacket.CalculateChecksum(data);
        byte checksum = bytes[4];

        if (dataChecksum != checksum)
        {
            throw new ArgumentException("dataChecksum != checksum");
        }

        int currentIndex = 0;

        GameStage = Convert.ToInt32(data[currentIndex++]);
        ElapsedTime = BitConverter.ToInt32(data, currentIndex);
        currentIndex += 4;

        for (int i = 0; i < 64; i++)
        {
            InformationOfChunks.Add(Convert.ToInt32(data[currentIndex++]));
        }

        HasBed = Convert.ToBoolean(data[currentIndex++]);
        HasBedOpponent = Convert.ToBoolean(data[currentIndex++]);

        PositionX = BitConverter.ToSingle(data, currentIndex);
        currentIndex += 4;
        PositionY = BitConverter.ToSingle(data, currentIndex);
        currentIndex += 4;

        PositionOpponentX = BitConverter.ToSingle(data, currentIndex);
        currentIndex += 4;
        PositionOpponentY = BitConverter.ToSingle(data, currentIndex);
        currentIndex += 4;

        Agility = Convert.ToInt32(data[currentIndex++]);
        Health = Convert.ToInt32(data[currentIndex++]);
        MaxHealth = Convert.ToInt32(data[currentIndex++]);
        Strength = Convert.ToInt32(data[currentIndex++]);
        EmeraldCount = Convert.ToInt32(data[currentIndex++]);
        WoolCount = Convert.ToInt32(data[currentIndex++]);
    }

    public PacketFromHost(
        int gameStage, int elapsedTime, List<int> heightOfChunks, bool hasBed, bool hasBedOpponent,
        float positionX, float positionY, float positionOpponentX, float positionOpponentY,
        int agility, int health, int maxHealth, int strength,
        int emeraldCount, int woolCount, List<int> owningOreKindOfChunks
        )
    {
        if (heightOfChunks.Count != ChunkCount)
        {
            throw new ArgumentException("heightOfChunks.Count != ChunkCount");
        }

        GameStage = gameStage;
        ElapsedTime = elapsedTime;

        InformationOfChunks = new(heightOfChunks);
        for (int i = 0; i < owningOreKindOfChunks.Count; i++)
        {
            InformationOfChunks[i] += (owningOreKindOfChunks[i] << 4);
        }

        HasBed = hasBed;
        HasBedOpponent = hasBedOpponent;

        PositionX = positionX;
        PositionY = positionY;
        PositionOpponentX = positionOpponentX;
        PositionOpponentY = positionOpponentY;

        Agility = agility;
        Health = health;
        MaxHealth = maxHealth;
        Strength = strength;
        EmeraldCount = emeraldCount;
        WoolCount = woolCount;
    }

    public byte[] ToBytes()
    {
        int datalength = (
           1 +                  // GameStage
           4 +                  // ElapsedTime
           1 * ChunkCount +     // InformationOfChunks
           1 +                  // HasBed
           1 +                  // HasBedOpponet
           4 * 4 +              // Position
           1 * 6                // agility health maxHealth strength emeraldCount woolCount
        );

        byte[] data = new byte[datalength];

        int currentIndex = 0;

        //GameStage
        data[currentIndex] = Convert.ToByte(GameStage);
        currentIndex++;

        //ElapsedTime
        BitConverter.GetBytes(ElapsedTime).CopyTo(data, currentIndex);
        currentIndex += 4;

        //InformationOfChunks
        for (int i = 0; i < InformationOfChunks.Count; i++)
        {
            data[currentIndex] = Convert.ToByte(InformationOfChunks[i]);
            currentIndex++;
        }

        //HasBed
        data[currentIndex] = Convert.ToByte(HasBed);
        currentIndex++;

        //HasBedOpponent
        data[currentIndex] = Convert.ToByte(HasBedOpponent);
        currentIndex++;

        //Position
        byte[] temp = BitConverter.GetBytes((float)PositionX);    //convert float to 4 bytes
        for (int i = 0; i < temp.Length; i++)
        {
            data[currentIndex] = temp[i];
            currentIndex++;
        }
        temp = BitConverter.GetBytes(PositionY);
        for (int i = 0; i < 4; i++)
        {
            data[currentIndex] = temp[i];
            currentIndex++;
        }
        temp = BitConverter.GetBytes(PositionOpponentX);
        for (int i = 0; i < 4; i++)
        {
            data[currentIndex] = temp[i];
            currentIndex++;
        }
        temp = BitConverter.GetBytes(PositionOpponentY);
        for (int i = 0; i < 4; i++)
        {
            data[currentIndex] = temp[i];
            currentIndex++;
        }


        //1 byte factors
        data[currentIndex++] = Convert.ToByte(Agility);
        data[currentIndex++] = Convert.ToByte(Health);
        data[currentIndex++] = Convert.ToByte(MaxHealth);
        data[currentIndex++] = Convert.ToByte(Strength);
        data[currentIndex++] = Convert.ToByte(EmeraldCount);
        data[currentIndex++] = Convert.ToByte(WoolCount);

        //add header
        byte[] header = IPacket.GeneratePacketHeader(data);
        byte[] bytes = new byte[header.Length + data.Length];
        header.CopyTo(bytes, 0);
        data.CopyTo(bytes, header.Length);
        return bytes;
    }
}
