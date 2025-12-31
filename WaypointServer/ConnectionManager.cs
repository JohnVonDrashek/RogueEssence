using System;
using System.Collections.Generic;
using LiteNetLib;
using LiteNetLib.Utils;

namespace WaypointServer
{
    /// <summary>
    /// Manages client connections and peer-to-peer matchmaking for the waypoint server.
    /// Handles connection requests, client registration, partner matching, and data relay between connected peers.
    /// </summary>
    public class ConnectionManager
    {
        private const int SERVER_INTRO = 0;
        private const int CLIENT_INFO = 1;
        private const int SERVER_CONNECTED = 2;
        private const int CLIENT_DATA = 3;

        private const int DC_CODE_OTHER_ENDED = 1;
        private const int DC_CODE_ALREADY_CONNECTED = 2;
        private const int DC_CODE_DIFFERENT_ACTIVITY = 3;
        private const int DC_CODE_SHUTDOWN = 4;

        private const string CONNECTION_KEY = "n09gBuU3h76ZyORXSlaiEkAT7tbOBG1";


        private Dictionary<long, NetPeer> peers;
        private TwoWayDict<long, string> clientIDs;
        private Dictionary<string, ClientInfo> info;
        private Dictionary<string, string> searchingConnections;
        private Dictionary<string, string> activeConnections;

        /// <summary>
        /// Gets the number of clients currently searching for a partner connection.
        /// </summary>
        public int Searching { get { return searchingConnections.Count; } }

        /// <summary>
        /// Gets the number of active peer-to-peer connections (counted as individual endpoints).
        /// </summary>
        public int Active { get { return activeConnections.Count; } }

        /// <summary>
        /// Gets the total number of connected peers.
        /// </summary>
        public int Peers { get { return peers.Count; } }

        /// <summary>
        /// Initializes a new instance of the <see cref="ConnectionManager"/> class with empty connection tracking dictionaries.
        /// </summary>
        public ConnectionManager()
        {
            peers = new Dictionary<long, NetPeer>();
            clientIDs = new TwoWayDict<long, string>();
            info = new Dictionary<string, ClientInfo>();
            searchingConnections = new Dictionary<string, string>();
            activeConnections = new Dictionary<string, string>();
        }

        /// <summary>
        /// Handles an incoming connection request from a client.
        /// Accepts the connection only if the client provides the correct connection key.
        /// </summary>
        /// <param name="request">The connection request from the client.</param>
        public void ClientRequested(ConnectionRequest request)
        {
            try
            {
                request.AcceptIfKey(CONNECTION_KEY);
            }
            catch (Exception ex)
            {
                DiagManager.Instance.LogError(ex);
            }
        }

        /// <summary>
        /// Handles a newly connected client peer.
        /// Registers the peer and sends the server introduction message containing the server name.
        /// </summary>
        /// <param name="peer">The newly connected network peer.</param>
        public void ClientConnected(NetPeer peer)
        {
            try
            {
                peers.Add(peer.Id, peer);
                NetDataWriter writer = new NetDataWriter();
                writer.Put(SERVER_INTRO);
                writer.Put(DiagManager.Instance.ServerName);
                peer.Send(writer, DeliveryMethod.ReliableOrdered);

            }
            catch (Exception ex)
            {
                DiagManager.Instance.LogError(ex);
            }
        }

        /// <summary>
        /// Handles client disconnection by cleaning up all associated state.
        /// Removes the peer from tracking, clears client info, and disconnects any active partner.
        /// </summary>
        /// <param name="peer">The disconnecting network peer.</param>
        /// <param name="disconnectInfo">Information about the disconnection reason.</param>
        public void ClientDisconnected(NetPeer peer, DisconnectInfo disconnectInfo)
        {
            try
            {
                peers.Remove(peer.Id);
                if (clientIDs.Forward.Contains(peer.Id))
                {
                    string uuid = clientIDs.Forward[peer.Id];
                    clientIDs.RemoveForward(peer.Id);
                    info.Remove(uuid);
                    string partnerUUID = null;
                    if (activeConnections.ContainsKey(uuid))
                    {
                        partnerUUID = activeConnections[uuid];
                        activeConnections.Remove(uuid);
                    }
                    if (searchingConnections.ContainsKey(uuid))
                        searchingConnections.Remove(uuid);

                    //disconnect partner if applicable
                    if (partnerUUID != null && clientIDs.Reverse.Contains(partnerUUID))
                    {
                        long partnerid = clientIDs.Reverse[partnerUUID];
                        peers[partnerid].Disconnect(createDCMsg(DC_CODE_OTHER_ENDED));
                    }
                }
            }
            catch (Exception ex)
            {
                DiagManager.Instance.LogError(ex);
            }
        }

