using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using JsonDiffPatchDotNet;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json.Serialization;
using RogueEssence.Dev;
using RogueEssence.LevelGen;

namespace RogueEssence.Data
{
    /// <summary>
    /// Wrapper class for serialized objects that includes version information.
    /// Used to support versioned data migration during deserialization.
    /// </summary>
    [Serializable]
    public class SerializationContainer
    {
        /// <summary>
        /// The version of the data format.
        /// </summary>
        public Version Version;

        /// <summary>
        /// The actual serialized object.
        /// </summary>
        public object Object;
    }

    /// <summary>
    /// Provides JSON serialization and deserialization utilities for game data.
    /// Supports versioned serialization, diff-based patching, and mod layering.
    /// </summary>
    public static class Serializer
    {
        /// <summary>
        /// The JSON serializer settings used for all serialization operations.
        /// </summary>
        public static JsonSerializerSettings Settings { get; private set; }

        private static JsonDiffPatch jdp;

        /// <summary>
        /// Initializes the serializer settings with custom contract resolver and binder.
        /// </summary>
        /// <param name="resolver">The contract resolver for customizing serialization behavior.</param>
        /// <param name="binder">The serialization binder for type resolution.</param>
        public static void InitSettings(IContractResolver resolver, ISerializationBinder binder)
        {
            Settings = new JsonSerializerSettings()
            {
                ContractResolver = resolver,
                SerializationBinder = binder,
                ConstructorHandling = ConstructorHandling.AllowNonPublicDefaultConstructor,
                TypeNameHandling = TypeNameHandling.Auto,
            };
            jdp = new JsonDiffPatch();
        }

        /// <summary>
        /// A value that is temporarily set when deserializing a data object, serving as a global old version for converters in UpgradeConverters.cs to recognize the version.
        /// A bit hacky, but is currently the only way for converters to recognize version.
        /// </summary>
        public static Version OldVersion;

        private static object lockObj = new object();

        /// <summary>
        /// Deserializes an object from a stream.
        /// </summary>
        /// <param name="stream">The stream containing JSON data.</param>
        /// <param name="type">The type to deserialize to.</param>
        /// <param name="converters">Optional custom JSON converters.</param>
        /// <returns>The deserialized object.</returns>
        public static object Deserialize(Stream stream, Type type, params JsonConverter[] converters)
        {
            lock (lockObj)
            {
                object obj;
                Version pastVersion = OldVersion;
                OldVersion = Versioning.GetVersion();
                using (StreamReader reader = new StreamReader(stream, Encoding.UTF8, true, -1, true))
                {
                    JsonSerializerSettings settingsCopy = new JsonSerializerSettings(Settings);
                    settingsCopy.Converters = converters;
                    obj = JsonConvert.DeserializeObject(reader.ReadToEnd(), type, settingsCopy);
                }
                OldVersion = pastVersion;
                return obj;
            }
        }

        /// <summary>
        /// Serializes an object to a stream using default settings.
        /// </summary>
        /// <param name="stream">The stream to write to.</param>
        /// <param name="entry">The object to serialize.</param>
        public static void Serialize(Stream stream, object entry)
        {
            using (StreamWriter writer = new StreamWriter(stream, Encoding.UTF8, -1, true))
            {
                string val = JsonConvert.SerializeObject(entry, Settings);
                writer.Write(val);
            }
        }

        /// <summary>
        /// Serializes an object to a stream with custom converters.
        /// </summary>
        /// <param name="stream">The stream to write to.</param>
        /// <param name="entry">The object to serialize.</param>
        /// <param name="converters">Custom JSON converters to use.</param>
        public static void Serialize(Stream stream, object entry, params JsonConverter[] converters)
        {
            using (StreamWriter writer = new StreamWriter(stream, Encoding.UTF8, -1, true))
            {
                JsonSerializerSettings settingsCopy = new JsonSerializerSettings(Settings);
                settingsCopy.Converters = converters;
                string val = JsonConvert.SerializeObject(entry, settingsCopy);
                writer.Write(val);
            }
        }

