using System;
using System.Collections.Generic;
using client;
using LudoProtocol;
using LudoMatch;

namespace GameClient
{
    public class GClient : IClient
    {
        // Properties
        private static Client TClient;
        public static Match Match;
        private static string Token;
        private bool Connected = false;

        // Events properties
        private List<IGClient> Observers;
        private List<IGClient2> Observers2;
        private List<IGClient3> Observers3;

        // Constructor
        public GClient(int ServerPort)
        {
            TClient = new Client(ServerPort, typeof(LPackage));
            TClient.RegisterObserver(this);
            Observers = new List<IGClient>();
            Observers2 = new List<IGClient2>();
            Observers3 = new List<IGClient3>();
        }

        // Main Methods
        public void Start()
        {
            TClient.Start();
        }

        public void Send(string actionName, string[] contents = null)
        {
            LPackage lPackage = new LPackage();
            switch (actionName)
            {
                case "login":           lPackage = LProtocol.GetPackage(actionName, contents);  break;
                case "logout":          lPackage = LProtocol.GetPackage(actionName);            break;
                case "register":        lPackage = LProtocol.GetPackage(actionName, contents);  break;
                case "game create":     lPackage = LProtocol.GetPackage(actionName, contents);  break;
                case "game join":       lPackage = LProtocol.GetPackage(actionName, contents);  break;
                case "game play":       lPackage = LProtocol.GetPackage(actionName, contents);  break;
                case "game throw dice": lPackage = LProtocol.GetPackage(actionName, contents);  break;
                case "game move piece": lPackage = LProtocol.GetPackage(actionName, contents);  break;
                default:                break;
            }
            if(lPackage != null) {
                Console.WriteLine("(GClient)\tSending '{0}'["+ LProtocol.ActionCode(actionName) +"] to server.", actionName);
                TClient.Send(lPackage); 
            }
        }

        public bool IsConnected()
        {
            return Connected;
        }

        // Main Actions
        public void Login(string username, string password)
        {
            Send("login", new string[] { username, password });
        }
        public void StartGame(string boardType)
        {
            Send("game create", new string[] { boardType });
        }

        public void JoinGame(int gameId)
        {
            Send("game join", new string[] { gameId.ToString() });
        }

        public void EndTurn()
        {
            Send("game play", new string[] { Match.id.ToString() });
        }

        public void ThrowDice()
        {
            Send("game throw dice", new string[] { Match.id.ToString() });
        }

        public void MovePiece(int piece)
        {
            Send("game move piece", new string[] { Match.id.ToString(), piece.ToString() });
        }

        // Action Methods
        private void AnalizeRequest(LPackage lPackage)
        {
            string actionName = LProtocol.ActionName(lPackage.action);
            switch (actionName)
            {
                case "login successful":    SuccessfulLogin(lPackage.contents);     break;
                case "login failure":       LoginFailure(lPackage.contents);        break;
                case "logout successful":   SuccessfulLogout(lPackage.contents);    break;
                case "register successful": SuccessfulLogin(lPackage.contents);     break;
                case "game data":           StartGame(lPackage.contents);           break;
                case "game status":         UpdateGame(lPackage.contents);          break;
                default:                    break;
            }
        }

        private void SuccessfulLogin(string[] contents)
        {
            Token = contents[0];
            Console.WriteLine("(GClient)\tLogin confirmation received.");
            OnGLogin();
        }

        private void LoginFailure(string[] contents)
        {
            Console.WriteLine("(GClient)\tLogin failed.");
            OnGLoginFailure(contents[0]);
        }

        private void SuccessfulLogout(string[] contents)
        {
            Token = null;
            Console.WriteLine("(GClient)\tLogout confirmation received.");
            OnGLogout();
        }

        private void StartGame(string[] contents)
        {
            Console.WriteLine("(GClient)\tGame initialized (id and boardData received).");
            int id; int.TryParse(contents[0], out id);
            string[] boardData = new string[contents.Length-1]; Array.Copy(contents, 1, boardData, 0, contents.Length - 1);
            Match = new Match(id, boardData);
            OnGBoard();
        }

        public void UpdateGame(string[] contents)
        {
            Match.SetFromStatus(contents);
            OnGUpdate();
        }

        // IClient
        public void OnMessageReceived(MessageEvent e)
        {
            LPackage lPackage = (LPackage)e.data.obj;
            Console.WriteLine("(GClient)\tNew message [{0}].", lPackage.ToString());
            AnalizeRequest(lPackage);
        }

        public void OnConnected(ConnectedEvent e)
        {
            Connected = true;
            Console.WriteLine("(GClient)\tConnection {0} assigned by server.", e.data.connectionId);
            OnGConnected();
        }

        public void OnDisconnected(DisconnectedEvent e)
        {
            Connected = false;
            Console.WriteLine("(GClient)\tConnection lost.");
            OnGDisconnected();
        }

        // GClient dispatchers
        public void OnGConnected()
        {
            GConnectedEvent e = new GConnectedEvent(this);
            foreach (IGClient observer in Observers)
            {
                observer.OnGConnected(e);
            }
        }

        public void OnGDisconnected()
        {
            GDisconnectedEvent e = new GDisconnectedEvent(this);
            foreach (IGClient observer in Observers)
            {
                observer.OnGDisconnected(e);
            }
        }

        public void OnGLogin()
        {
            GLoginEvent e = new GLoginEvent(this);
            foreach (IGClient observer in Observers)
            {
                observer.OnGLogin(e);
            }
        }

        public void OnGLoginFailure(string message)
        {
            GLoginFailureData data = new GLoginFailureData(message);
            GLoginFailureEvent e = new GLoginFailureEvent(this, data);
            foreach (IGClient observer in Observers)
            {
                observer.OnGLoginFailure(e);
            }
        }

        // GClient2 dispatchers
        public void OnGLogout()
        {
            GLogoutEvent e = new GLogoutEvent(this);
            foreach (IGClient2 observer2 in Observers2)
            {
                observer2.OnGLogout(e);
            }
        }

        // GClient3 dispatchers
        public void OnGBoard()
        {
            GBoardEvent e = new GBoardEvent(this);
            foreach (IGClient3 observer3 in Observers3)
            {
                observer3.OnGBoard(e);
            }
        }

        public void OnGUpdate()
        {
            GUpdateEvent e = new GUpdateEvent(this);
            foreach (IGClient3 observer3 in Observers3)
            {
                observer3.OnGUpdate(e);
            }
        }

        // Interface Methods
        public void RegisterObserver(IGClient observer)
        {
            Observers.Add(observer);
        }

        public void RemoveObserver(IGClient observer)
        {
            Observers.Remove(observer);
        }

        public void RegisterObserver2(IGClient2 observer)
        {
            Observers2.Add(observer);
        }

        public void RemoveObserver2(IGClient2 observer)
        {
            Observers2.Remove(observer);
        }

        public void RegisterObserver3(IGClient3 observer)
        {
            Observers3.Add(observer);
        }

        public void RemoveObserver3(IGClient3 observer)
        {
            Observers3.Remove(observer);
        }
    }
}
