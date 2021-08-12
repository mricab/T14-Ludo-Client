using System;
namespace client
{
    public class ConnectedEvent
    {
        public ConnectedData data;

        public ConnectedEvent(object source, ConnectedData data)
        {
            this.data = data;
        }
    }
}
