using System;
using System.Text;
using RogueEssence.Data;
using System.IO;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;

namespace RogueEssence
{
    /// <summary>
    /// Contains data about a player contact, including team information and profile.
    /// </summary>
    [Serializable]
    public class ContactData
    {
        /// <summary>
        /// The name of the team.
        /// </summary>
        public string TeamName;

        /// <summary>
        /// The rank designation of the team.
        /// </summary>
        public string Rank;

        /// <summary>
        /// The number of stars for the rank.
        /// </summary>
        public int RankStars;

        /// <summary>
        /// Profile pictures for team members.
        /// </summary>
        public ProfilePic[] TeamProfile;

        /// <summary>
        /// Initializes a new instance of the ContactData class.
        /// </summary>
        public ContactData()
        {
            TeamName = "";
            Rank = "";
            TeamProfile = new ProfilePic[0];
        }

        /// <summary>
        /// Gets a localized string representation of the rank with star icons.
        /// </summary>
        /// <returns>A string showing the rank stars, or "**Empty**" if no rank is set.</returns>
        public string GetLocalRankStr()
        {
            if (String.IsNullOrEmpty(Rank))
                return "**Empty**";
            return /*Rank.ToLocal() + */new string('\uE10C', RankStars);
        }
    }

    /// <summary>
    /// Contains contact information for a player, including UUID and contact data.
    /// </summary>
    [Serializable]
    public class ContactInfo
    {
        /// <summary>
        /// The unique identifier for this contact.
        /// </summary>
        public string UUID;

        /// <summary>
        /// The date of last contact with this player.
        /// </summary>
        public string LastContact;

        /// <summary>
        /// The detailed contact data including team information.
        /// </summary>
        public ContactData Data;

        /// <summary>
        /// Initializes a new instance of the ContactInfo class with empty values.
        /// </summary>
        public ContactInfo()
        {
            UUID = "";
            LastContact = "";
            Data = new ContactData();
        }

        /// <summary>
        /// Initializes a new instance of the ContactInfo class with a specified UUID.
        /// </summary>
        /// <param name="uuid">The unique identifier for this contact.</param>
        public ContactInfo(string uuid)
        {
            UUID = uuid;
            LastContact = "---";
            Data = new ContactData();
        }

        /// <summary>
        /// Serializes the contact data to a byte array.
        /// </summary>
        /// <returns>A byte array containing the serialized contact data.</returns>
        public byte[] SerializeData()
        {
            using (MemoryStream stream = new MemoryStream())
            {
                Serializer.SerializeData(stream, Data);
                return stream.ToArray();
            }
        }

        /// <summary>
        /// Deserializes contact data from a byte array.
        /// </summary>
        /// <param name="bytes">The byte array containing serialized contact data.</param>
        public void DeserializeData(byte[] bytes)
        {
            using (MemoryStream stream = new MemoryStream())
            {
                StringBuilder builder = new StringBuilder();
                stream.Write(bytes, 0, bytes.Length);
                stream.Position = 0;
                Data = (ContactData)Serializer.DeserializeData(stream);
            }
        }
    }

    /// <summary>
    /// Contact information for a peer in peer-to-peer networking, including IP and port.
    /// </summary>
    [Serializable]
    public class PeerInfo : ContactInfo
    {
        /// <summary>
        /// The IP address of the peer.
        /// </summary>
        public string IP;

        /// <summary>
        /// The port number of the peer.
        /// </summary>
        public int Port;

        /// <summary>
        /// Initializes a new instance of the PeerInfo class with empty values.
        /// </summary>
        public PeerInfo() : base()
        {
            IP = "";
        }

        /// <summary>
        /// Initializes a new instance of the PeerInfo class with specified IP and port.
        /// </summary>
        /// <param name="ip">The IP address of the peer.</param>
        /// <param name="port">The port number of the peer.</param>
        public PeerInfo(string ip, int port)
            : base("")
        {
            IP = ip;
            Port = port;
        }
    }

    /// <summary>
    /// Contains server connection information including name, IP, and port.
    /// </summary>
    [Serializable]
    public class ServerInfo
    {
        /// <summary>
        /// The display name of the server.
        /// </summary>
        public string ServerName;

        /// <summary>
        /// The IP address of the server.
        /// </summary>
        public string IP;

        /// <summary>
        /// The port number of the server.
        /// </summary>
        public int Port;

        /// <summary>
        /// Initializes a new instance of the ServerInfo class with empty values.
        /// </summary>
        public ServerInfo()
        {
            ServerName = "";
            IP = "";
        }

        /// <summary>
        /// Initializes a new instance of the ServerInfo class with specified values.
        /// </summary>
        /// <param name="serverName">The display name of the server.</param>
        /// <param name="ip">The IP address of the server.</param>
        /// <param name="port">The port number of the server.</param>
        public ServerInfo(string serverName, string ip, int port)
        {
            ServerName = serverName;
            IP = ip;
            Port = port;
        }
    }
}