        /// <summary>
        /// Extracts the version from a serialized container string.
        /// </summary>
        /// <param name="containerStr">The JSON string containing the serialized container.</param>
        /// <returns>The version extracted from the container.</returns>
        public static Version GetVersion(string containerStr)
        {
            Version objVersion = new Version(0, 0);
            try
            {
                using (JsonTextReader textReader = new JsonTextReader(new StringReader(containerStr)))
                {
                    textReader.Read();
                    textReader.Read();
                    while (true)
                    {
                        if (textReader.TokenType == JsonToken.PropertyName && (string)textReader.Value == "Version")
                        {
                            textReader.Read();
                            objVersion = new Version((string)textReader.Value);
                            break;
                        }
                        else
                        {
                            textReader.Skip();
                            textReader.Read();
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                DiagManager.Instance.LogError(ex);
            }
            return objVersion;
        }

        /// <summary>
        /// Sets the version in a JToken container.
        /// </summary>
        /// <param name="containerToken">The container token to modify.</param>
        /// <param name="version">The version to set.</param>
        public static void SetVersion(JToken containerToken, Version version)
        {
            string versionStr = version.ToString();
            containerToken["Version"] = versionStr;
        }


        /// <summary>
        /// Deserializes data from a base file with diff patches applied.
        /// Used for mod layering where mods provide diffs instead of full files.
        /// </summary>
        /// <param name="path">The path to the base data file.</param>
        /// <param name="diffpaths">Paths to diff patch files to apply in order.</param>
        /// <returns>The deserialized object with all diffs applied.</returns>
        public static object DeserializeDataWithDiffs(string path, params string[] diffpaths)
        {
            lock (lockObj)
            {
                //read the base string
                JToken containerToken;
                //Currently, we can only load the base version when deserializing
                //TODO: diffs must ALWAYS have a version attached.  This way we can point the modder to go back to a version before failure
                Version baseOldVersion;
                using (Stream stream = new FileStream(path, FileMode.Open, FileAccess.Read, FileShare.Read))
                {
                    using (StreamReader reader = new StreamReader(stream, Encoding.UTF8, true, -1, true))
                    {
                        string containerStr = reader.ReadToEnd();
                        baseOldVersion = GetVersion(containerStr);
                        containerToken = JToken.Parse(containerStr);
                    }
                }

                //for each diff, read and apply
                for (int ii = 0; ii < diffpaths.Length; ii++)
                {
                    try
                    {
                        using (Stream stream = new FileStream(diffpaths[ii], FileMode.Open, FileAccess.Read, FileShare.Read))
                        {
                            using (StreamReader reader = new StreamReader(stream, Encoding.UTF8, true, -1, true))
                            {
                                string diffStr = reader.ReadToEnd();
                                JToken diff = JToken.Parse(diffStr);
                                // Apply the patch
                                // Note: version is also changed: is this a problem?
                                JToken newToken = jdp.Patch(containerToken, diff);
                                containerToken = newToken;
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        DiagManager.Instance.LogError(ex);
                    }
                }

                //then, load from this string
                {
                    string containerStr = containerToken.ToString();
                    Version pastVersion = OldVersion;
                    OldVersion = baseOldVersion;
                    SerializationContainer container = (SerializationContainer)JsonConvert.DeserializeObject(containerStr, typeof(SerializationContainer), Settings);
                    OldVersion = pastVersion;
                    return container.Object;
                }
            }
        }

        /// <summary>
        /// Deserializes versioned data from a stream.
        /// </summary>
        /// <param name="stream">The stream containing the serialized data.</param>
        /// <returns>The deserialized object from the container.</returns>
        public static object DeserializeData(Stream stream)
        {
            lock (lockObj)
            {
                using (StreamReader reader = new StreamReader(stream, Encoding.UTF8, true, -1, true))
                {
                    string containerStr = reader.ReadToEnd();
                    //Temporarily set global old version for converters in UpgradeConverters.cs to recognize the version.
                    Version pastVersion = OldVersion;
                    OldVersion = GetVersion(containerStr);
                    SerializationContainer container = (SerializationContainer)JsonConvert.DeserializeObject(containerStr, typeof(SerializationContainer), Settings);
                    OldVersion = pastVersion;
                    return container.Object;
                }
            }
        }

        /// <summary>
        /// Serializes data as a diff patch against a base file.
        /// Only saves the differences, reducing file size for mods.
        /// </summary>
        /// <param name="path">The path to save the diff file.</param>
        /// <param name="basepath">The path to the base file to diff against.</param>
        /// <param name="entry">The object to serialize as a diff.</param>
        public static void SerializeDataAsDiff(string path, string basepath, object entry)
        {
            //save the current object json to a string
            JToken currentToken;
            {
                SerializationContainer container = new SerializationContainer();
                container.Object = entry;
                container.Version = Versioning.GetVersion();
                string containerStr = SerializeObjectInternal(container, Settings, true);
                currentToken = JToken.Parse(containerStr);
            }

            //load the original object json
            JToken baseToken;
            using (Stream stream = new FileStream(basepath, FileMode.Open, FileAccess.Read, FileShare.Read))
            {
                using (StreamReader reader = new StreamReader(stream, Encoding.UTF8, true, -1, true))
                {
                    string containerStr = reader.ReadToEnd();
                    baseToken = JToken.Parse(containerStr);
                    SetVersion(baseToken, new Version());
                }
            }

            //diff the two
            JToken diffToken = jdp.Diff(baseToken, currentToken);

            bool nullDiff = false;
            //null diff?  no diff!
            if (diffToken == null)
                nullDiff = true;
            else if (diffToken["Object"] == null)
                nullDiff = true;

            if (nullDiff)
            {
                //no need to save anything
                //delete if exists
                if (File.Exists(path))
                    File.Delete(path);
                return;
            }

            //save the resulting diff string to the path
            using (Stream stream = new FileStream(path, FileMode.Create, FileAccess.Write, FileShare.None))
            {
                using (StreamWriter writer = new StreamWriter(stream, Encoding.UTF8, -1, true))
                {
                    writer.Write(diffToken.ToString());
                }
            }
        }

        /// <summary>
        /// Serializes versioned data to a stream.
        /// </summary>
        /// <param name="stream">The stream to write to.</param>
        /// <param name="entry">The object to serialize.</param>
        /// <param name="min">If true, uses minified JSON without formatting.</param>
        public static void SerializeData(Stream stream, object entry, bool min = false)
        {
            using (StreamWriter writer = new StreamWriter(stream, Encoding.UTF8, -1, true))
            {
                SerializationContainer container = new SerializationContainer();
                container.Object = entry;
                container.Version = Versioning.GetVersion();
                string val = SerializeObjectInternal(container, Settings, min);
                writer.Write(val);
            }
        }

        private static string SerializeObjectInternal(object value, JsonSerializerSettings settings, bool min)
        {
            JsonSerializer jsonSerializer = JsonSerializer.CreateDefault(settings);
            StringBuilder sb = new StringBuilder(256);
            StringWriter sw = new StringWriter(sb, CultureInfo.InvariantCulture);
            using (JsonTextWriter jsonWriter = new JsonTextWriter(sw))
            {
                if (min)
                {
                    jsonWriter.Formatting = Formatting.None;
                }
                else
                {
                    jsonWriter.Formatting = Formatting.Indented;
                    jsonWriter.Indentation = 0;
                    jsonWriter.IndentChar = '\t';
                }

                jsonSerializer.Serialize(jsonWriter, value, null);
            }

            return sw.ToString();
        }

    }
}