using System;
using System.Threading;
using LiteNetLib;

namespace WaypointServer
{
    /// <summary>
    /// Entry point class for the WaypointServer application.
    /// Initializes the server, sets up network event handlers, and runs the main server loop.
    /// </summary>
    class Program
    {
        /// <summary>
        /// Main entry point for the WaypointServer application.
        /// Initializes diagnostics, creates the connection manager, sets up network listeners,
        /// and runs an infinite loop to poll network events and display server status.
        /// </summary>
        /// <param name="args">Command-line arguments (not used).</param>
        static void Main(string[] args)
        {

            DiagManager.InitInstance();

            ConnectionManager connectionManager = new ConnectionManager();

            EventBasedNetListener listener = new EventBasedNetListener();
            NetManager server = new NetManager(listener);

            listener.ConnectionRequestEvent += connectionManager.ClientRequested;
            listener.PeerConnectedEvent += connectionManager.ClientConnected;
            listener.PeerDisconnectedEvent += connectionManager.ClientDisconnected;
            listener.NetworkReceiveEvent += connectionManager.NetworkReceived;

            server.Start(DiagManager.Instance.Port);

            while (true)
            {
                Console.Clear();
                Console.WriteLine("Name:{0}", DiagManager.Instance.ServerName);
                Console.WriteLine("Port:{0}", DiagManager.Instance.Port);
                Console.WriteLine("Searching:{0}/{1}", connectionManager.Searching, connectionManager.Peers);
                Console.WriteLine("Active:{0}/{1}", connectionManager.Active, connectionManager.Peers);
                Console.WriteLine("Errors:{0}", DiagManager.Instance.Errors);
                for (int ii = 0; ii < 100; ii++)
                {
                    server.PollEvents();
                    Thread.Sleep(15);
                }
            }
            //server.Stop(true);
        }

    }
}
