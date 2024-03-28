namespace EdcHost.SlaveServers;

public interface IPacket
{
    static byte CalculateChecksum(byte[] bytes)
    {
        byte checksum = 0x00;
        foreach (byte byte_item in bytes)
        {
            checksum ^= byte_item;
        }
        return checksum;
    }

    /// <summary>
    /// Generate the header of some data.
    /// </summary>
    /// <param name="packetId">The packet ID.</param>
    /// <param name="data">The data.</param>
    /// <returns></returns>
    static byte[] GeneratePacketHeader(byte[] data)
    {
        short dataLength = (short)data.Length;
        byte checksum = IPacket.CalculateChecksum(data);

        byte[] header = new byte[5];
        header[0] = (byte)0x55;
        header[1] = (byte)0xAA;
        Array.Copy(BitConverter.GetBytes(dataLength), 0, header, 2, 2);
        header[4] = checksum;

        return header;
    }

    /// <summary>
    /// Extract the data from a packet in raw byte array form.
    /// </summary>
    /// <param name="bytes">
    /// The packet in raw byte array form.
    /// </param>
    /// <returns>The data.</returns>
    static byte[] GetPacketData(byte[] bytes)
    {
        // Validate the byte array
        if (bytes.Length < 5)
        {
            throw new Exception("The header of the packet is broken.");
        }

        if (bytes[0] != 0x55 || bytes[1] != 0xAA)
        {
            throw new Exception("The header of the packet is broken.");
        }

        short dataLength = BitConverter.ToInt16(bytes, 2);
        byte checksum = bytes[4];

        if (bytes.Length < dataLength + 5)
        {
            throw new Exception("The data length of the packet is incorrect.");
        }

        byte[] data = new byte[dataLength];
        Array.Copy(bytes, 5, data, 0, dataLength);

        if (checksum != CalculateChecksum(data))
        {
            throw new Exception("The data of the packet is broken.");
        }

        return data;
    }

    /// <summary>
    /// Get the data from the byte array without header to the packet object
    /// </summary>
    byte[] ToBytes();
}
