using System.Collections.Generic;
using LiteNetLib;
using RogueEssence.Dungeon;

namespace RogueEssence.Network
{
    /// <summary>
    /// Represents an online activity for trading items between two players.
    /// Both players must be in TradeItem activity mode for the exchange to work.
    /// </summary>
    public class ActivityTradeItem : OnlineActivity
    {

        public override ActivityType Activity { get { return ActivityType.TradeItem; } }
        public override ActivityType CompatibleActivity { get { return ActivityType.TradeItem; } }

        /// <summary>
        /// Gets the list of items offered by the other player in the trade.
        /// </summary>
        public List<InvItem> OfferedItems { get; private set; }

        /// <summary>
        /// Gets the current state of the trade exchange process.
        /// </summary>
        public ExchangeState CurrentState { get; private set; }

        /// <summary>
        /// Initializes a new instance of the ActivityTradeItem class.
        /// </summary>
        /// <param name="server">The server information for the connection.</param>
        /// <param name="selfInfo">Contact information for the local player.</param>
        /// <param name="targetInfo">Contact information for the trading partner.</param>
        public ActivityTradeItem(ServerInfo server, ContactInfo selfInfo, ContactInfo targetInfo)
            : base(server, selfInfo, targetInfo)
        {
            netPacketProcessor.SubscribeNetSerializable<ExchangeItemState>((state) => OfferedItems = state.State);
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
        /// Sends a list of items being offered for trade to the trading partner.
        /// </summary>
        /// <param name="offer">The list of inventory items to offer in the trade.</param>
        public void OfferItems(List<InvItem> offer)
        {
            netPacketProcessor.SendNetSerializable(partner, new ExchangeItemState { State = offer }, DeliveryMethod.ReliableOrdered);
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
        /// Network packet wrapper for item list exchange.
        /// </summary>
        private class ExchangeItemState : WrapperPacket<List<InvItem>> { }

        /// <summary>
        /// Network packet wrapper for exchange state synchronization.
        /// </summary>
        private class ExchangeReadyState : WrapperPacket<ExchangeState> { }
    }

}
