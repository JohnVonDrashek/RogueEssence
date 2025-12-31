using System;
using LiteNetLib;
using System.Net;
using System.Net.Sockets;
using LiteNetLib.Utils;

namespace RogueEssence.Network
{
    /// <summary>
    /// Represents the current online connection status of the network manager.
    /// </summary>
    public enum OnlineStatus
    {
        /// <summary>Not connected to any network.</summary>
        Offline,
        /// <summary>Currently attempting to connect to a server.</summary>
        Connecting,
        /// <summary>Connected to server, looking for a partner to connect with.</summary>
        FindingPartner,
        /// <summary>Receiving connection from a partner.</summary>
        ReceivingPartner,
        /// <summary>Fully connected and communicating with a partner.</summary>
        Connected
    }

    /// <summary>
    /// Represents the reason for a network connection being closed.
    /// </summary>
    public enum NetCloseReason
    {
        /// <summary>No specific close reason.</summary>
        None,
        /// <summary>The other party ended the connection.</summary>
        OtherEnded,
        /// <summary>Already connected to another peer.</summary>
        AlreadyConnected,
        /// <summary>The activity types between peers do not match.</summary>
        DifferentActivity,
        /// <summary>The server is shutting down.</summary>
        ServerShutdown,
        /// <summary>The peer ID does not match the expected value.</summary>
        PeerIDMismatch
    }

    /// <summary>
    /// Manages all network connections and peer-to-peer communication for online activities.
    /// Uses LiteNetLib for underlying network operations.
    /// </summary>
    public class NetworkManager
    {
        private const string CONNECTION_KEY = "n09gBuU3h76ZyORXSlaiEkAT7tbOBG1";

        /// <summary>
        /// The default port number used for network connections.
        /// </summary>
        public const int DEFAULT_PORT = 1705;

        private const int SERVER_INTRO = 0;
        private const int CLIENT_INFO = 1;
        private const int SERVER_CONNECTED = 2;
        private const int CLIENT_DATA = 3;

        private static NetworkManager instance;

        /// <summary>
        /// Initializes the singleton instance of the NetworkManager.
        /// </summary>
        public static void InitInstance()
        {
            instance = new NetworkManager();
        }

        /// <summary>
        /// Gets the singleton instance of the NetworkManager.
        /// </summary>
        public static NetworkManager Instance { get { return instance; } }

        private NetManager client;
        private EventBasedNetListener clientListener;

        /// <summary>
        /// Gets the current online connection status.
        /// </summary>
        public OnlineStatus Status { get; private set; }

        /// <summary>
        /// Gets the exit message when a connection is closed.
        /// </summary>
        public string ExitMsg { get; private set; }

        /// <summary>
        /// Gets the current online activity being performed.
        /// </summary>
        public OnlineActivity Activity { get; private set; }

        /// <summary>
        /// Gets a value indicating whether the connection is peer-to-peer.
        /// </summary>
        public bool P2P { get; private set; }

        /// <summary>
        /// Initializes a new instance of the NetworkManager class.
        /// Sets up the network client and event listeners.
        /// </summary>
        public NetworkManager()
        {
            clientListener = new EventBasedNetListener();
            client = new NetManager(clientListener);
            client.MaxConnectAttempts = 100000000;//TODO: if litenetlib ever allows infinite, set it to that.
            client.ReconnectDelay = 500;
            clientListener.ConnectionRequestEvent += OnPeerRequested;
            clientListener.PeerConnectedEvent += OnPeerConnected;
            clientListener.PeerDisconnectedEvent += OnPeerDisconnected;
            clientListener.NetworkReceiveEvent += OnNetworkReceived;
            clientListener.NetworkErrorEvent += OnNetworkError;
            ExitMsg = "";
        }

        /// <summary>
        /// Prepares an online activity for connection.
        /// </summary>
        /// <param name="activity">The online activity to prepare.</param>
        public void PrepareActivity(OnlineActivity activity/*, bool p2p*/)
        {
            Activity = activity;
            //P2P = p2p;
        }

        /// <summary>
        /// Initiates a connection to the server specified in the current activity.
        /// </summary>
        public void Connect()
        {
            ExitMsg = "";
            Status = OnlineStatus.Connecting;
            client.Start(Activity.Server.Port);
            client.Connect(Activity.Server.IP, Activity.Server.Port, CONNECTION_KEY);
        }

