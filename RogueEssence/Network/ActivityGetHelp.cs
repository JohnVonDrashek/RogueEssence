using LiteNetLib;
using RogueEssence.Data;

namespace RogueEssence.Network
{
    /// <summary>
    /// Represents an online activity where a player requests help (rescue) from another player.
    /// This activity handles the client side of the rescue request flow.
    /// </summary>
    public class ActivityGetHelp : OnlineActivity
    {

        public override ActivityType Activity { get { return ActivityType.GetHelp; } }
        public override ActivityType CompatibleActivity { get { return ActivityType.SendHelp; } }


        /// <summary>
        /// Gets the AOK (A-OK/rescue confirmation) mail offered by the helper player.
        /// </summary>
        public AOKMail OfferedMail { get; private set; }

        /// <summary>
        /// Gets the current state of the rescue exchange process.
        /// </summary>
        public ExchangeRescueState CurrentState { get; private set; }

        /// <summary>
        /// Initializes a new instance of the ActivityGetHelp class.
        /// </summary>
        /// <param name="server">The server information for the connection.</param>
        /// <param name="selfInfo">Contact information for the local player.</param>
        /// <param name="targetInfo">Contact information for the helper player.</param>
        public ActivityGetHelp(ServerInfo server, ContactInfo selfInfo, ContactInfo targetInfo)
            : base(server, selfInfo, targetInfo)
        {
            netPacketProcessor.SubscribeNetSerializable<ExchangeAOKState>((state) => OfferedMail = state.State);
            netPacketProcessor.SubscribeNetSerializable<ExchangeRescueReadyState>((state) => CurrentState = state.State);
            CurrentState = ExchangeRescueState.Communicating;
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

        /// <summary>
        /// Sends an SOS mail to the helper player requesting rescue.
        /// </summary>
        /// <param name="mail">The SOS mail containing rescue request details.</param>
        public void OfferMail(SOSMail mail)
        {
            netPacketProcessor.SendNetSerializable(partner, new ExchangeSOSState { State = mail }, DeliveryMethod.ReliableOrdered);
        }

        /// <summary>
        /// Sets and sends the current ready state to the helper player.
        /// </summary>
        /// <param name="state">The exchange rescue state to set.</param>
        public void SetReady(ExchangeRescueState state)
        {
            netPacketProcessor.SendNetSerializable(partner, new ExchangeRescueReadyState { State = state }, DeliveryMethod.ReliableOrdered);
        }
    }

    /// <summary>
    /// Network packet wrapper for SOS mail data exchange.
    /// </summary>
    public class ExchangeSOSState : WrapperPacket<SOSMail> { }

    /// <summary>
    /// Network packet wrapper for AOK (rescue confirmation) mail data exchange.
    /// </summary>
    public class ExchangeAOKState : WrapperPacket<AOKMail> { }

    /// <summary>
    /// Network packet wrapper for rescue exchange state synchronization.
    /// </summary>
    public class ExchangeRescueReadyState : WrapperPacket<ExchangeRescueState> { }

}
