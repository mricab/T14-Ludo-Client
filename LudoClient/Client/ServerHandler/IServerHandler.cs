using System;
namespace client
{
    public interface IServerHandler
    {
        void OnServerMessageReceived(ServerMessageEvent e);
        void OnServerConnected(ServerConnectedEvent e);
        void OnServerDisconnected(ServerDisconnectedEvent e);
        void OnUserQuitted(UserQuitEvent e);
    }
}
