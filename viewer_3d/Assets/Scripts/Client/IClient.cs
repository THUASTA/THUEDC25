using System;

namespace EDCViewer.Client
{
    /// <summary>
    /// Represents common interfaces for all communication clients.
    /// </summary>
    public interface IClient
    {
        /// <summary>
        /// Occurs when a message is received from the server.
        /// </summary>
#nullable enable
        public event EventHandler<Messages.IMessage>? AfterMessageReceiveEvent;


        public decimal BandWidth { get; }


        /// <summary>
        /// Sends a message to the server.
        /// </summary>
        /// <param name="message">The message to be sent.</param>
        public void Send(Messages.IMessage message);
    }
}
