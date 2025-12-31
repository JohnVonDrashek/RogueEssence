using System;
using System.IO;
using RogueEssence.Data;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using System.Collections.Generic;
using System.Xml.Serialization;
using RogueEssence.Content;
using Newtonsoft.Json;
using NLua;
using RogueElements;
using Newtonsoft.Json.Linq;
using RogueEssence.Dungeon;

namespace RogueEssence.Dev
{
    /// <summary>
    /// JSON converter for script variables that handles backward compatibility with older save formats.
    /// Converts between LuaTableContainer and various JSON representations.
    /// </summary>
    //TODO: Created v0.5.2, delete on v1.1
    public class ScriptVarsConverter : JsonConverter
    {
        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            throw new NotImplementedException("We shouldn't be here.");
            // will this work?
            //serializer.Serialize(writer, value);

            // doesnt work due to self reference
            //serializer.Serialize(writer, serializer);
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            if (reader.TokenType == JsonToken.StartObject)
            {
                // doesn't work due to object type disagreement of some sort...
                //return serializer.Deserialize(reader, objectType);


                // will this work?
                //Script.LuaTableContainer container = new Script.LuaTableContainer();
                //reader.Read();
                ////we're now in the first property, table
                //reader.Read();
                ////now in the property data?
                //JObject jObject = JObject.Load(reader);
                //serializer.Populate(jObject.CreateReader(), container.Table);


                JObject jObject = JObject.Load(reader);
                Script.LuaTableContainer container = new Script.LuaTableContainer();
                serializer.Populate(jObject.CreateReader(), container);
                return container;
            }
            else
            {
                string s = (string)reader.Value;
                if (s == null)
                    return null;

                try
                {
                    return JsonConvert.DeserializeObject(s, objectType, Serializer.Settings);
                }
                catch (Exception ex)
                {
                    LuaTable tbl = Script.LuaEngine.Instance.DeserializedLuaTable(s);
                    return Script.LuaEngine.Instance.SaveLuaTable(tbl);
                }
            }
        }

        public override bool CanWrite
        {
            get
            {
                return false;
            }
        }

