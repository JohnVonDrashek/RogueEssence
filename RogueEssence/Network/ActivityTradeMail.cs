using LiteNetLib;

namespace RogueEssence.Network
{
    /// <summary>
    /// Represents an online activity for trading mail between two players.
    /// Both players must be in TradeMail activity mode for the exchange to work.
    /// </summary>
    public class ActivityTradeMail : OnlineActivity
    {

        public override ActivityType Activity { get { return ActivityType.TradeMail; } }
        public override ActivityType CompatibleActivity { get { return ActivityType.TradeMail; } }

        /// <summary>
        /// Gets the current state of the mail exchange process.
        /// </summary>
        public ExchangeState CurrentState { get; private set; }

        /// <summary>
        /// Initializes a new instance of the ActivityTradeMail class.
        /// </summary>
        /// <param name="server">The server information for the connection.</param>
        /// <param name="selfInfo">Contact information for the local player.</param>
        /// <param name="targetInfo">Contact information for the mail exchange partner.</param>
        public ActivityTradeMail(ServerInfo server, ContactInfo selfInfo, ContactInfo targetInfo)
            : base(server, selfInfo, targetInfo)
        {
            CurrentState = ExchangeState.Selecting;
        }

        /// <summary>
        /// Processes incoming network packets from the connected peer.
        /// </summary>
        /// <param name="peer">The network peer that sent the data.</param>
        /// <param name="reader">The packet reader containing the data.</param>
        /// <param name="deliveryMethod">The delivery method used for the packet.</param>
        public override void NetworkReceived(NetPeer peer, NetPacketReader reader, DeliveryMethod deliveryMethod)
        {
            netPacketProcessor.ReadAllPackets(reader);
        }

    }

}
