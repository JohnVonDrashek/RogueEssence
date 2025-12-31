using LiteNetLib;
using RogueEssence.Data;

namespace RogueEssence.Network
{
    /// <summary>
    /// Represents the various states of a rescue exchange process between two players.
    /// </summary>
    public enum ExchangeRescueState
    {
        /// <summary>No active exchange state.</summary>
        None,
        /// <summary>Players are currently communicating and setting up the exchange.</summary>
        Communicating,
        /// <summary>The SOS mail is ready to be sent.</summary>
        SOSReady,
        /// <summary>The SOS mail is currently being traded.</summary>
        SOSTrading,
        /// <summary>The AOK (rescue confirmation) mail is ready to be sent.</summary>
        AOKReady,
        /// <summary>The AOK mail is currently being traded.</summary>
        AOKTrading,
        /// <summary>The rescue exchange has been completed successfully.</summary>
        Completed
    }

    /// <summary>
    /// Represents an online activity where a player sends help (performs a rescue) for another player.
    /// This activity handles the helper side of the rescue flow.
    /// </summary>
    public class ActivitySendHelp : OnlineActivity
    {

        public override ActivityType Activity { get { return ActivityType.SendHelp; } }
        public override ActivityType CompatibleActivity { get { return ActivityType.GetHelp; } }

        /// <summary>
        /// Gets the SOS mail offered by the player requesting rescue.
        /// </summary>
        public SOSMail OfferedMail { get; private set; }

        /// <summary>
        /// Gets the current state of the rescue exchange process.
        /// </summary>
        public ExchangeRescueState CurrentState { get; private set; }

        /// <summary>
        /// Initializes a new instance of the ActivitySendHelp class.
        /// </summary>
        /// <param name="server">The server information for the connection.</param>
        /// <param name="selfInfo">Contact information for the local helper player.</param>
        /// <param name="targetInfo">Contact information for the player requesting help.</param>
        public ActivitySendHelp(ServerInfo server, ContactInfo selfInfo, ContactInfo targetInfo)
            : base(server, selfInfo, targetInfo)
        {
            netPacketProcessor.SubscribeNetSerializable<ExchangeSOSState>((state) => OfferedMail = state.State);
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
        /// Sends an AOK (rescue confirmation) mail to the player who requested help.
        /// </summary>
        /// <param name="mail">The AOK mail containing rescue confirmation details.</param>
        public void OfferMail(AOKMail mail)
        {
            netPacketProcessor.SendNetSerializable(partner, new ExchangeAOKState { State = mail }, DeliveryMethod.ReliableOrdered);
        }

        /// <summary>
        /// Sets and sends the current ready state to the player requesting help.
        /// </summary>
        /// <param name="state">The exchange rescue state to set.</param>
        public void SetReady(ExchangeRescueState state)
        {
            netPacketProcessor.SendNetSerializable(partner, new ExchangeRescueReadyState { State = state }, DeliveryMethod.ReliableOrdered);
        }

    }

}
