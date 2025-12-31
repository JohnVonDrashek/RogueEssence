using System;
using System.IO;
using System.Xml;
using System.Text;
using System.Diagnostics;

namespace WaypointServer
{
    /// <summary>
    /// Provides diagnostic and configuration management services for the waypoint server.
    /// Implements a singleton pattern for centralized logging, error tracking, and server configuration.
    /// </summary>
    public class DiagManager
    {
        private static DiagManager instance;

        /// <summary>
        /// Initializes the singleton instance of the <see cref="DiagManager"/> class.
        /// Must be called before accessing <see cref="Instance"/>.
        /// </summary>
        public static void InitInstance()
        {
            instance = new DiagManager();
        }

        /// <summary>
        /// Gets the singleton instance of the <see cref="DiagManager"/> class.
        /// </summary>
        public static DiagManager Instance { get { return instance; } }

        /// <summary>
        /// The relative path to the log directory where log files are stored.
        /// </summary>
        public const string LOG_PATH = "Log/";

        /// <summary>
        /// The display name of the server, loaded from configuration.
        /// </summary>
        public string ServerName;

        /// <summary>
        /// The network port the server listens on, loaded from configuration.
        /// </summary>
        public int Port;

        /// <summary>
        /// The total count of errors that have been logged during this session.
        /// </summary>
        public int Errors;

        /// <summary>
        /// Initializes a new instance of the <see cref="DiagManager"/> class.
        /// Creates the log directory if it does not exist, then loads and saves settings.
        /// </summary>
        public DiagManager()
        {
            if (!Directory.Exists(LOG_PATH))
                Directory.CreateDirectory(LOG_PATH);

            LoadSettings();
            SaveSettings();
        }


        /// <summary>
        /// Logs an exception with full stack trace and inner exception details to both debug output and a daily log file.
        /// Increments the error counter for tracking purposes.
        /// </summary>
        /// <param name="exception">The exception to log.</param>
        public void LogError(Exception exception)
        {
            Errors++;

            StringBuilder errorMsg = new StringBuilder();
            errorMsg.Append(String.Format("[{0}] {1}", String.Format("{0:yyyy/MM/dd HH:mm:ss.fff}", DateTime.Now), exception.Message));
            errorMsg.Append("\n");
            Exception innerException = exception;
            int depth = 0;
            while (innerException != null)
            {
                errorMsg.Append("Exception Depth: " + depth);
                errorMsg.Append("\n");

                errorMsg.Append(innerException.ToString());
                errorMsg.Append("\n\n");

                innerException = innerException.InnerException;
                depth++;
            }


            Debug.Write(errorMsg);

            try
            {
                string filePath = LOG_PATH + String.Format("{0:yyyy-MM-dd}", DateTime.Now) + ".txt";

                using (StreamWriter writer = new StreamWriter(filePath, true))
                    writer.Write(errorMsg);
            }
            catch (Exception ex)
            {
                Debug.Write(ex.ToString());
            }
        }

        /// <summary>
        /// Logs an informational message with a timestamp to the daily log file.
        /// </summary>
        /// <param name="diagInfo">The diagnostic information message to log.</param>
        public void LogInfo(string diagInfo)
        {
            string fullMsg = String.Format("[{0}] {1}", String.Format("{0:yyyy/MM/dd HH:mm:ss.fff}", DateTime.Now), diagInfo);

            try
            {
                string filePath = LOG_PATH + String.Format("{0:yyyy-MM-dd}", DateTime.Now) + ".txt";

                using (StreamWriter writer = new StreamWriter(filePath, true))
                {
                    writer.WriteLine(fullMsg);
                    writer.WriteLine();
                }
            }
            catch (Exception ex)
            {
                Debug.Write(ex.ToString());
            }
        }

        /// <summary>
        /// Loads server settings from the Config.xml file.
        /// Sets default values for ServerName and Port if the file does not exist or cannot be parsed.
        /// </summary>
        public void LoadSettings()
        {
            string path = "Config.xml";

            ServerName = "Default Server";
            Port = 1705;

            //try to load from file
            if (File.Exists(path))
            {
                try
                {
                    XmlDocument xmldoc = new XmlDocument();
                    xmldoc.Load(path);

                    ServerName = xmldoc.SelectSingleNode("Config/ServerName").InnerText;
                    Port = Int32.Parse(xmldoc.SelectSingleNode("Config/Port").InnerText);

                }
                catch (Exception ex)
                {
                    DiagManager.Instance.LogError(ex);
                }
            }
        }

        /// <summary>
        /// Saves the current server settings to the Config.xml file.
        /// Writes the ServerName and Port values to an XML configuration file.
        /// </summary>
        public void SaveSettings()
        {
            string path = "Config.xml";
            XmlDocument xmldoc = new XmlDocument();

            XmlNode docNode = xmldoc.CreateElement("Config");
            xmldoc.AppendChild(docNode);

            appendConfigNode(xmldoc, docNode, "ServerName", ServerName);
            appendConfigNode(xmldoc, docNode, "Port", Port.ToString());

            xmldoc.Save(path);
        }

        private static void appendConfigNode(XmlDocument doc, XmlNode parentNode, string name, string text)
        {
            XmlNode node = doc.CreateElement(name);
            node.InnerText = text;
            parentNode.AppendChild(node);
        }

    }
}
