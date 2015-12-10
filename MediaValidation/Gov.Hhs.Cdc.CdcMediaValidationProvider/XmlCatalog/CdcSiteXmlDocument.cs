// Copyright [2015] [Centers for Disease Control and Prevention] 
// Licensed under the CDC Custom Open Source License 1 (the 'License'); 
// you may not use this file except in compliance with the License. 
// You may obtain a copy of the License at
// 
//   http://t.cdc.gov/O4O
// 
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an 'AS IS' BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Xml;
using gov.hhs.cdc.xmlcatalog;
using Gov.Hhs.Cdc.Bo;
using Gov.Hhs.Cdc.Logging;

namespace Gov.Hhs.Cdc.CdcMediaValidationProvider
{

    public class CdcSiteXmlDocument : IDisposable
    {
        private static XmlCatalogResolver xmlResolver = null;
        private static object xmlResolverLock = new object();
        Uri Uri;
        private XmlNamespaceManager cdcNamespaceManager;
        public string ContentNamespace { get; set; }
        private static string ExtractionNamespace = "html";
        public static string ExtractionPrefix = "html:";

        XmlDocument document;
        public bool IsValid { get { return document != null; } }

        public CdcSiteXmlDocument(string data, string xmlCatalogPath, string nameSpace, Uri uri)
        {
            document = new XmlDocument();
            Uri = uri;
            if (string.IsNullOrEmpty(xmlCatalogPath))
                throw new ApplicationException("Config setting XMLCatalogPath is missing or blank");
            document.XmlResolver = GetXmlResolver(xmlCatalogPath);
            try
            {
                document.LoadXml(data);
            }
            catch (Exception ex)
            {                
                throw ex;
            }

            XmlDocument xmlDocument = new XmlDocument();
            ContentNamespace = nameSpace.ToLower();
            cdcNamespaceManager = new XmlNamespaceManager(xmlDocument.NameTable);
            cdcNamespaceManager.AddNamespace(ExtractionNamespace, "http://www.w3.org/1999/xhtml");
        }


        private static string XPathAnywhereBodyDescendant(string elementType)
        {
            if (elementType == null)
                return "";
            string str = XPathFormat("//" + ExtractionNamespace + ":body/descendant::{0}{1}", elementType);
            return str;
        }

        private static string XPathAnywhere(string elementType)
        {
            if (elementType == null)
                return string.Empty;
            return XPathFormat("//{0}{1}", elementType);
        }

        private static string XPathDescendantOfCurrentNode(string elementType)
        {
            if (elementType == null)
                return string.Empty;
            string str = XPathFormat(".//{0}{1}", elementType);
            return str;
        }

        private static string XPathFormat(string format, string elementType)
        {
            string str = string.Format(format, elementType == "*" ? "" : (ExtractionNamespace + ":"), elementType);
            return str;
        }

        private static XmlCatalogResolver GetXmlResolver(string xmlCatalogPath)
        {
            lock (xmlResolverLock)
            {
                if (xmlResolver == null)
                {

                    if (!string.IsNullOrEmpty(xmlCatalogPath))
                    {
                        string qualifiedPath = GetValidPath(xmlCatalogPath, "..\\MediaValidation\\Gov.Hhs.Cdc.CdcMediaValidationProvider");
                        Logger.LogInfo("Loading xmlCatalogPath from " + qualifiedPath);
                        xmlResolver = new XmlCatalogResolver(qualifiedPath);
                    }
                }
            }
            return xmlResolver;

        }

        private static string GetValidPath(string path, string alternateRelativeDirectory)
        {
            string attemptedPaths;
            if (!path.StartsWith("~"))
            {
                if (File.Exists(path))
                    return path;
                attemptedPaths = path;
            }
            else
            {
                string currentPath = AppDomain.CurrentDomain.BaseDirectory.TrimEnd('\\');
                currentPath = currentPath.GetLast(1) == "\\" ? currentPath.GetAllButLast(1) : currentPath;
                string path1 = currentPath + path.Substring(1);
                attemptedPaths = path1;
                if (File.Exists(path1))
                    return path1;
                string path2 = currentPath + "\\" + alternateRelativeDirectory.TrimEnd('\\') + path.Substring(1);
                attemptedPaths = attemptedPaths + "," + path2;
                if (File.Exists(path2))
                    return path2;
            }
            string message = "Unable to find the Xml file at: " + attemptedPaths;
            Logger.LogError(message);
            throw new ApplicationException("Unable to find path");
        }

        public XmlNodeList SelectNodesByXpath(string xpath)
        {
            return document.SelectNodes(HttpUtility.HtmlDecode(xpath), cdcNamespaceManager);
        }

        public string GetFirstInnerText(string elementType)
        {
            XmlNodeList nodes = SelectNodesFromCurrent(elementType);
            return (nodes != null && nodes.Count > 0) ? nodes[0].InnerText : null;
        }

        public XmlNodeList SelectNodesFromCurrent(string elementType)
        {
            return document.SelectNodes(XPathDescendantOfCurrentNode(elementType), cdcNamespaceManager);
        }

