using System.Text;
using LiteNetLib.Utils;
using System.IO;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using RogueEssence.Data;

namespace RogueEssence.Network
{
    /// <summary>
    /// Abstract generic wrapper class for network packets that provides serialization support.
    /// Wraps any serializable type T for transmission over the network using LiteNetLib.
    /// </summary>
    /// <typeparam name="T">The type of data to wrap in the packet.</typeparam>
    public abstract class WrapperPacket<T> : INetSerializable
    {
        /// <summary>
        /// Gets or sets the state data contained in this packet.
        /// </summary>
        public T State { get; set; }

        /// <summary>
        /// Deserializes the packet data from the network reader.
        /// </summary>
        /// <param name="reader">The network data reader containing the serialized data.</param>
        void INetSerializable.Deserialize(NetDataReader reader)
        {
            byte[] arr = reader.GetBytesWithLength();
            if (arr.Length == 0)
            {
                State = default(T);
                return;
            }

            using (MemoryStream stream = new MemoryStream())
            {
                StringBuilder builder = new StringBuilder();
                stream.Write(arr, 0, arr.Length);
                stream.Position = 0;
                State = (T)Serializer.Deserialize(stream, typeof(T));
            }
        }

        /// <summary>
        /// Serializes the packet data to the network writer.
        /// </summary>
        /// <param name="netWriter">The network data writer to write the serialized data to.</param>
        void INetSerializable.Serialize(NetDataWriter netWriter)
        {
            if (State == null)
            {
                byte[] bytes = new byte[0];
                netWriter.PutBytesWithLength(bytes);
                return;
            }

            using (MemoryStream stream = new MemoryStream())
            {
                Serializer.Serialize(stream, State);
                netWriter.PutBytesWithLength(stream.ToArray());
            }
        }
    }
}
