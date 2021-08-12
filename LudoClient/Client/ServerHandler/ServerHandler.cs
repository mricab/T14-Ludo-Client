using System;
using System.IO;
using System.Text;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.Collections.Generic;
using System.Xml.Serialization;
using TcpProtocol;

namespace client
{
    public class ServerHandler
    {
        // Properties
        private static bool Handle;
        private Thread ClientThread;
        private bool quit = false;

        // Received Properties
        private TcpClient Client;
        private Type AppPackageType;

        // Events properties
        private List<IServerHandler> Observers;

        // Methods
        public ServerHandler(TcpClient Client, Type AppPackageType)
        {
            this.Client = Client;
            this.AppPackageType = AppPackageType;
            this.Observers = new List<IServerHandler>();
        }

        public void Start()
        {
            Handle = true;
            ClientThread = new Thread(new ThreadStart(Receive));
            ClientThread.Start();
        }

        public void Stop()
        {
            Handle = false;
        }

        // Handler Actions
        public void Send(Package package)
        {
            NetworkStream networkStream = Client.GetStream();
            XmlSerializer serializer = new XmlSerializer(typeof(Package), new Type[] { AppPackageType });
            Console.WriteLine("(Handler)\tSending to server.");
            serializer.Serialize(networkStream, package);
        }

        private void Receive()
        {
            Console.WriteLine("(Handler)\tHandler up.");
            byte[] bytes; // Incoming data buffer.
            NetworkStream networkStream = Client.GetStream();
            XmlSerializer deserializer = new XmlSerializer(typeof(Package), new Type[] { AppPackageType });

            while (Handle)
            {
                try
                {
                    bytes = new byte[Client.ReceiveBufferSize]; // 8192 Bytes
                    int BytesRead = networkStream.Read(bytes, 0, (int)Client.ReceiveBufferSize); // Receiving response
                    int TotalBytes = BytesRead;
                    while (BytesRead >= 1024)
                    {
                        BytesRead = networkStream.Read(bytes, TotalBytes, (int)Client.ReceiveBufferSize-TotalBytes);
                        TotalBytes = TotalBytes + BytesRead;
                    }                    
                    
                    MemoryStream memoryStream = new MemoryStream(bytes);
                    Package package = (Package)deserializer.Deserialize(memoryStream);
                    if (package.type == Protocol.TypeCode("message")) 
                    {
                        Console.WriteLine("(Handler)\tMessage package received ({0} Bytes).", TotalBytes);
                        OnServerMessageReceived(package.obj);
                    }
                    else if (package.type == Protocol.TypeCode("connected"))
                    {
                        Console.WriteLine("(Handler)\tConnection package received ({0} Bytes).", BytesRead);
                        OnServerConnected((int)package.obj);
                    }
                    else
                    {
                        //Console.WriteLine("(Handler)\tKeep package received ({0} Bytes).", BytesRead);
                    }
                }
                catch (InvalidOperationException) // System.Xml.XmlException: Root element is missing,
                {
                    //Console.WriteLine("(Handler)\tServer stopped transfering!");
                    //OnServerDisconnected();
                    //break;
                }
                catch (System.Xml.XmlException)
                {
                    Console.WriteLine("(Handler)\tServer stopped transfering!");
                    OnServerDisconnected();
                    break;
                }
                catch (IOException) // Timeout
                {
                    Console.WriteLine("(Handler)\tServer timed out!");
                    OnServerDisconnected();
                    break;
                }
                catch (SocketException)
                {
                    Console.WriteLine("(Handler)\tConection broken!");
                    OnServerDisconnected();
                    break;
                }
                Thread.Sleep(20); // 0.02s
            }

            networkStream.Close();
            Client.Close();
            Console.WriteLine("(Handler)\tHandler stopped.");
        }

        // Interface Methods
        public void RegisterObserver(IServerHandler observer)
        {
            Observers.Add(observer);
        }

        public void RemoveObserver(IServerHandler observer)
        {
            Observers.Remove(observer);
        }

        // Dispachers
        public void OnUserQuitted()
        {
            //UserQuitEvent e = new UserQuitEvent(this);
            //foreach (IServerHandler observer in Observers)
            //{
            //    observer.OnUserQuitted(e);
            //}
        }

        public void OnServerMessageReceived(Object obj)
        {
            ServerMessageData data = new ServerMessageData(obj);
            ServerMessageEvent e = new ServerMessageEvent(this, data);
            foreach (IServerHandler observer in Observers)
            {
                observer.OnServerMessageReceived(e);
            }
        }

        public void OnServerConnected(int connectionId)
        {
            ServerConnectedData data = new ServerConnectedData(connectionId);
            ServerConnectedEvent e = new ServerConnectedEvent(data);
            foreach (IServerHandler observer in Observers)
            {
                observer.OnServerConnected(e);
            }
        }

        public void OnServerDisconnected()
        {
            ServerDisconnectedEvent e = new ServerDisconnectedEvent();
            foreach (IServerHandler observer in Observers)
            {
                observer.OnServerDisconnected(e);
            }
        }

    }


}
