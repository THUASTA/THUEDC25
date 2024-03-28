namespace EdcHost.ViewerServers;

enum ErrorCode
{
    SocketServerError = 0,
    InvalidMessageType,
    NoSocketConnection,
    NoDeviceAvailable,
    InvalidCommand,
    InvalidPort,
    InvalidCamera,
    InvalidPlayer,
    CameraDuplication
}
