using System.Text;
using EdcHost.ViewerServers;
using Xunit;

namespace EdcHost.Tests.IntegrationTests;

public partial class ViewerServersTests
{
    [Theory]
    [InlineData(2)]
    [InlineData(3)]
    [InlineData(5)]
    [InlineData(7)]
    [InlineData(11)]
    [InlineData(13)]
    [InlineData(17)]
    [InlineData(19)]
    [InlineData(23)]
    [InlineData(29)]
    [InlineData(31)]
    [InlineData(37)]
    [InlineData(41)]
    [InlineData(43)]
    [InlineData(47)]
    [InlineData(53)]
    [InlineData(59)]
    [InlineData(61)]
    [InlineData(67)]
    [InlineData(71)]
    [InlineData(73)]
    [InlineData(79)]
    [InlineData(83)]
    [InlineData(89)]
    [InlineData(97)]
    [InlineData(101)]
    [InlineData(103)]
    [InlineData(107)]
    [InlineData(109)]
    [InlineData(113)]
    [InlineData(127)]
    [InlineData(131)]
    [InlineData(137)]
    [InlineData(139)]
    [InlineData(149)]
    [InlineData(151)]
    [InlineData(157)]
    [InlineData(163)]
    [InlineData(167)]
    [InlineData(173)]
    [InlineData(179)]
    [InlineData(181)]
    [InlineData(191)]
    [InlineData(193)]
    [InlineData(197)]
    [InlineData(199)]
    public void Random(int randomSeed)
    {
        const int MinPort = 1;
        const int MaxPort = 65534;
        const int MaxLength = 100000;
        // const int MaxSize = 100;

        // Arrange
        Random random = new(randomSeed);
        int port = random.Next(MinPort, MaxPort);
        List<WebSocketConnectionMock> wsConnMockList = new()
        {
            new() {

            }
        };
        WebSocketServerMock wsServerMock = new(port);
        WebSocketServerHubMock wsServerHubMock = new()
        {
            Servers = {
                { port, wsServerMock }
            }
        };

        IViewerServer server = new ViewerServer(port, wsServerHubMock);

        // Act
        server.Start();
        foreach (WebSocketConnectionMock wsConnMock in wsConnMockList)
        {
            wsServerMock.AddConnection(wsConnMock);
        }

        Assert.True(wsServerMock.IsStarted);

        // Act
        foreach (WebSocketConnectionMock wsConnMock in wsConnMockList)
        {
            wsConnMock.OnOpen();
        }

        // Act
        foreach (WebSocketConnectionMock wsConnMock in wsConnMockList)
        {
            string message = Encoding.ASCII.GetString(Utils.GenerateRandomBytes(random, random.Next(0, MaxLength)));
            wsConnMock.OnMessage(message);
            wsConnMock.OnBinary(Encoding.ASCII.GetBytes(message));
        }

        // Act: random value - message
        foreach (WebSocketConnectionMock wsConnMock in wsConnMockList)
        {
            Message message = new()
            {
                MessageType = Encoding.ASCII.GetString(Utils.GenerateRandomBytes(random, random.Next(0, MaxLength))),
            };
            wsConnMock.OnMessage(message.Json);
            wsConnMock.OnBinary(Encoding.ASCII.GetBytes(message.Json));
        }

        // Act: random value - competition control command
        foreach (WebSocketConnectionMock wsConnMock in wsConnMockList)
        {
            CompetitionControlCommandMessage message = new()
            {
                Token = Encoding.ASCII.GetString(Utils.GenerateRandomBytes(random, random.Next(0, MaxLength))),
                Command = Encoding.ASCII.GetString(Utils.GenerateRandomBytes(random, random.Next(0, MaxLength))),
            };
            wsConnMock.OnMessage(message.Json);
            wsConnMock.OnBinary(Encoding.ASCII.GetBytes(message.Json));
        }

        // // Act: random value - host configuration from client
        // foreach (WebSocketConnectionMock wsConnMock in wsConnMockList)
        // {
        //     HostConfigurationFromClientMessage message = new()
        //     {
        //         Token = Encoding.ASCII.GetString(Utils.GenerateRandomBytes(random, random.Next(0, MaxLength))),
        //         Players = Enumerable.Range(0, random.Next(0, MaxSize)).Select(_ => new HostConfigurationFromClientMessage.PlayerType()
        //         {
        //             PlayerId = random.Next(),
        //             Camera = random.Next() % 2 == 0 ? null : new()
        //             {
        //                 CameraId = random.Next(),
        //                 Calibration = random.Next() % 2 == 0 ? null : new()
        //                 {
        //                     TopLeft = new()
        //                     {
        //                         X = random.NextSingle(),
        //                         Y = random.NextSingle(),
        //                     },
        //                     TopRight = new()
        //                     {
        //                         X = random.NextSingle(),
        //                         Y = random.NextSingle(),
        //                     },
        //                     BottomLeft = new()
        //                     {
        //                         X = random.NextSingle(),
        //                         Y = random.NextSingle(),
        //                     },
        //                     BottomRight = new()
        //                     {
        //                         X = random.NextSingle(),
        //                         Y = random.NextSingle(),
        //                     },
        //                 },
        //                 Recognition = new()
        //                 {
        //                     HueCenter = random.NextSingle(),
        //                     HueRange = random.NextSingle(),
        //                     SaturationCenter = random.NextSingle(),
        //                     SaturationRange = random.NextSingle(),
        //                     ValueCenter = random.NextSingle(),
        //                     ValueRange = random.NextSingle(),
        //                     MinArea = random.NextSingle(),
        //                     ShowMask = random.Next() % 2 == 0,
        //                 }
        //             },
        //             SerialPort = random.Next() % 2 == 0 ? null : new()
        //             {
        //                 PortName = Encoding.ASCII.GetString(Utils.GenerateRandomBytes(random, random.Next(0, MaxLength))),
        //                 BaudRate = random.Next(),
        //             }
        //         }).ToList(),
        //     };

        //     wsConnMock.OnMessage(message.Json);
        //     wsConnMock.OnBinary(Encoding.ASCII.GetBytes(message.Json));
        // }

        // // Act
        // foreach (WebSocketConnectionMock wsConnMock in wsConnMockList)
        // {
        //     wsConnMock.OnClose();
        // }
    }
}