        public override bool CanConvert(Type objectType)
        {
            return objectType == typeof(Script.LuaTableContainer);
        }
    }

    /// <summary>
    /// JSON converter for Lua table container dictionaries that handles backward compatibility.
    /// Converts between dictionary and array representations of Lua table data.
    /// </summary>
    //TODO: Created v0.5.3, delete on v1.1
    public class LuaTableContainerDictConverter : JsonConverter
    {
        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            throw new NotImplementedException("We shouldn't be here.");
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            if (reader.TokenType == JsonToken.StartArray)
            {
                JArray jArray = JArray.Load(reader);
                List<object[]> container = new List<object[]>();
                serializer.Populate(jArray.CreateReader(), container);
                return container;
            }
            else
            {
                JObject jObject = JObject.Load(reader);
                Dictionary<object, object> dict = new Dictionary<object, object>();
                serializer.Populate(jObject.CreateReader(), dict);
                List<object[]> container = new List<object[]>();
                foreach (object key in dict.Keys)
                    container.Add(new object[] { key, dict[key] });
                return container;
            }
        }

        public override bool CanWrite => false;

        public override bool CanConvert(Type objectType)
        {
            return objectType == typeof(List<(object, object)>);
        }
    }


    /// <summary>
    /// JSON converter for IRandom interface types that handles serialization compatibility.
    /// Serializes random number generator state as a JSON string value.
    /// </summary>
    //TODO: Created v0.5.2, delete on v1.1
    public class IRandomConverter : JsonConverter
    {
        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            string val = JsonConvert.SerializeObject(value, Serializer.Settings);
            writer.WriteValue(val);
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            string s = (string)reader.Value;
            if (s == null)
                return null;

            try
            {
                return JsonConvert.DeserializeObject(s, objectType, Serializer.Settings);
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public override bool CanConvert(Type objectType)
        {
            return objectType == typeof(IRandom);
        }
    }

    /// <summary>
    /// JSON converter for MapBG objects that handles version migration.
    /// Automatically sets RepeatX and RepeatY to true for files from version 0.5.8 or earlier.
    /// </summary>
    //TODO: Created v0.5.10, delete on v1.1
    public class MapBGConverter : JsonConverter<MapBG>
    {
        public override void WriteJson(JsonWriter writer, MapBG value, JsonSerializer serializer)
        {
            throw new NotImplementedException("We shouldn't be here.");
        }

        public override MapBG ReadJson(JsonReader reader, Type objectType, MapBG existingValue, bool hasExistingValue, JsonSerializer serializer)
        {
            JObject jObject = JObject.Load(reader);
            MapBG container = new MapBG();
            serializer.Populate(jObject.CreateReader(), container);


            if (Serializer.OldVersion <= new Version(0, 5, 8, 0))
            {
                container.RepeatX = true;
                container.RepeatY = true;
            }

            return container;
        }


        public override bool CanWrite
        {
            get
            {
                return false;
            }
        }
    }

    /// <summary>
    /// JSON converter for dungeon unlock state dictionaries that handles migration from integer to string asset IDs.
    /// Converts older array-based unlock state data to the newer dictionary format with string keys.
    /// </summary>
    //TODO: Created v0.6.0, delete on v1.1
    public class DungeonUnlockConverter : JsonConverter
    {
        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            throw new NotImplementedException("We shouldn't be here.");
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            Dictionary<string, GameProgress.UnlockState> dict = new Dictionary<string, GameProgress.UnlockState>();
            if (Serializer.OldVersion < DevHelper.StringAssetVersion)
            {
                JArray jArray = JArray.Load(reader);
                List<GameProgress.UnlockState> container = new List<GameProgress.UnlockState>();
                serializer.Populate(jArray.CreateReader(), container);

                for (int ii = 0; ii < container.Count; ii++)
                {
                    if (container[ii] > GameProgress.UnlockState.None)
                    {
                        if (DataManager.Instance.Conversions[DataManager.DataType.Zone].ContainsKey(ii.ToString()))
                        {
                            string asset_name = DataManager.Instance.MapAssetName(DataManager.DataType.Zone, ii);
                            int val;
                            if (int.TryParse(asset_name, out val))
                                continue;
                            dict[asset_name] = container[ii];
                        }
                    }
                }
            }
            else
            {
                JObject jObject = JObject.Load(reader);
                serializer.Populate(jObject.CreateReader(), dict);
            }
            return dict;
        }

        public override bool CanWrite
        {
            get
            {
                return false;
            }
        }

        public override bool CanConvert(Type objectType)
        {
            return objectType == typeof(Dictionary<string, GameProgress.UnlockState>);
        }
    }

    /// <summary>
    /// JSON converter for dungeon/zone asset references that handles migration from integer to string IDs.
    /// Converts numeric zone IDs from older versions to string-based asset names.
    /// </summary>
    public class DungeonConverter : JsonConverter
    {
        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            throw new NotImplementedException("We shouldn't be here.");
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            if (Serializer.OldVersion < DevHelper.StringAssetVersion)
            {
                int ii = Int32.Parse(reader.Value.ToString());
                string asset_name = DataManager.Instance.MapAssetName(DataManager.DataType.Zone, ii);
                return asset_name;
            }
            else
            {
                string s = (string)reader.Value;
                return s;
            }
        }

        public override bool CanWrite
        {
            get
            {
                return false;
            }
        }

        public override bool CanConvert(Type objectType)
        {
            return objectType == typeof(string);
        }
    }


    /// <summary>
    /// JSON converter for autotile asset references that handles migration from integer to string IDs.
    /// Converts numeric autotile IDs from older versions to string-based asset names.
    /// </summary>
    public class AutotileConverter : JsonConverter
    {
        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            throw new NotImplementedException("We shouldn't be here.");
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            if (Serializer.OldVersion < DevHelper.StringAssetVersion)
            {
                int ii = Int32.Parse(reader.Value.ToString());
                string asset_name = DataManager.Instance.MapAssetName(DataManager.DataType.AutoTile, ii);
                return asset_name;
            }
            else
            {
                string s = (string)reader.Value;
                return s;
            }
        }

        public override bool CanWrite
        {
            get
            {
                return false;
            }
        }

        public override bool CanConvert(Type objectType)
        {
            return objectType == typeof(string);
        }
    }


    /// <summary>
    /// JSON converter for autotile set collections that handles migration from integer to string IDs.
    /// Converts HashSet of integer autotile IDs to HashSet of string asset names.
    /// </summary>
    public class AutotileSetConverter : JsonConverter
    {
        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            throw new NotImplementedException("We shouldn't be here.");
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            HashSet<string> dict = new HashSet<string>();
            if (Serializer.OldVersion < DevHelper.StringAssetVersion)
            {
                if (reader.TokenType == JsonToken.StartArray)
                {
                    JArray jArray = JArray.Load(reader);
                    HashSet<int> container = new HashSet<int>();
                    serializer.Populate(jArray.CreateReader(), container);

                    foreach (int ii in container)
                    {
                        string asset_name = DataManager.Instance.MapAssetName(DataManager.DataType.AutoTile, ii);
                        dict.Add(asset_name);
                    }
                }
            }
            else
            {
                JArray jArray = JArray.Load(reader);
                serializer.Populate(jArray.CreateReader(), dict);
            }
            return dict;
        }

        public override bool CanWrite
        {
            get
            {
                return false;
            }
        }

        public override bool CanConvert(Type objectType)
        {
            return objectType == typeof(HashSet<string>);
        }
    }

    /// <summary>
    /// JSON converter for terrain-to-autotile mapping dictionaries that handles migration from integer to string IDs.
    /// Converts dictionaries mapping terrain IDs to autotile IDs from integer keys to string asset names.
    /// </summary>
    public class TerrainAutotileDictConverter : JsonConverter
    {
        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            throw new NotImplementedException("We shouldn't be here.");
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            Dictionary<string, string> dict = new Dictionary<string, string>();
            if (Serializer.OldVersion < DevHelper.StringAssetVersion)
            {
                JObject jObject = JObject.Load(reader);
                Dictionary<int, int> container = new Dictionary<int, int>();
                serializer.Populate(jObject.CreateReader(), container);

                foreach (int ii in container.Keys)
                {
                    string terrain_name = DataManager.Instance.MapAssetName(DataManager.DataType.Terrain, ii);
                    string asset_name = DataManager.Instance.MapAssetName(DataManager.DataType.AutoTile, container[ii]);
                    dict[terrain_name] = asset_name;
                }
            }
            else
            {
                JObject jObject = JObject.Load(reader);
                serializer.Populate(jObject.CreateReader(), dict);
            }
            return dict;
        }

        public override bool CanWrite
        {
            get
            {
                return false;
            }
        }

        public override bool CanConvert(Type objectType)
        {
            return objectType == typeof(Dictionary<string, string>);
        }
    }


    /// <summary>
    /// JSON converter for terrain asset references that handles migration from integer to string IDs.
    /// Converts numeric terrain IDs from older versions to string-based asset names.
    /// </summary>
    public class TerrainConverter : JsonConverter
    {
        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            throw new NotImplementedException("We shouldn't be here.");
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            if (Serializer.OldVersion < DevHelper.StringAssetVersion)
            {
                int ii = Int32.Parse(reader.Value.ToString());
                string asset_name = DataManager.Instance.MapAssetName(DataManager.DataType.Terrain, ii);
                return asset_name;
            }
            else
            {
                string s = (string)reader.Value;
                return s;
            }
        }

        public override bool CanWrite
        {
            get
            {
                return false;
            }
        }

        public override bool CanConvert(Type objectType)
        {
            return objectType == typeof(string);
        }
    }

    /// <summary>
    /// JSON converter for terrain set collections that handles migration from integer to string IDs.
    /// Converts HashSet of integer terrain IDs to HashSet of string asset names.
    /// </summary>
    public class TerrainSetConverter : JsonConverter
    {
        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            throw new NotImplementedException("We shouldn't be here.");
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            HashSet<string> dict = new HashSet<string>();
            if (Serializer.OldVersion < DevHelper.StringAssetVersion)
            {
                JArray jArray = JArray.Load(reader);
                HashSet<int> container = new HashSet<int>();
                serializer.Populate(jArray.CreateReader(), container);

                foreach (int ii in container)
                {
                    string asset_name = DataManager.Instance.MapAssetName(DataManager.DataType.Terrain, ii);
                    dict.Add(asset_name);
                }
            }
            else
            {
                JArray jArray = JArray.Load(reader);
                serializer.Populate(jArray.CreateReader(), dict);
            }
            return dict;
        }

        public override bool CanWrite
        {
            get
            {
                return false;
            }
        }

        public override bool CanConvert(Type objectType)
        {
            return objectType == typeof(HashSet<string>);
        }
    }




    /// <summary>
    /// JSON converter for terrain-to-autotile object mapping dictionaries that handles migration from integer to string IDs.
    /// Converts dictionaries mapping integer terrain IDs to AutoTile objects to string-keyed dictionaries.
    /// </summary>
    public class TerrainDictAutotileConverter : JsonConverter
    {
        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            throw new NotImplementedException("We shouldn't be here.");
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            Dictionary<string, AutoTile> dict = new Dictionary<string, AutoTile>();
            if (Serializer.OldVersion < DevHelper.StringAssetVersion)
            {
                JObject jObject = JObject.Load(reader);
                Dictionary<int, AutoTile> container = new Dictionary<int, AutoTile>();
                serializer.Populate(jObject.CreateReader(), container);

                foreach (int ii in container.Keys)
                {
                    string terrain_name = DataManager.Instance.MapAssetName(DataManager.DataType.Terrain, ii);
                    dict[terrain_name] = container[ii];
                }
            }
            else
            {
                JObject jObject = JObject.Load(reader);
                serializer.Populate(jObject.CreateReader(), dict);
            }
            return dict;
        }

        public override bool CanWrite
        {
            get
            {
                return false;
            }
        }

        public override bool CanConvert(Type objectType)
        {
            return objectType == typeof(Dictionary<string, AutoTile>);
        }
    }

    /// <summary>
    /// JSON converter for growth group asset references that handles migration from integer to string IDs.
    /// Converts numeric growth group IDs from older versions to string-based asset names.
    /// </summary>
    public class GrowthGroupConverter : JsonConverter
    {
        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            throw new NotImplementedException("We shouldn't be here.");
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            if (Serializer.OldVersion < DevHelper.StringAssetVersion)
            {
                int ii = Int32.Parse(reader.Value.ToString());
                string asset_name = DataManager.Instance.MapAssetName(DataManager.DataType.GrowthGroup, ii);
                return asset_name;
            }
            else
            {
                string s = (string)reader.Value;
                return s;
            }
        }

        public override bool CanWrite
        {
            get
            {
                return false;
            }
        }

        public override bool CanConvert(Type objectType)
        {
            return objectType == typeof(string);
        }
    }


    /// <summary>
    /// JSON converter for skill group asset references that handles migration from integer to string IDs.
    /// Converts numeric skill group IDs from older versions to string-based asset names.
    /// </summary>
    public class SkillGroupConverter : JsonConverter
    {
        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            throw new NotImplementedException("We shouldn't be here.");
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            if (Serializer.OldVersion < DevHelper.StringAssetVersion)
            {
                int ii = Int32.Parse(reader.Value.ToString());
                string asset_name = DataManager.Instance.MapAssetName(DataManager.DataType.SkillGroup, ii);
                return asset_name;
            }
            else
            {
                string s = (string)reader.Value;
                return s;
            }
        }

        public override bool CanWrite
        {
            get
            {
                return false;
            }
        }

        public override bool CanConvert(Type objectType)
        {
            return objectType == typeof(string);
        }
    }

    /// <summary>
    /// JSON converter for rank asset references that handles migration from integer to string IDs.
    /// Converts numeric rank IDs from older versions to string-based asset names.
    /// </summary>
    public class RankConverter : JsonConverter
    {
        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            throw new NotImplementedException("We shouldn't be here.");
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            if (Serializer.OldVersion < DevHelper.StringAssetVersion)
            {
                int ii = Int32.Parse(reader.Value.ToString());
                string asset_name = DataManager.Instance.MapAssetName(DataManager.DataType.Rank, ii);
                return asset_name;
            }
            else
            {
                string s = (string)reader.Value;
                return s;
            }
        }

        public override bool CanWrite
        {
            get
            {
                return false;
            }
        }

        public override bool CanConvert(Type objectType)
        {
            return objectType == typeof(string);
        }
    }


    /// <summary>
    /// JSON converter for AI asset references that handles migration from integer to string IDs.
    /// Converts numeric AI behavior IDs from older versions to string-based asset names.
    /// </summary>
    public class AIConverter : JsonConverter
    {
        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            throw new NotImplementedException("We shouldn't be here.");
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            if (Serializer.OldVersion < DevHelper.StringAssetVersion)
            {
                int ii = Int32.Parse(reader.Value.ToString());
                string asset_name = DataManager.Instance.MapAssetName(DataManager.DataType.AI, ii);
                return asset_name;
            }
            else
            {
                string s = (string)reader.Value;
                return s;
            }
        }

        public override bool CanWrite
        {
            get
            {
                return false;
            }
        }

        public override bool CanConvert(Type objectType)
        {
            return objectType == typeof(string);
        }
    }

    /// <summary>
    /// JSON converter for tile asset references that handles migration from integer to string IDs.
    /// Converts numeric tile IDs from older versions to string-based asset names.
    /// </summary>
    public class TileConverter : JsonConverter
    {
        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            throw new NotImplementedException("We shouldn't be here.");
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            if (Serializer.OldVersion < DevHelper.StringAssetVersion)
            {
                int ii = Int32.Parse(reader.Value.ToString());
                string asset_name = DataManager.Instance.MapAssetName(DataManager.DataType.Tile, ii);
                return asset_name;
            }
            else
            {
                string s = (string)reader.Value;
                return s;
            }
        }

        public override bool CanWrite
        {
            get
            {
                return false;
            }
        }

        public override bool CanConvert(Type objectType)
        {
            return objectType == typeof(string);
        }
    }


    /// <summary>
    /// JSON converter for tile list collections that handles migration from integer to string IDs.
    /// Converts lists of integer tile IDs to lists of string asset names.
    /// </summary>
    public class TileListConverter : JsonConverter
    {
        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            throw new NotImplementedException("We shouldn't be here.");
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            List<string> dict = new List<string>();
            if (Serializer.OldVersion < DevHelper.StringAssetVersion)
            {
                JArray jArray = JArray.Load(reader);
                List<int> container = new List<int>();
                serializer.Populate(jArray.CreateReader(), container);

                foreach (int ii in container)
                {
                    string asset_name = DataManager.Instance.MapAssetName(DataManager.DataType.Tile, ii);
                    dict.Add(asset_name);
                }
            }
            else
            {
                JArray jArray = JArray.Load(reader);
                serializer.Populate(jArray.CreateReader(), dict);
            }
            return dict;
        }

        public override bool CanWrite
        {
            get
            {
                return false;
            }
        }

        public override bool CanConvert(Type objectType)
        {
            return objectType == typeof(List<string>);
        }
    }



    /// <summary>
    /// JSON converter for element type asset references that handles migration from integer to string IDs.
    /// Converts numeric element type IDs from older versions to string-based asset names.
    /// </summary>
    public class ElementConverter : JsonConverter
    {
        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            throw new NotImplementedException("We shouldn't be here.");
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            if (Serializer.OldVersion < DevHelper.StringAssetVersion)
            {
                int ii = Int32.Parse(reader.Value.ToString());
                string asset_name = DataManager.Instance.MapAssetName(DataManager.DataType.Element, ii);
                return asset_name;
            }
            else
            {
                string s = (string)reader.Value;
                return s;
            }
        }

        public override bool CanWrite
        {
            get
            {
                return false;
            }
        }

        public override bool CanConvert(Type objectType)
        {
            return objectType == typeof(string);
        }
    }

    /// <summary>
    /// JSON converter for element set collections that handles migration from integer to string IDs.
    /// Converts HashSet of integer element IDs to HashSet of string asset names.
    /// </summary>
    public class ElementSetConverter : JsonConverter
    {
        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            throw new NotImplementedException("We shouldn't be here.");
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            HashSet<string> dict = new HashSet<string>();
            if (Serializer.OldVersion < DevHelper.StringAssetVersion)
            {
                JArray jArray = JArray.Load(reader);
                HashSet<int> container = new HashSet<int>();
                serializer.Populate(jArray.CreateReader(), container);

                foreach (int ii in container)
                {
                    string asset_name = DataManager.Instance.MapAssetName(DataManager.DataType.Element, ii);
                    dict.Add(asset_name);
                }
            }
            else
            {
                JArray jArray = JArray.Load(reader);
                serializer.Populate(jArray.CreateReader(), dict);
            }
            return dict;
        }

        public override bool CanWrite
        {
            get
            {
                return false;
            }
        }

        public override bool CanConvert(Type objectType)
        {
            return objectType == typeof(HashSet<string>);
        }
    }

    /// <summary>
    /// JSON converter for element list collections that handles migration from integer to string IDs.
    /// Converts lists of integer element IDs to lists of string asset names.
    /// </summary>
    public class ElementListConverter : JsonConverter
    {
        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            throw new NotImplementedException("We shouldn't be here.");
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            List<string> dict = new List<string>();
            if (Serializer.OldVersion < DevHelper.StringAssetVersion)
            {
                JArray jArray = JArray.Load(reader);
                HashSet<int> container = new HashSet<int>();
                serializer.Populate(jArray.CreateReader(), container);

                foreach (int ii in container)
                {
                    string asset_name = DataManager.Instance.MapAssetName(DataManager.DataType.Element, ii);
                    dict.Add(asset_name);
                }
            }
            else
            {
                JArray jArray = JArray.Load(reader);
                serializer.Populate(jArray.CreateReader(), dict);
            }
            return dict;
        }

        public override bool CanWrite
        {
            get
            {
                return false;
            }
        }

        public override bool CanConvert(Type objectType)
        {
            return objectType == typeof(List<string>);
        }
    }

    /// <summary>
    /// JSON converter for element array collections that handles migration from integer to string IDs.
    /// Converts arrays of integer element IDs to arrays of string asset names.
    /// </summary>
    public class ElementArrayConverter : JsonConverter
    {
        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            throw new NotImplementedException("We shouldn't be here.");
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            List<string> dict = new List<string>();
            if (Serializer.OldVersion < DevHelper.StringAssetVersion)
            {
                JArray jArray = JArray.Load(reader);
                List<int> container = new List<int>();
                serializer.Populate(jArray.CreateReader(), container);

                foreach (int ii in container)
                {
                    string asset_name = DataManager.Instance.MapAssetName(DataManager.DataType.Element, ii);
                    dict.Add(asset_name);
                }
            }
            else
            {
                JArray jArray = JArray.Load(reader);
                serializer.Populate(jArray.CreateReader(), dict);
            }
            return dict.ToArray();
        }

        public override bool CanWrite
        {
            get
            {
                return false;
            }
        }

        public override bool CanConvert(Type objectType)
        {
            return objectType == typeof(string[]);
        }
    }

    /// <summary>
    /// JSON converter for item-to-element mapping dictionaries that handles migration from integer to string IDs.
    /// Converts dictionaries mapping item IDs to element IDs from integer keys to string asset names.
    /// </summary>
    public class ItemElementDictConverter : JsonConverter
    {
        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            throw new NotImplementedException("We shouldn't be here.");
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            Dictionary<string, string> dict = new Dictionary<string, string>();
            if (Serializer.OldVersion < DevHelper.StringAssetVersion)
            {
                JObject jObject = JObject.Load(reader);
                Dictionary<int, int> container = new Dictionary<int, int>();
                serializer.Populate(jObject.CreateReader(), container);

                foreach (int ii in container.Keys)
                {
                    string item_name = DataManager.Instance.MapAssetName(DataManager.DataType.Item, ii);
                    string asset_name = DataManager.Instance.MapAssetName(DataManager.DataType.Element, container[ii]);
                    dict[item_name] = asset_name;
                }
            }
            else
            {
                JObject jObject = JObject.Load(reader);
                serializer.Populate(jObject.CreateReader(), dict);
            }
            return dict;
        }

        public override bool CanWrite
        {
            get
            {
                return false;
            }
        }

        public override bool CanConvert(Type objectType)
        {
            return objectType == typeof(Dictionary<string, string>);
        }
    }

    /// <summary>
    /// JSON converter for map status-to-element mapping dictionaries that handles migration from integer to string IDs.
    /// Converts dictionaries mapping map status IDs to element IDs from integer keys to string asset names.
    /// </summary>
    public class MapStatusElementDictConverter : JsonConverter
    {
        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            throw new NotImplementedException("We shouldn't be here.");
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            Dictionary<string, string> dict = new Dictionary<string, string>();
            if (Serializer.OldVersion < DevHelper.StringAssetVersion)
            {
                JObject jObject = JObject.Load(reader);
                Dictionary<int, int> container = new Dictionary<int, int>();
                serializer.Populate(jObject.CreateReader(), container);

                foreach (int ii in container.Keys)
                {
                    string item_name = DataManager.Instance.MapAssetName(DataManager.DataType.MapStatus, ii);
                    string asset_name = DataManager.Instance.MapAssetName(DataManager.DataType.Element, container[ii]);
                    dict[item_name] = asset_name;
                }
            }
            else
            {
                JObject jObject = JObject.Load(reader);
                serializer.Populate(jObject.CreateReader(), dict);
            }
            return dict;
        }

        public override bool CanWrite
        {
            get
            {
                return false;
            }
        }

        public override bool CanConvert(Type objectType)
        {
            return objectType == typeof(Dictionary<int, string>);
        }
    }

    /// <summary>
    /// JSON converter for element-to-map status mapping dictionaries that handles migration from integer to string IDs.
    /// Converts dictionaries mapping element IDs to map status IDs from integer keys to string asset names.
    /// </summary>
    public class ElementMapStatusDictConverter : JsonConverter
    {
        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            throw new NotImplementedException("We shouldn't be here.");
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            Dictionary<string, string> dict = new Dictionary<string, string>();
            if (Serializer.OldVersion < DevHelper.StringAssetVersion)
            {
                JObject jObject = JObject.Load(reader);
                Dictionary<int, int> container = new Dictionary<int, int>();
                serializer.Populate(jObject.CreateReader(), container);

                foreach (int ii in container.Keys)
                {
                    string item_name = DataManager.Instance.MapAssetName(DataManager.DataType.Element, ii);
                    string asset_name = DataManager.Instance.MapAssetName(DataManager.DataType.MapStatus, container[ii]);
                    dict[item_name] = asset_name;
                }
            }
            else
            {
                JObject jObject = JObject.Load(reader);
                serializer.Populate(jObject.CreateReader(), dict);
            }
            return dict;
        }

        public override bool CanWrite
        {
            get
            {
                return false;
            }
        }

        public override bool CanConvert(Type objectType)
        {
            return objectType == typeof(Dictionary<string, int>);
        }
    }


    /// <summary>
    /// JSON converter for element-to-item mapping dictionaries that handles migration from integer to string IDs.
    /// Converts dictionaries mapping element IDs to item IDs from integer keys to string asset names.
    /// </summary>
    public class ElementItemDictConverter : JsonConverter
    {
        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            throw new NotImplementedException("We shouldn't be here.");
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            Dictionary<string, string> dict = new Dictionary<string, string>();
            if (Serializer.OldVersion < DevHelper.StringAssetVersion)
            {
                JObject jObject = JObject.Load(reader);
                Dictionary<int, int> container = new Dictionary<int, int>();
                serializer.Populate(jObject.CreateReader(), container);

                foreach (int ii in container.Keys)
                {
                    string item_name = DataManager.Instance.MapAssetName(DataManager.DataType.Element, ii);
                    string asset_name = DataManager.Instance.MapAssetName(DataManager.DataType.Item, container[ii]);
                    dict[item_name] = asset_name;
                }
            }
            else
            {
                JObject jObject = JObject.Load(reader);
                serializer.Populate(jObject.CreateReader(), dict);
            }
            return dict;
        }

        public override bool CanWrite
        {
            get
            {
                return false;
            }
        }

        public override bool CanConvert(Type objectType)
        {
            return objectType == typeof(Dictionary<string, int>);
        }
    }

    /// <summary>
    /// JSON converter for element-to-skill mapping dictionaries that handles migration from integer to string IDs.
    /// Converts dictionaries mapping element IDs to skill IDs from integer keys to string asset names.
    /// </summary>
    public class ElementSkillDictConverter : JsonConverter
    {
        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            throw new NotImplementedException("We shouldn't be here.");
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            Dictionary<string, string> dict = new Dictionary<string, string>();
            if (Serializer.OldVersion < DevHelper.StringAssetVersion)
            {
                JObject jObject = JObject.Load(reader);
                Dictionary<int, int> container = new Dictionary<int, int>();
                serializer.Populate(jObject.CreateReader(), container);

                foreach (int ii in container.Keys)
                {
                    string item_name = DataManager.Instance.MapAssetName(DataManager.DataType.Element, ii);
                    string asset_name = DataManager.Instance.MapAssetName(DataManager.DataType.Skill, container[ii]);
                    dict[item_name] = asset_name;
                }
            }
            else
            {
                JObject jObject = JObject.Load(reader);
                serializer.Populate(jObject.CreateReader(), dict);
            }
            return dict;
        }

        public override bool CanWrite
        {
            get
            {
                return false;
            }
        }

        public override bool CanConvert(Type objectType)
        {
            return objectType == typeof(Dictionary<string, int>);
        }
    }


    /// <summary>
    /// JSON converter for element-to-battle event mapping dictionaries that handles migration from integer to string IDs.
    /// Converts dictionaries mapping element IDs to BattleEvent objects from integer keys to string asset names.
    /// </summary>
    public class ElementBattleEventDictConverter : JsonConverter
    {
        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            throw new NotImplementedException("We shouldn't be here.");
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            Dictionary<string, BattleEvent> dict = new Dictionary<string, BattleEvent>();
            if (Serializer.OldVersion < DevHelper.StringAssetVersion)
            {
                JObject jObject = JObject.Load(reader);
                Dictionary<int, BattleEvent> container = new Dictionary<int, BattleEvent>();
                serializer.Populate(jObject.CreateReader(), container);

                foreach (int ii in container.Keys)
                {
                    string item_name = DataManager.Instance.MapAssetName(DataManager.DataType.Element, ii);
                    dict[item_name] = container[ii];
                }
            }
            else
            {
                JObject jObject = JObject.Load(reader);
                serializer.Populate(jObject.CreateReader(), dict);
            }
            return dict;
        }

        public override bool CanWrite
        {
            get
            {
                return false;
            }
        }

        public override bool CanConvert(Type objectType)
        {
            return objectType == typeof(Dictionary<string, BattleEvent>);
        }
    }

    /// <summary>
    /// JSON converter for map status asset references that handles migration from integer to string IDs.
    /// Converts numeric map status IDs from older versions to string-based asset names.
    /// </summary>
    public class MapStatusConverter : JsonConverter
    {
        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            throw new NotImplementedException("We shouldn't be here.");
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            if (Serializer.OldVersion < DevHelper.StringAssetVersion)
            {
                int ii = Int32.Parse(reader.Value.ToString());
                string asset_name = DataManager.Instance.MapAssetName(DataManager.DataType.MapStatus, ii);
                return asset_name;
            }
            else
            {
                string s = (string)reader.Value;
                return s;
            }
        }

        public override bool CanWrite
        {
            get
            {
                return false;
            }
        }

        public override bool CanConvert(Type objectType)
        {
            return objectType == typeof(string);
        }
    }


    /// <summary>
    /// JSON converter for map status array collections that handles migration from integer to string IDs.
    /// Converts arrays of integer map status IDs to arrays of string asset names.
    /// </summary>
    public class MapStatusArrayConverter : JsonConverter
    {
        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            throw new NotImplementedException("We shouldn't be here.");
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            List<string> dict = new List<string>();
            if (Serializer.OldVersion < DevHelper.StringAssetVersion)
            {
                JArray jArray = JArray.Load(reader);
                List<int> container = new List<int>();
                serializer.Populate(jArray.CreateReader(), container);

                foreach (int ii in container)
                {
                    string asset_name = DataManager.Instance.MapAssetName(DataManager.DataType.MapStatus, ii);
                    dict.Add(asset_name);
                }
            }
            else
            {
                JArray jArray = JArray.Load(reader);
                serializer.Populate(jArray.CreateReader(), dict);
            }
            return dict.ToArray();
        }

        public override bool CanWrite
        {
            get
            {
                return false;
            }
        }

        public override bool CanConvert(Type objectType)
        {
            return objectType == typeof(string[]);
        }
    }

    /// <summary>
    /// JSON converter for map status-to-MapStatus object mapping dictionaries that handles migration from integer to string IDs.
    /// Converts dictionaries mapping integer map status IDs to MapStatus objects to string-keyed dictionaries.
    /// </summary>
    public class MapStatusDictConverter : JsonConverter
    {
        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            throw new NotImplementedException("We shouldn't be here.");
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            Dictionary<string, MapStatus> dict = new Dictionary<string, MapStatus>();
            if (Serializer.OldVersion < DevHelper.StringAssetVersion)
            {
                JObject jObject = JObject.Load(reader);
                Dictionary<int, MapStatus> container = new Dictionary<int, MapStatus>();
                serializer.Populate(jObject.CreateReader(), container);

                foreach (int ii in container.Keys)
                {
                    string item_name = DataManager.Instance.MapAssetName(DataManager.DataType.MapStatus, ii);
                    dict[item_name] = container[ii];
                }
            }
            else
            {
                JObject jObject = JObject.Load(reader);
                serializer.Populate(jObject.CreateReader(), dict);
            }
            return dict;
        }

        public override bool CanWrite
        {
            get
            {
                return false;
            }
        }

        public override bool CanConvert(Type objectType)
        {
            return objectType == typeof(Dictionary<string, MapStatus>);
        }
    }


    /// <summary>
    /// JSON converter for map status-to-integer mapping dictionaries that handles migration from integer to string IDs.
    /// Converts dictionaries mapping integer map status IDs to integer values to string-keyed dictionaries.
    /// </summary>
    public class MapStatusIntDictConverter : JsonConverter
    {
        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            throw new NotImplementedException("We shouldn't be here.");
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            Dictionary<string, int> dict = new Dictionary<string, int>();
            if (Serializer.OldVersion < DevHelper.StringAssetVersion)
            {
                JObject jObject = JObject.Load(reader);
                Dictionary<int, int> container = new Dictionary<int, int>();
                serializer.Populate(jObject.CreateReader(), container);

                foreach (int ii in container.Keys)
                {
                    string item_name = DataManager.Instance.MapAssetName(DataManager.DataType.MapStatus, ii);
                    dict[item_name] = container[ii];
                }
            }
            else
            {
                JObject jObject = JObject.Load(reader);
                serializer.Populate(jObject.CreateReader(), dict);
            }
            return dict;
        }

        public override bool CanWrite
        {
            get
            {
                return false;
            }
        }

        public override bool CanConvert(Type objectType)
        {
            return objectType == typeof(Dictionary<string, int>);
        }
    }

    /// <summary>
    /// JSON converter for map status list collections that handles migration from integer to string IDs.
    /// Converts lists of integer map status IDs to lists of string asset names.
    /// </summary>
    public class MapStatusListConverter : JsonConverter
    {
        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            throw new NotImplementedException("We shouldn't be here.");
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            List<string> dict = new List<string>();
            if (Serializer.OldVersion < DevHelper.StringAssetVersion)
            {
                JArray jArray = JArray.Load(reader);
                List<int> container = new List<int>();
                serializer.Populate(jArray.CreateReader(), container);

                foreach (int ii in container)
                {
                    string asset_name = DataManager.Instance.MapAssetName(DataManager.DataType.MapStatus, ii);
                    dict.Add(asset_name);
                }
            }
            else
            {
                JArray jArray = JArray.Load(reader);
                serializer.Populate(jArray.CreateReader(), dict);
            }
            return dict;
        }

        public override bool CanWrite
        {
            get
            {
                return false;
            }
        }

        public override bool CanConvert(Type objectType)
        {
            return objectType == typeof(List<string>);
        }
    }



    /// <summary>
    /// JSON converter for map status-to-battle data mapping dictionaries that handles migration from integer to string IDs.
    /// Converts dictionaries mapping integer map status IDs to BattleData objects to string-keyed dictionaries.
    /// </summary>
    public class MapStatusBattleDataDictConverter : JsonConverter
    {
        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            throw new NotImplementedException("We shouldn't be here.");
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            Dictionary<string, BattleData> dict = new Dictionary<string, BattleData>();
            if (Serializer.OldVersion < DevHelper.StringAssetVersion)
            {
                JObject jObject = JObject.Load(reader);
                Dictionary<int, BattleData> container = new Dictionary<int, BattleData>();
                serializer.Populate(jObject.CreateReader(), container);

                foreach (int ii in container.Keys)
                {
                    string item_name = DataManager.Instance.MapAssetName(DataManager.DataType.MapStatus, ii);
                    dict[item_name] = container[ii];
                }
            }
            else
            {
                JObject jObject = JObject.Load(reader);
                serializer.Populate(jObject.CreateReader(), dict);
            }
            return dict;
        }

        public override bool CanWrite
        {
            get
            {
                return false;
            }
        }

        public override bool CanConvert(Type objectType)
        {
            return objectType == typeof(Dictionary<string, BattleData>);
        }
    }

    /// <summary>
    /// JSON converter for map status-to-skill mapping dictionaries that handles migration from integer to string IDs.
    /// Converts dictionaries mapping map status IDs to skill IDs from integer keys to string asset names.
    /// </summary>
    public class MapStatusSkillDictConverter : JsonConverter
    {
        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            throw new NotImplementedException("We shouldn't be here.");
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            Dictionary<string, string> dict = new Dictionary<string, string>();
            if (Serializer.OldVersion < DevHelper.StringAssetVersion)
            {
                JObject jObject = JObject.Load(reader);
                Dictionary<int, int> container = new Dictionary<int, int>();
                serializer.Populate(jObject.CreateReader(), container);

                foreach (int ii in container.Keys)
                {
                    string item_name = DataManager.Instance.MapAssetName(DataManager.DataType.MapStatus, ii);
                    string asset_name = DataManager.Instance.MapAssetName(DataManager.DataType.Skill, container[ii]);
                    dict[item_name] = asset_name;
                }
            }
            else
            {
                JObject jObject = JObject.Load(reader);
                serializer.Populate(jObject.CreateReader(), dict);
            }
            return dict;
        }

        public override bool CanWrite
        {
            get
            {
                return false;
            }
        }

        public override bool CanConvert(Type objectType)
        {
            return objectType == typeof(Dictionary<string, int>);
        }
    }



    /// <summary>
    /// JSON converter for map status-to-battle event mapping dictionaries that handles migration from integer to string IDs.
    /// Converts dictionaries mapping integer map status IDs to BattleEvent objects to string-keyed dictionaries.
    /// </summary>
    public class MapStatusBattleEventDictConverter : JsonConverter
    {
        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            throw new NotImplementedException("We shouldn't be here.");
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            Dictionary<string, BattleEvent> dict = new Dictionary<string, BattleEvent>();
            if (Serializer.OldVersion < DevHelper.StringAssetVersion)
            {
                JObject jObject = JObject.Load(reader);
                Dictionary<int, BattleEvent> container = new Dictionary<int, BattleEvent>();
                serializer.Populate(jObject.CreateReader(), container);

                foreach (int ii in container.Keys)
                {
                    string item_name = DataManager.Instance.MapAssetName(DataManager.DataType.MapStatus, ii);
                    dict[item_name] = container[ii];
                }
            }
            else
            {
                JObject jObject = JObject.Load(reader);
                serializer.Populate(jObject.CreateReader(), dict);
            }
            return dict;
        }

        public override bool CanWrite
        {
            get
            {
                return false;
            }
        }

        public override bool CanConvert(Type objectType)
        {
            return objectType == typeof(Dictionary<string, BattleEvent>);
        }
    }


    /// <summary>
    /// JSON converter for map status-to-boolean mapping dictionaries that handles migration from integer to string IDs.
    /// Converts dictionaries mapping integer map status IDs to boolean values to string-keyed dictionaries.
    /// </summary>
    public class MapStatusBoolDictConverter : JsonConverter
    {
        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            throw new NotImplementedException("We shouldn't be here.");
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            Dictionary<string, bool> dict = new Dictionary<string, bool>();
            if (Serializer.OldVersion < DevHelper.StringAssetVersion)
            {
                JObject jObject = JObject.Load(reader);
                Dictionary<int, bool> container = new Dictionary<int, bool>();
                serializer.Populate(jObject.CreateReader(), container);

                foreach (int ii in container.Keys)
                {
                    string item_name = DataManager.Instance.MapAssetName(DataManager.DataType.MapStatus, ii);
                    dict[item_name] = container[ii];
                }
            }
            else
            {
                JObject jObject = JObject.Load(reader);
                serializer.Populate(jObject.CreateReader(), dict);
            }
            return dict;
        }

        public override bool CanWrite
        {
            get
            {
                return false;
            }
        }

        public override bool CanConvert(Type objectType)
        {
            return objectType == typeof(Dictionary<string, bool>);
        }
    }




    /// <summary>
    /// JSON converter for intrinsic ability asset references that handles migration from integer to string IDs.
    /// Converts numeric intrinsic IDs from older versions to string-based asset names.
    /// </summary>
    public class IntrinsicConverter : JsonConverter
    {
        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            throw new NotImplementedException("We shouldn't be here.");
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            if (Serializer.OldVersion < DevHelper.StringAssetVersion)
            {
                int ii = Int32.Parse(reader.Value.ToString());
                string asset_name = DataManager.Instance.MapAssetName(DataManager.DataType.Intrinsic, ii);
                return asset_name;
            }
            else
            {
                string s = (string)reader.Value;
                return s;
            }
        }

        public override bool CanWrite
        {
            get
            {
                return false;
            }
        }

        public override bool CanConvert(Type objectType)
        {
            return objectType == typeof(string);
        }
    }

    /// <summary>
    /// JSON converter for intrinsic ability list collections that handles migration from integer to string IDs.
    /// Converts lists of integer intrinsic IDs to lists of string asset names.
    /// </summary>
    public class IntrinsicListConverter : JsonConverter
    {
        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            throw new NotImplementedException("We shouldn't be here.");
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            List<string> dict = new List<string>();
            if (Serializer.OldVersion < DevHelper.StringAssetVersion)
            {
                JArray jArray = JArray.Load(reader);
                List<int> container = new List<int>();
                serializer.Populate(jArray.CreateReader(), container);

                foreach (int ii in container)
                {
                    string asset_name = DataManager.Instance.MapAssetName(DataManager.DataType.Intrinsic, ii);
                    dict.Add(asset_name);
                }
            }
            else
            {
                JArray jArray = JArray.Load(reader);
                serializer.Populate(jArray.CreateReader(), dict);
            }
            return dict;
        }

        public override bool CanWrite
        {
            get
            {
                return false;
            }
        }

        public override bool CanConvert(Type objectType)
        {
            return objectType == typeof(List<string>);
        }
    }




    /// <summary>
    /// JSON converter for status effect asset references that handles migration from integer to string IDs.
    /// Converts numeric status IDs from older versions to string-based asset names.
    /// </summary>
    public class StatusConverter : JsonConverter
    {
        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            throw new NotImplementedException("We shouldn't be here.");
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            if (Serializer.OldVersion < DevHelper.StringAssetVersion)
            {
                int ii = Int32.Parse(reader.Value.ToString());
                string asset_name = DataManager.Instance.MapAssetName(DataManager.DataType.Status, ii);
                return asset_name;
            }
            else
            {
                string s = (string)reader.Value;
                return s;
            }
        }

        public override bool CanWrite
        {
            get
            {
                return false;
            }
        }

        public override bool CanConvert(Type objectType)
        {
            return objectType == typeof(string);
        }
    }

    /// <summary>
    /// JSON converter for status-to-StatusEffect object mapping dictionaries that handles migration from integer to string IDs.
    /// Converts dictionaries mapping integer status IDs to StatusEffect objects to string-keyed dictionaries.
    /// </summary>
    public class StatusDictConverter : JsonConverter
    {
        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            throw new NotImplementedException("We shouldn't be here.");
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            Dictionary<string, StatusEffect> dict = new Dictionary<string, StatusEffect>();
            if (Serializer.OldVersion < DevHelper.StringAssetVersion)
            {
                JObject jObject = JObject.Load(reader);
                Dictionary<int, StatusEffect> container = new Dictionary<int, StatusEffect>();
                serializer.Populate(jObject.CreateReader(), container);

                foreach (int ii in container.Keys)
                {
                    string item_name = DataManager.Instance.MapAssetName(DataManager.DataType.Status, ii);
                    dict[item_name] = container[ii];
                }
            }
            else
            {
                JObject jObject = JObject.Load(reader);
                serializer.Populate(jObject.CreateReader(), dict);
            }
            return dict;
        }

        public override bool CanWrite
        {
            get
            {
                return false;
            }
        }

        public override bool CanConvert(Type objectType)
        {
            return objectType == typeof(Dictionary<string, MapStatus>);
        }
    }



    /// <summary>
    /// JSON converter for status set collections that handles migration from integer to string IDs.
    /// Converts HashSet of integer status IDs to HashSet of string asset names.
    /// </summary>
    public class StatusSetConverter : JsonConverter
    {
        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            throw new NotImplementedException("We shouldn't be here.");
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            HashSet<string> dict = new HashSet<string>();
            if (Serializer.OldVersion < DevHelper.StringAssetVersion)
            {
                JArray jArray = JArray.Load(reader);
                HashSet<int> container = new HashSet<int>();
                serializer.Populate(jArray.CreateReader(), container);

                foreach (int ii in container)
                {
                    string asset_name = DataManager.Instance.MapAssetName(DataManager.DataType.Status, ii);
                    dict.Add(asset_name);
                }
            }
            else
            {
                JArray jArray = JArray.Load(reader);
                serializer.Populate(jArray.CreateReader(), dict);
            }
            return dict;
        }

        public override bool CanWrite
        {
            get
            {
                return false;
            }
        }

        public override bool CanConvert(Type objectType)
        {
            return objectType == typeof(HashSet<string>);
        }
    }


    /// <summary>
    /// JSON converter for status list collections that handles migration from integer to string IDs.
    /// Converts lists of integer status IDs to lists of string asset names.
    /// </summary>
    public class StatusListConverter : JsonConverter
    {
        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            throw new NotImplementedException("We shouldn't be here.");
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            List<string> dict = new List<string>();
            if (Serializer.OldVersion < DevHelper.StringAssetVersion)
            {
                JArray jArray = JArray.Load(reader);
                List<int> container = new List<int>();
                serializer.Populate(jArray.CreateReader(), container);

                foreach (int ii in container)
                {
                    string asset_name = DataManager.Instance.MapAssetName(DataManager.DataType.Status, ii);
                    dict.Add(asset_name);
                }
            }
            else
            {
                JArray jArray = JArray.Load(reader);
                serializer.Populate(jArray.CreateReader(), dict);
            }
            return dict;
        }

        public override bool CanWrite
        {
            get
            {
                return false;
            }
        }

        public override bool CanConvert(Type objectType)
        {
            return objectType == typeof(HashSet<string>);
        }
    }


    /// <summary>
    /// JSON converter for status array collections that handles migration from integer to string IDs.
    /// Converts arrays of integer status IDs to arrays of string asset names.
    /// </summary>
    public class StatusArrayConverter : JsonConverter
    {
        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            throw new NotImplementedException("We shouldn't be here.");
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            List<string> dict = new List<string>();
            if (Serializer.OldVersion < DevHelper.StringAssetVersion)
            {
                JArray jArray = JArray.Load(reader);
                List<int> container = new List<int>();
                serializer.Populate(jArray.CreateReader(), container);

                foreach (int ii in container)
                {
                    string asset_name = DataManager.Instance.MapAssetName(DataManager.DataType.Status, ii);
                    dict.Add(asset_name);
                }
            }
            else
            {
                JArray jArray = JArray.Load(reader);
                serializer.Populate(jArray.CreateReader(), dict);
            }
            return dict.ToArray();
        }

        public override bool CanWrite
        {
            get
            {
                return false;
            }
        }

        public override bool CanConvert(Type objectType)
        {
            return objectType == typeof(string[]);
        }
    }

    /// <summary>
    /// JSON converter for skill asset references that handles migration from integer to string IDs.
    /// Converts numeric skill IDs from older versions to string-based asset names.
    /// </summary>
    public class SkillConverter : JsonConverter
    {
        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            throw new NotImplementedException("We shouldn't be here.");
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            if (Serializer.OldVersion < DevHelper.StringAssetVersion)
            {
                int ii = Int32.Parse(reader.Value.ToString());
                string asset_name = DataManager.Instance.MapAssetName(DataManager.DataType.Skill, ii);
                return asset_name;
            }
            else
            {
                string s = (string)reader.Value;
                return s;
            }
        }

        public override bool CanWrite
        {
            get
            {
                return false;
            }
        }

        public override bool CanConvert(Type objectType)
        {
            return objectType == typeof(string);
        }
    }


    /// <summary>
    /// JSON converter for relearnable skill dictionaries that handles migration from array to dictionary format.
    /// Converts older boolean array format (indexed by skill ID) to dictionary with string keys.
    /// </summary>
    public class RelearnableConverter : JsonConverter
    {
        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            throw new NotImplementedException("We shouldn't be here.");
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            Dictionary<string, bool> dict = new Dictionary<string, bool>();
            if (Serializer.OldVersion < DevHelper.StringAssetVersion)
            {
                JArray jArray = JArray.Load(reader);
                List<bool> container = new List<bool>();
                serializer.Populate(jArray.CreateReader(), container);

                for (int ii = 0; ii < container.Count; ii++)
                {
                    if (container[ii])
                    {
                        string asset_name = DataManager.Instance.MapAssetName(DataManager.DataType.Skill, ii);
                        dict[asset_name] = true;
                    }
                }
            }
            else
            {
                JObject jObject = JObject.Load(reader);
                serializer.Populate(jObject.CreateReader(), dict);
            }
            return dict;
        }

        public override bool CanWrite
        {
            get
            {
                return false;
            }
        }

        public override bool CanConvert(Type objectType)
        {
            return objectType == typeof(Dictionary<string, bool>);
        }
    }

    /// <summary>
    /// JSON converter for skill list collections that handles migration from integer to string IDs.
    /// Converts lists of integer skill IDs to lists of string asset names.
    /// </summary>
    public class SkillListConverter : JsonConverter
    {
        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            throw new NotImplementedException("We shouldn't be here.");
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            List<string> dict = new List<string>();
            if (Serializer.OldVersion < DevHelper.StringAssetVersion)
            {
                JArray jArray = JArray.Load(reader);
                List<int> container = new List<int>();
                serializer.Populate(jArray.CreateReader(), container);

                foreach (int ii in container)
                {
                    string asset_name = DataManager.Instance.MapAssetName(DataManager.DataType.Skill, ii);
                    dict.Add(asset_name);
                }
            }
            else
            {
                JArray jArray = JArray.Load(reader);
                serializer.Populate(jArray.CreateReader(), dict);
            }
            return dict;
        }

        public override bool CanWrite
        {
            get
            {
                return false;
            }
        }

        public override bool CanConvert(Type objectType)
        {
            return objectType == typeof(List<string>);
        }
    }



    /// <summary>
    /// JSON converter for skill array collections that handles migration from integer to string IDs.
    /// Converts arrays of integer skill IDs to arrays of string asset names.
    /// </summary>
    public class SkillArrayConverter : JsonConverter
    {
        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            throw new NotImplementedException("We shouldn't be here.");
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            List<string> dict = new List<string>();
            if (Serializer.OldVersion < DevHelper.StringAssetVersion)
            {
                JArray jArray = JArray.Load(reader);
                List<int> container = new List<int>();
                serializer.Populate(jArray.CreateReader(), container);

                foreach (int ii in container)
                {
                    string asset_name = DataManager.Instance.MapAssetName(DataManager.DataType.Skill, ii);
                    dict.Add(asset_name);
                }
            }
            else
            {
                JArray jArray = JArray.Load(reader);
                serializer.Populate(jArray.CreateReader(), dict);
            }
            return dict.ToArray();
        }

        public override bool CanWrite
        {
            get
            {
                return false;
            }
        }

        public override bool CanConvert(Type objectType)
        {
            return objectType == typeof(string[]);
        }
    }

    /// <summary>
    /// JSON converter for skin asset references that handles migration from integer to string IDs.
    /// Converts numeric skin IDs from older versions to string-based asset names.
    /// </summary>
    public class SkinConverter : JsonConverter
    {
        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            throw new NotImplementedException("We shouldn't be here.");
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            if (Serializer.OldVersion < DevHelper.StringAssetVersion)
            {
                int ii = Int32.Parse(reader.Value.ToString());
                string asset_name = DataManager.Instance.MapAssetName(DataManager.DataType.Skin, ii);
                return asset_name;
            }
            else
            {
                string s = (string)reader.Value;
                return s;
            }
        }

        public override bool CanWrite
        {
            get
            {
                return false;
            }
        }

        public override bool CanConvert(Type objectType)
        {
            return objectType == typeof(string);
        }
    }

    /// <summary>
    /// JSON converter for monster species asset references that handles migration from integer to string IDs.
    /// Converts numeric monster species IDs from older versions to string-based asset names.
    /// </summary>
    public class MonsterConverter : JsonConverter
    {
        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            throw new NotImplementedException("We shouldn't be here.");
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            if (Serializer.OldVersion < DevHelper.StringAssetVersion)
            {
                int ii = Int32.Parse(reader.Value.ToString());
                string asset_name = DataManager.Instance.MapAssetName(DataManager.DataType.Monster, ii);
                return asset_name;
            }
            else
            {
                string s = (string)reader.Value;
                return s;
            }
        }

        public override bool CanWrite
        {
            get
            {
                return false;
            }
        }

        public override bool CanConvert(Type objectType)
        {
            return objectType == typeof(string);
        }
    }

    /// <summary>
    /// JSON converter for monster list collections that handles migration from integer to string IDs.
    /// Converts lists of integer monster IDs to lists of string asset names.
    /// </summary>
    public class MonsterListConverter : JsonConverter
    {
        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            throw new NotImplementedException("We shouldn't be here.");
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            List<string> dict = new List<string>();
            if (Serializer.OldVersion < DevHelper.StringAssetVersion)
            {
                JArray jArray = JArray.Load(reader);
                List<int> container = new List<int>();
                serializer.Populate(jArray.CreateReader(), container);

                foreach (int ii in container)
                {
                    string asset_name = DataManager.Instance.MapAssetName(DataManager.DataType.Monster, ii);
                    dict.Add(asset_name);
                }
            }
            else
            {
                JArray jArray = JArray.Load(reader);
                serializer.Populate(jArray.CreateReader(), dict);
            }
            return dict;
        }

        public override bool CanWrite
        {
            get
            {
                return false;
            }
        }

        public override bool CanConvert(Type objectType)
        {
            return objectType == typeof(List<string>);
        }
    }


    /// <summary>
    /// JSON converter for monster unlock state dictionaries that handles migration from array to dictionary format.
    /// Converts older array-based unlock state data (indexed by monster ID) to dictionary with string keys.
    /// </summary>
    public class MonsterUnlockConverter : JsonConverter
    {
        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            throw new NotImplementedException("We shouldn't be here.");
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            Dictionary<string, GameProgress.UnlockState> dict = new Dictionary<string, GameProgress.UnlockState>();
            if (Serializer.OldVersion < DevHelper.StringAssetVersion)
            {
                JArray jArray = JArray.Load(reader);
                List<GameProgress.UnlockState> container = new List<GameProgress.UnlockState>();
                serializer.Populate(jArray.CreateReader(), container);

                for (int ii = 0; ii < container.Count; ii++)
                {
                    if (container[ii] > GameProgress.UnlockState.None)
                    {
                        if (DataManager.Instance.Conversions[DataManager.DataType.Monster].ContainsKey(ii.ToString()))
                        {
                            string asset_name = DataManager.Instance.MapAssetName(DataManager.DataType.Monster, ii);
                            int val;
                            if (int.TryParse(asset_name, out val))
                                continue;
                            dict[asset_name] = container[ii];
                        }
                    }
                }
            }
            else
            {
                JObject jObject = JObject.Load(reader);
                serializer.Populate(jObject.CreateReader(), dict);
            }
            return dict;
        }

        public override bool CanWrite
        {
            get
            {
                return false;
            }
        }

        public override bool CanConvert(Type objectType)
        {
            return objectType == typeof(Dictionary<string, GameProgress.UnlockState>);
        }
    }

    /// <summary>
    /// JSON converter for monster-to-boolean mapping dictionaries that handles migration from array to dictionary format.
    /// Converts older boolean array format (indexed by monster ID) to dictionary with string keys.
    /// </summary>
    public class MonsterBoolDictConverter : JsonConverter
    {
        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            throw new NotImplementedException("We shouldn't be here.");
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            Dictionary<string, bool> dict = new Dictionary<string, bool>();
            if (Serializer.OldVersion < DevHelper.StringAssetVersion)
            {
                JArray jArray = JArray.Load(reader);
                List<bool> container = new List<bool>();
                serializer.Populate(jArray.CreateReader(), container);

                for (int ii = 0; ii < container.Count; ii++)
                {
                    if (container[ii])
                    {
                        string asset_name = DataManager.Instance.MapAssetName(DataManager.DataType.Monster, ii);
                        dict[asset_name] = container[ii];
                    }
                }
            }
            else
            {
                JObject jObject = JObject.Load(reader);
                serializer.Populate(jObject.CreateReader(), dict);
            }
            return dict;
        }

        public override bool CanWrite
        {
            get
            {
                return false;
            }
        }

        public override bool CanConvert(Type objectType)
        {
            return objectType == typeof(Dictionary<string, bool>);
        }
    }



    /// <summary>
    /// JSON converter for item asset references that handles migration from integer to string IDs.
    /// Converts numeric item IDs from older versions to string-based asset names.
    /// </summary>
    public class ItemConverter : JsonConverter
    {
        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            throw new NotImplementedException("We shouldn't be here.");
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            if (Serializer.OldVersion < DevHelper.StringAssetVersion)
            {
                int ii = Int32.Parse(reader.Value.ToString());
                string asset_name = DataManager.Instance.MapAssetName(DataManager.DataType.Item, ii);
                return asset_name;
            }
            else
            {
                string s = (string)reader.Value;
                return s;
            }
        }

        public override bool CanWrite
        {
            get
            {
                return false;
            }
        }

        public override bool CanConvert(Type objectType)
        {
            return objectType == typeof(string);
        }
    }

    /// <summary>
    /// JSON converter for item storage dictionaries that handles migration from array to dictionary format.
    /// Converts older array-based item quantity data (indexed by item ID) to dictionary with string keys.
    /// </summary>
    public class ItemStorageConverter : JsonConverter
    {
        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            throw new NotImplementedException("We shouldn't be here.");
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            Dictionary<string, int> dict = new Dictionary<string, int>();
            if (Serializer.OldVersion < DevHelper.StringAssetVersion)
            {
                JArray jArray = JArray.Load(reader);
                List<int> container = new List<int>();
                serializer.Populate(jArray.CreateReader(), container);

                for (int ii = 0; ii < container.Count; ii++)
                {
                    if (DataManager.Instance.Conversions[DataManager.DataType.Item].ContainsKey(ii.ToString()))
                    {
                        string asset_name = DataManager.Instance.MapAssetName(DataManager.DataType.Item, ii);
                        int val;
                        if (int.TryParse(asset_name, out val))
                            continue;
                        dict[asset_name] = container[ii];
                    }
                }
            }
            else
            {
                JObject jObject = JObject.Load(reader);
                serializer.Populate(jObject.CreateReader(), dict);
            }
            return dict;
        }

        public override bool CanWrite
        {
            get
            {
                return false;
            }
        }

        public override bool CanConvert(Type objectType)
        {
            return objectType == typeof(Dictionary<string, int>);
        }
    }


    /// <summary>
    /// JSON converter for item list collections that handles migration from integer to string IDs.
    /// Converts lists of integer item IDs to lists of string asset names.
    /// </summary>
    public class ItemListConverter : JsonConverter
    {
        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            throw new NotImplementedException("We shouldn't be here.");
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            List<string> dict = new List<string>();
            if (Serializer.OldVersion < DevHelper.StringAssetVersion)
            {
                JArray jArray = JArray.Load(reader);
                List<int> container = new List<int>();
                serializer.Populate(jArray.CreateReader(), container);

                foreach (int ii in container)
                {
                    string asset_name = DataManager.Instance.MapAssetName(DataManager.DataType.Item, ii);
                    dict.Add(asset_name);
                }
            }
            else
            {
                JArray jArray = JArray.Load(reader);
                serializer.Populate(jArray.CreateReader(), dict);
            }
            return dict;
        }

        public override bool CanWrite
        {
            get
            {
                return false;
            }
        }

        public override bool CanConvert(Type objectType)
        {
            return objectType == typeof(List<string>);
        }
    }


    /// <summary>
    /// JSON converter for item range-to-list migration that handles conversion from IntRange to List format.
    /// Converts older IntRange format (Min to Max exclusive) to a list of string asset names.
    /// </summary>
    public class ItemRangeToListConverter : JsonConverter
    {
        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            throw new NotImplementedException("We shouldn't be here.");
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            List<string> dict = new List<string>();
            if (Serializer.OldVersion < DevHelper.StringAssetVersion)
            {
                JObject jObject = JObject.Load(reader);
                IntRange container = new IntRange();
                serializer.Populate(jObject.CreateReader(), container);

                for (int ii = container.Min; ii < container.Max; ii++)
                {
                    string asset_name = DataManager.Instance.MapAssetName(DataManager.DataType.Item, ii);
                    dict.Add(asset_name);
                }
            }
            else
            {
                JArray jArray = JArray.Load(reader);
                serializer.Populate(jArray.CreateReader(), dict);
            }
            return dict;
        }

        public override bool CanWrite
        {
            get
            {
                return false;
            }
        }

        public override bool CanConvert(Type objectType)
        {
            return objectType == typeof(List<string>);
        }
    }

    /// <summary>
    /// JSON converter for SegLoc-to-Map dictionary that handles version migration.
    /// Converts from older JObject format (pre-0.7.21) to tuple array format for proper SegLoc key handling.
    /// </summary>
    public class SegLocTableConverter : JsonConverter
    {
        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            Dictionary<SegLoc, Map> dict = (Dictionary<SegLoc, Map>)value;
            writer.WriteStartArray();
            foreach (SegLoc item in dict.Keys)
            {
                serializer.Serialize(writer, (item, dict[item]));
            }
            writer.WriteEndArray();
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            Dictionary<SegLoc, Map> dict = new Dictionary<SegLoc, Map>();
            //TODO: Remove in v1.1
            if (Serializer.OldVersion < new Version(0, 7, 21))
            {
                JObject jObject = JObject.Load(reader);
                serializer.Populate(jObject.CreateReader(), dict);
            }
            else
            {
                JArray jArray = JArray.Load(reader);
                List<(SegLoc, Map)> container = new List<(SegLoc, Map)>();
                serializer.Populate(jArray.CreateReader(), container);

                foreach ((SegLoc, Map) item in container)
                    dict[item.Item1] = item.Item2;
            }

            return dict;
        }

        public override bool CanConvert(Type objectType)
        {
            return objectType == typeof(Dictionary<SegLoc, Map>);
        }
    }

    /// <summary>
    /// JSON converter for SegLoc-to-integer dictionary that uses tuple array format.
    /// Serializes and deserializes using array of (SegLoc, int) tuples for proper key handling.
    /// </summary>
    public class SegLocIntTableConverter : JsonConverter
    {
        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            Dictionary<SegLoc, int> dict = (Dictionary<SegLoc, int>)value;
            writer.WriteStartArray();
            foreach (SegLoc item in dict.Keys)
            {
                serializer.Serialize(writer, (item, dict[item]));
            }
            writer.WriteEndArray();
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            Dictionary<SegLoc, int> dict = new Dictionary<SegLoc, int>();

            JArray jArray = JArray.Load(reader);
            List<(SegLoc, int)> container = new List<(SegLoc, int)>();
            serializer.Populate(jArray.CreateReader(), container);

            foreach ((SegLoc, int) item in container)
                dict[item.Item1] = item.Item2;

            return dict;
        }

        public override bool CanConvert(Type objectType)
        {
            return objectType == typeof(Dictionary<SegLoc, Map>);
        }
    }

    /// <summary>
    /// JSON converter for MonsterID-to-UnlockState dictionary that uses tuple array format.
    /// Required because the standard JSON serializer has issues deserializing MonsterID as dictionary keys.
    /// </summary>
    // Without this, the system just doesn't like deserializing MonsterID for some reason
    public class FormUnlockDictConverter : JsonConverter
    {
        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            Dictionary<MonsterID, GameProgress.UnlockState> dict = (Dictionary<MonsterID, GameProgress.UnlockState>)value;
            writer.WriteStartArray();
            foreach (MonsterID item in dict.Keys)
            {
                serializer.Serialize(writer, (item, dict[item]));
            }
            writer.WriteEndArray();
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            Dictionary<MonsterID, GameProgress.UnlockState> dict = new Dictionary<MonsterID, GameProgress.UnlockState>();

            List<(MonsterID, GameProgress.UnlockState)> container = new List<(MonsterID, GameProgress.UnlockState)>();

            try
            {
                JArray jArray = JArray.Load(reader);
                serializer.Populate(jArray.CreateReader(), container);
            }
            catch (JsonSerializationException)
            {
                // Do nothing, return empty and let OnDeserialized handle it
            }

            foreach ((MonsterID, GameProgress.UnlockState) item in container)
                dict[item.Item1] = item.Item2;

            return dict;
        }

        public override bool CanConvert(Type objectType)
        {
            return objectType == typeof(Dictionary<MonsterID, GameProgress.UnlockState>);
        }
    }
}
