using System;
namespace client
{
    public interface IClient
    {
        void OnMessageReceived(MessageEvent e);
        void OnConnected(ConnectedEvent e);
        void OnDisconnected(DisconnectedEvent e);
    }
}
