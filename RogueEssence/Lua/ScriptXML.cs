using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml;
using NLua;
using System.IO;

namespace RogueEssence.Script
{
    /// <summary>
    /// Provides XML file handling functions for Lua scripts.
    /// Allows loading and parsing XML files into usable data structures.
    /// </summary>
    class ScriptXML : ILuaEngineComponent
    {
        LuaFunction InsertChildNodeType;

        /// <summary>
        /// Loads an XML file and returns its root element.
        /// </summary>
        /// <param name="filepath">The path to the XML file.</param>
        /// <returns>The root XmlElement, or null if the file doesn't exist.</returns>
        public XmlElement LoadXmlFileToTable(string filepath)
        {
            try
            {
                LuaTable tbl = LuaEngine.Instance.RunString("return {}").First() as LuaTable;


                if (!File.Exists(filepath))
                    return null;

                XmlDocument xmldoc = new XmlDocument();
                xmldoc.Load(filepath);

                //foreach (XmlNode xnode in xmldoc.DocumentElement.ChildNodes)
                //{
                //    InsertChildNodeType.Call(tbl, xnode.Name, AddNode(xnode));
                //    //LuaEngine.Instance.CallLuaFunctions("table.insert", tbl, AddNode(xnode));
                //}


                return xmldoc.DocumentElement;
            }
            catch (Exception ex)
            {
                DiagManager.Instance.LogError(ex);
                return null;
            }
        }

        /// <summary>
        /// Gets a named child element from an XML node.
        /// </summary>
        /// <param name="parent">The parent node to search in.</param>
        /// <param name="nodename">The name of the child node to find.</param>
        /// <returns>The child XmlElement, or null if not found.</returns>
        public XmlElement GetXmlNodeNamedChild(XmlNode parent, string nodename)
        {
            return parent[nodename];
        }

        /// <summary>
        /// Gets the inner text content of an XML node.
        /// </summary>
        /// <param name="node">The node to get text from.</param>
        /// <returns>The inner text of the node.</returns>
        public string GetXmlNodeText(XmlNode node)
        {
            return node.InnerText;
        }


        //private LuaTable AddNode(XmlNode curnode)
        //{
        //    LuaTable curtbl = LuaEngine.Instance.RunString("return {}").First() as LuaTable;

        //    if (curnode.HasChildNodes)
        //    {
        //        foreach (XmlNode xnode in curnode.ChildNodes)
        //            curtbl[xnode.Name] = AddNode(xnode);
        //    }
        //    else if(!String.IsNullOrEmpty(curnode.Value))
        //    {
        //        //curtbl = LuaEngine.Instance.RunString(@"
        //        //local tbl = {}
        //        //table.insert(tbl,'" + curnode.Value + @"')
        //        //return tbl
        //        //").First() as LuaTable;

        //        //LuaEngine.Instance.CallLuaFunctions("table.insert", curtbl, curnode.Value);
        //        InsertChildNodeType.Call(curtbl, curnode.Name, AddNode(curnode));
        //    }
        //    return curtbl;
        //}

        /// <summary>
        /// Sets up Lua function wrappers for XML operations.
        /// </summary>
        /// <param name="state">The Lua engine state.</param>
        public override void SetupLuaFunctions(LuaEngine state)
        {
            InsertChildNodeType = state.RunString("return function(tbl, nodename, value) table.insert( tbl[nodename], value); end").First() as LuaFunction;
        }
    }
}