        /// <summary>
        /// Processes incoming network data from a connected peer.
        /// For active connections, relays data to the partner peer.
        /// For new connections, handles client registration and matchmaking with potential partners.
        /// </summary>
        /// <param name="peer">The network peer that sent the data.</param>
        /// <param name="reader">The packet reader containing the received data.</param>
        /// <param name="deliveryMethod">The delivery method used for the packet.</param>
        public void NetworkReceived(NetPeer peer, NetPacketReader reader, DeliveryMethod deliveryMethod)
        {

            try
            {
                if (connectionActive(peer))
                {
                    string uuid = clientIDs.Forward[peer.Id];
                    string partnerUuid = activeConnections[uuid];
                    long partnerID = clientIDs.Reverse[partnerUuid];
                    NetPeer partnerPeer = peers[partnerID];

                    NetDataWriter writer = new NetDataWriter();
                    writer.Put(reader.GetRemainingBytes());
                    partnerPeer.Send(writer, DeliveryMethod.ReliableOrdered);
                }
                else
                {
                    int packetId = reader.GetInt();
                    string uuid = reader.GetString();
                    byte[] data = reader.GetBytesWithLength();
                    int activity = reader.GetInt();
                    string toUuid = reader.GetString();
                    int toActivity = reader.GetInt();

                    //stop the connection if it's not CLIENT_INFO
                    if (packetId != CLIENT_INFO)
                        peer.Disconnect();//bad packet!
                    else if (clientIDs.Reverse.Contains(uuid))
                        peer.Disconnect(createDCMsg(DC_CODE_ALREADY_CONNECTED));
                    else
                    {
                        clientIDs.Add(peer.Id, uuid);
                        info.Add(uuid, new ClientInfo(data, activity, toActivity));
                        if (searchingConnections.ContainsKey(toUuid))
                        {
                            searchingConnections.Remove(toUuid);
                            activeConnections.Add(uuid, toUuid);
                            activeConnections.Add(toUuid, uuid);

                            ClientInfo selfInfo = info[uuid];
                            ClientInfo partnerInfo = info[toUuid];

                            long partnerID = clientIDs.Reverse[toUuid];
                            NetPeer partnerPeer = peers[partnerID];

                            if (selfInfo.Activity != partnerInfo.ToActivity ||
                                selfInfo.ToActivity != partnerInfo.Activity)
                            {
                                peer.Disconnect(createDCMsg(DC_CODE_DIFFERENT_ACTIVITY));
                                partnerPeer.Disconnect(createDCMsg(DC_CODE_DIFFERENT_ACTIVITY));
                            }
                            else
                            {
                                {
                                    NetDataWriter writer = new NetDataWriter();
                                    writer.Put(SERVER_CONNECTED);
                                    writer.PutBytesWithLength(partnerInfo.Data);
                                    peer.Send(writer, DeliveryMethod.ReliableOrdered);
                                }
                                {
                                    NetDataWriter writer = new NetDataWriter();
                                    writer.Put(SERVER_CONNECTED);
                                    writer.PutBytesWithLength(selfInfo.Data);
                                    partnerPeer.Send(writer, DeliveryMethod.ReliableOrdered);
                                }
                            }
                        }
                        else
                            searchingConnections.Add(uuid, toUuid);
                    }
                }

                reader.Recycle();

            }
            catch (Exception ex)
            {
                DiagManager.Instance.LogError(ex);
            }
        }

        private bool connectionActive(NetPeer peer)
        {
            if (!clientIDs.Forward.Contains(peer.Id))
                return false;
            string uuid = clientIDs.Forward[peer.Id];
            return activeConnections.ContainsKey(uuid);
        }

        private NetDataWriter createDCMsg(int msgCode)
        {
            NetDataWriter writer = new NetDataWriter();
            writer.Put(msgCode);
            return writer;
        }
    }
}