        private void OnNetworkReceived(NetPeer peer, NetPacketReader reader, DeliveryMethod deliveryMethod)
        {
            //rely on current connection state to determine what packets are coming in.
            if (Status == OnlineStatus.Connecting)
            {
                int packetId = reader.GetInt();
                //TODO: stop the connection if it's not SERVER_INTRO?
                string serverName = reader.GetString();
                //if (!P2P)
                    Activity.Server.ServerName = serverName;
                //else
                //    Activity.TargetInfo.Data.TeamName = serverName;

                DiagManager.Instance.SaveSettings(DiagManager.Instance.CurSettings);
                NetDataWriter writer = new NetDataWriter();
                writer.Put(CLIENT_INFO);
                writer.Put(Activity.SelfInfo.UUID);
                writer.PutBytesWithLength(Activity.SelfInfo.SerializeData());
                writer.Put((int)Activity.Activity);
                writer.Put(Activity.TargetInfo.UUID);
                writer.Put((int)Activity.CompatibleActivity);
                peer.Send(writer, DeliveryMethod.ReliableOrdered);
                Status = OnlineStatus.FindingPartner;
            }
            else if (Status == OnlineStatus.FindingPartner)
            {
                int packetId = reader.GetInt();
                //TODO: stop the connection if it's not SERVER_CONNECTED?
                byte[] partnerData = reader.GetBytesWithLength();
                Activity.TargetInfo.DeserializeData(partnerData);
                Activity.TargetInfo.LastContact = String.Format("{0:yyyy-MM-dd}", DateTime.Now);
                DiagManager.Instance.SaveSettings(DiagManager.Instance.CurSettings);
                Activity.SetPeer(peer);
                Status = OnlineStatus.Connected;
            }
            else if (Status == OnlineStatus.ReceivingPartner)
            {
                int packetId = reader.GetInt();
                //TODO: stop the connection if it's not CLIENT_INFO?
                string partnerUuid = reader.GetString();
                if (partnerUuid != Activity.TargetInfo.UUID)
                    peerDisconnect(peer, NetCloseReason.PeerIDMismatch);
                else
                {
                    byte[] partnerData = reader.GetBytesWithLength();
                    Activity.TargetInfo.DeserializeData(partnerData);
                    Activity.TargetInfo.LastContact = String.Format("{0:yyyy-MM-dd}", DateTime.Now);
                    DiagManager.Instance.SaveSettings(DiagManager.Instance.CurSettings);

                    //compare activity for validity
                    ActivityType theirActivity = (ActivityType)reader.GetInt();
                    string yourUuid = reader.GetString();
                    if (yourUuid != Activity.SelfInfo.UUID)
                        peerDisconnect(peer, NetCloseReason.PeerIDMismatch);
                    else
                    {
                        ActivityType yourActivity = (ActivityType)reader.GetInt();
                        if (yourActivity != Activity.Activity || theirActivity != Activity.CompatibleActivity)
                            peerDisconnect(peer, NetCloseReason.DifferentActivity);
                        else
                        {
                            //Send your own info
                            NetDataWriter writer = new NetDataWriter();
                            writer.Put(SERVER_CONNECTED);
                            writer.PutBytesWithLength(Activity.SelfInfo.SerializeData());
                            peer.Send(writer, DeliveryMethod.ReliableOrdered);

                            Activity.SetPeer(peer);
                            Status = OnlineStatus.Connected;
                        }
                    }
                }
            }
            else if (Status == OnlineStatus.Connected)
            {
                Activity.NetworkReceived(peer, reader, deliveryMethod);
            }

            reader.Recycle();
        }

        private void OnPeerRequested(ConnectionRequest request)
        {
            try
            {
                if (request.RemoteEndPoint.Address.ToString() == Activity.Server.IP && request.RemoteEndPoint.Port == Activity.Server.Port && client.ConnectedPeerList.Count == 0)
                {
                    if (request.Data.GetString() == CONNECTION_KEY)
                    {
                        P2P = true;
                        request.Accept();
                    }
                }
                else
                    request.Reject();
            }
            catch (Exception ex)
            {
                DiagManager.Instance.LogError(ex);
            }
        }

        private void OnPeerConnected(NetPeer peer)
        {
            try
            {
                if (P2P)
                {
                    NetDataWriter writer = new NetDataWriter();
                    writer.Put(SERVER_INTRO);
                    writer.Put(Activity.SelfInfo.Data.TeamName);
                    peer.Send(writer, DeliveryMethod.ReliableOrdered);
                    Status = OnlineStatus.ReceivingPartner;
                }
            }
            catch (Exception ex)
            {
                DiagManager.Instance.LogError(ex);
            }
        }

        private void OnPeerDisconnected(NetPeer peer, DisconnectInfo disconnectInfo)
        {
            if (disconnectInfo.Reason == DisconnectReason.RemoteConnectionClose && disconnectInfo.AdditionalData.AvailableBytes > 0)
            {
                NetCloseReason reason = (NetCloseReason)disconnectInfo.AdditionalData.GetInt();
                ExitMsg = reason.ToLocal("msg");
            }
            else if (disconnectInfo.Reason == DisconnectReason.DisconnectPeerCalled)
            {
                //retain the existing exitmsg
            }
            else
                ExitMsg = disconnectInfo.Reason.ToLocal("msg");

            client.Stop();
            cleanup();
        }

        private void OnNetworkError(IPEndPoint endPoint, SocketError socketErrorCode)
        {
            DiagManager.Instance.LogError(new SocketException((int)socketErrorCode));
        }

        /// <summary>
        /// Polls for network events and processes any pending packets.
        /// Should be called regularly from the game loop.
        /// </summary>
        public void Update()
        {
            client.PollEvents();
        }

        /// <summary>
        /// Disconnects from the current connection and cleans up resources.
        /// </summary>
        public void Disconnect()
        {
            client.Stop();
            cleanup();
        }

        private void peerDisconnect(NetPeer peer, NetCloseReason reason)
        {
            ExitMsg = reason.ToLocal("msg");
            peer.Disconnect(createDCMsg((int)reason));
        }

        private void cleanup()
        {
            Status = OnlineStatus.Offline;
            Activity = null;
            P2P = false;
        }

        private NetDataWriter createDCMsg(int msgCode)
        {
            NetDataWriter writer = new NetDataWriter();
            writer.Put(msgCode);
            return writer;
        }
    }
}
