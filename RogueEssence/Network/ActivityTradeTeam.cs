using LiteNetLib;
using RogueEssence.Dungeon;

namespace RogueEssence.Network
{
    /// <summary>
    /// Represents the various states of an exchange process between two players.
    /// </summary>
    public enum ExchangeState
    {
        /// <summary>No active exchange state.</summary>
        None,
        /// <summary>Player is selecting items/characters to trade.</summary>
        Selecting,
        /// <summary>Player is viewing the other party's offer.</summary>
        Viewing,
        /// <summary>Player has confirmed and is ready to trade.</summary>
        Ready,
        /// <summary>The exchange is currently in progress.</summary>
        Exchange,
        /// <summary>Waiting period after trade completion.</summary>
        PostTradeWait
    }

    /// <summary>
    /// Represents an online activity for trading team members (characters) between two players.
    /// Both players must be in TradeTeam activity mode for the exchange to work.
    /// </summary>
    public class ActivityTradeTeam : OnlineActivity
    {

        public override ActivityType Activity { get { return ActivityType.TradeTeam; } }
        public override ActivityType CompatibleActivity { get { return ActivityType.TradeTeam; } }

        /// <summary>
        /// Gets the character data offered by the other player in the trade.
        /// </summary>
        public CharData OfferedChar { get; private set; }

        /// <summary>
        /// Gets the current state of the team trade exchange process.
        /// </summary>
        public ExchangeState CurrentState { get; private set; }

        /// <summary>
        /// Initializes a new instance of the ActivityTradeTeam class.
        /// </summary>
        /// <param name="server">The server information for the connection.</param>
        /// <param name="selfInfo">Contact information for the local player.</param>
        /// <param name="targetInfo">Contact information for the trading partner.</param>
        public ActivityTradeTeam(ServerInfo server, ContactInfo selfInfo, ContactInfo targetInfo)
            : base(server, selfInfo, targetInfo)
        {
            netPacketProcessor.SubscribeNetSerializable<ExchangeCharState>((state) => OfferedChar = state.State);
            netPacketProcessor.SubscribeNetSerializable<ExchangeReadyState>((state) => CurrentState = state.State);
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

        /// <summary>
        /// Sends character data being offered for trade to the trading partner.
        /// </summary>
        /// <param name="chara">The character data to offer in the trade.</param>
        public void OfferChar(CharData chara)
        {
            netPacketProcessor.SendNetSerializable(partner, new ExchangeCharState { State = chara }, DeliveryMethod.ReliableOrdered);
        }

        /// <summary>
        /// Sets and sends the current ready state to the trading partner.
        /// </summary>
        /// <param name="state">The exchange state to set.</param>
        public void SetReady(ExchangeState state)
        {
            netPacketProcessor.SendNetSerializable(partner, new ExchangeReadyState { State = state }, DeliveryMethod.ReliableOrdered);
        }

        /// <summary>
        /// Network packet wrapper for character data exchange.
        /// </summary>
        private class ExchangeCharState : WrapperPacket<CharData> { }

        /// <summary>
        /// Network packet wrapper for exchange state synchronization.
        /// </summary>
        private class ExchangeReadyState : WrapperPacket<ExchangeState> { }
    }
}
