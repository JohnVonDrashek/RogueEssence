using System.Xml;

namespace RogueEssence
{
    /// <summary>
    /// Provides extension methods for working with XML documents.
    /// </summary>
    public static class XmlExt
    {
        /// <summary>
        /// Creates and appends a child element with inner text to a parent node.
        /// </summary>
        /// <param name="parentNode">The parent node to append to.</param>
        /// <param name="doc">The XML document used to create the element.</param>
        /// <param name="name">The name of the new element.</param>
        /// <param name="text">The inner text content of the new element.</param>
        /// <returns>The newly created and appended node.</returns>
        public static XmlNode AppendInnerTextChild(this XmlNode parentNode, XmlDocument doc, string name, string text)
        {
            XmlNode node = doc.CreateElement(name);
            node.InnerText = text;
            parentNode.AppendChild(node);
            return node;
        }
    }
}
