namespace EdcHost.ViewerServers;

public class AfterMessageReceiveEventArgs : EventArgs
{
    public Message Message { get; }

    public AfterMessageReceiveEventArgs(Message message)
    {
        Message = message;
    }
}
