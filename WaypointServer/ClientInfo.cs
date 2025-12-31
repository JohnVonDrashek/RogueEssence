namespace WaypointServer
{
    /// <summary>
    /// Represents information about a connected client, including their data payload and activity state.
    /// Used to track client identity and matchmaking preferences during peer-to-peer connection establishment.
    /// </summary>
    public class ClientInfo
    {
        /// <summary>
        /// The raw data payload associated with this client, used for identification or state transfer.
        /// </summary>
        public byte[] Data;

        /// <summary>
        /// The current activity type of this client, used for matchmaking compatibility checks.
        /// </summary>
        public int Activity;

        /// <summary>
        /// The target activity type this client wants to connect to, used for matchmaking compatibility checks.
        /// </summary>
        public int ToActivity;

        /// <summary>
        /// Initializes a new instance of the <see cref="ClientInfo"/> class with the specified data and activity values.
        /// </summary>
        /// <param name="data">The raw data payload associated with the client.</param>
        /// <param name="activity">The current activity type of the client.</param>
        /// <param name="toActivity">The target activity type the client wants to connect to.</param>
        public ClientInfo(byte[] data, int activity, int toActivity)
        {
            Data = data;
            Activity = activity;
            ToActivity = toActivity;
        }
    }
}