        public XmlNodeList SelectNodesFrom(XmlNode fromElement, string elementType)
        {
            return fromElement.SelectNodes(XPathDescendantOfCurrentNode(elementType), cdcNamespaceManager);
        }

        public XmlNodeList SelectNodesFrom(XmlNode fromElement, XPathSelection selection)
        {
            return fromElement.SelectNodes(XPathDescendantOfCurrentNode(selection.ElementType) + selection.AttributeSelection.Value, cdcNamespaceManager);
        }

        public XmlNodeList SelectNodesFrom(XmlNode fromElement, string elementName, AttributeSelection attributeSelection)
        {
            return fromElement.SelectNodes(XPathDescendantOfCurrentNode(elementName) + attributeSelection.Value, cdcNamespaceManager);
        }

        public XmlNodeList SelectNodesAnywhere(string elementType)
        {
            return document.SelectNodes(XPathAnywhere(elementType), cdcNamespaceManager);
        }

        public XmlNodeList SelectNodesFromBody(XPathSelection selection)
        {
            return document.SelectNodes(XPathAnywhereBodyDescendant(selection.ElementType) + selection.AttributeSelection.Value, cdcNamespaceManager);
        }

        public XmlNode RemoveNodesFrom(XmlNode node, XPathSelection selection, bool keepChildren = false)
        {
            if (selection != null)
            {
                XmlNodeList excludedNodes = SelectNodesFrom(node, selection);
                int i = 0;
                //Give up if there are too many tries
                while (excludedNodes != null && excludedNodes.Count > 0 && i++ <= 1000)
                {
                    RemoveNode(excludedNodes[0], keepChildren);
                    excludedNodes = SelectNodesFrom(node, selection);
                    i++;
                }
                if (i >= 1000)
                {
                    var selMessage = ". Selection: " + XPathDescendantOfCurrentNode(selection.ElementType) + selection.AttributeSelection.Value;
                    Logger.LogWarning("Found more than 1000 items to exclude in " + Uri.AbsoluteUri + selMessage);
                }
            }
            return node;
        }

        private static void RemoveNode(XmlNode excludedNode, bool keepChildren)
        {
            XmlNode parentNode = excludedNode.ParentNode;
            string a = parentNode.OuterXml;
            XmlNodeList childNodes = excludedNode.ChildNodes;

            parentNode.RemoveChild(excludedNode);
            if (keepChildren && childNodes != null && childNodes.Count > 0)
            {
                foreach (XmlNode childNode in childNodes)
                {
                    parentNode.AppendChild(childNode);
                }
            }
        }

        public void RemoveAttributeFrom(XmlNode node, string elementName, string attributeName)
        {
            XmlNodeList nodes = SelectNodesFrom(node, elementName, new AttributeSelection(attributeName));
            RemoveAttributeFrom(nodes, attributeName);
        }

        public void RemoveAttributeFrom(XmlNodeList nodes, string attributeName)
        {
            if (nodes != null)
            {
                foreach (XmlElement selectedNode in nodes)
                {
                    if (attributeName == "href")
                    {
                        XmlElement elem = document.CreateElement("span");
                        //elem.InnerText = selectedNode.InnerText;

                        if (selectedNode.HasChildNodes)
                            foreach (XmlNode obj in selectedNode.ChildNodes)
                                elem.AppendChild(obj);

                        selectedNode.ParentNode.ReplaceChild(elem, selectedNode);
                    }
                    selectedNode.RemoveAttribute(attributeName);
                }
            }
        }

        public static void SetAttribute(XmlNodeList nodes, string attributeName, string attributeValue)
        {
            if (nodes == null) return;
            foreach (XmlElement element in nodes)
            {
                element.SetAttribute(attributeName, attributeValue);
            }
        }

        public void AppendAttribute(XmlNodeList nodes, string attributeName, string attributeValue)
        {
            if (nodes == null)
                return;
            foreach (XmlElement element in nodes)
            {
                AppendAttribute(element, attributeName, attributeValue.Split(':'));
            }
        }

        private void AppendAttribute(XmlElement element, string attributeName, string[] attributeValue)
        {
            List<string> values = GetAttributeAsList(element, attributeName)
                .Where(v => !KeyValuePairKeyEquals(attributeValue[0], v)).ToList();
            values.Add(string.Join(":", attributeValue));
            element.SetAttribute(attributeName, string.Join(";", values.ToArray()) + ";");
        }

        private static bool KeyValuePairKeyEquals(string attributeName, string v)
        {
            return string.Equals(v.Split(':')[0], attributeName, StringComparison.OrdinalIgnoreCase);
        }

        private static List<string> GetAttributeAsList(XmlNode element, string attributeName)
        {
            if (element.Attributes == null || element.Attributes[attributeName] == null || element.Attributes[attributeName].Value == null)
                return new List<string>();

            string value = element.Attributes[attributeName].Value.Trim().TrimEnd(';');
            //string value = "abc:abc;;xyz:;float:middle;last:abc";
            return value.Split(';').Where(v => v.Trim() != "").ToList();

        }

        /// <summary>
        /// Don't think we need this, but we are running out of memory and this is an attempt to dispose of the XML documents
        /// </summary>
        public void Dispose()
        {
            document = null;
            cdcNamespaceManager = null;
        }
    }
}
