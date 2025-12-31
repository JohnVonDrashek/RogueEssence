using LiteNetLib;
using LiteNetLib.Utils;

namespace RogueEssence.Network
{
    /// <summary>
    /// Represents the type of online activity being performed between players.
    /// </summary>
    public enum ActivityType
    {
        /// <summary>No activity type specified.</summary>
        None,
        /// <summary>Trading team members (characters) between players.</summary>
        TradeTeam,
        /// <summary>Trading items between players.</summary>
        TradeItem,
        /// <summary>Trading mail between players.</summary>
        TradeMail,
        /// <summary>Sending rescue help to another player.</summary>
        SendHelp,
        /// <summary>Receiving rescue help from another player.</summary>
        GetHelp
    }

    /// <summary>
    /// Abstract base class for all online multiplayer activities.
    /// Provides common functionality for network communication between players.
    /// </summary>
    public abstract class OnlineActivity
    {
        /// <summary>
        /// Gets the server information for the connection.
        /// </summary>
        public ServerInfo Server { get; private set; }

        /// <summary>
        /// Gets the contact information for the local player.
        /// </summary>
        public ContactInfo SelfInfo { get; private set; }

        /// <summary>
        /// Gets the contact information for the remote player.
        /// </summary>
        public ContactInfo TargetInfo { get; private set; }

        /// <summary>
        /// Gets the type of activity this instance represents.
        /// </summary>
        public abstract ActivityType Activity { get; }

        /// <summary>
        /// Gets the activity type that is compatible with this activity on the other end.
        /// </summary>
        public abstract ActivityType CompatibleActivity { get; }

        /// <summary>
        /// The packet processor for serializing and deserializing network packets.
        /// </summary>
        protected readonly NetPacketProcessor netPacketProcessor = new NetPacketProcessor();

        /// <summary>
        /// The network peer representing the connected partner.
        /// </summary>
        protected NetPeer partner;

        /// <summary>
        /// Initializes a new instance of the OnlineActivity class.
        /// </summary>
        /// <param name="server">The server information for the connection.</param>
        /// <param name="selfInfo">Contact information for the local player.</param>
        /// <param name="targetInfo">Contact information for the remote player.</param>
        public OnlineActivity(ServerInfo server, ContactInfo selfInfo, ContactInfo targetInfo)
        {
            Server = server;
            SelfInfo = selfInfo;
            TargetInfo = targetInfo;
        }

        /// <summary>
        /// Sets the network peer for this activity.
        /// </summary>
        /// <param name="peer">The network peer to associate with this activity.</param>
        public void SetPeer(NetPeer peer)
        {
            partner = peer;
        }

        /// <summary>
        /// Processes incoming network packets from the connected peer.
        /// </summary>
        /// <param name="peer">The network peer that sent the data.</param>
        /// <param name="reader">The packet reader containing the data.</param>
        /// <param name="deliveryMethod">The delivery method used for the packet.</param>
        public abstract void NetworkReceived(NetPeer peer, NetPacketReader reader, DeliveryMethod deliveryMethod);
    }
}
