namespace EdcHost.Tests.IntegrationTests;

public partial class Utils
{
    public static byte[] GenerateRandomBytes(Random random, int count)
    {
        byte[] bytes = new byte[count];
        random.NextBytes(bytes);
        return bytes;
    }
}
