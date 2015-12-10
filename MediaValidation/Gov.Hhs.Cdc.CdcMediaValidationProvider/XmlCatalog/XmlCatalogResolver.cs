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

/*
 Copyright 2009 Londata Limited.
 
 Licensed under the Apache License, Version 2.0 (the "License");
 you may not use this file except in compliance with the License.
 You may obtain a copy of the License at
 
 http://www.apache.org/licenses/LICENSE-2.0
 
 Unless required by applicable law or agreed to in writing, software
 distributed under the License is distributed on an "AS IS" BASIS,
 WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
 See the License for the specific language governing permissions and
 limitations under the License.
 */
//using Common.Logging;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Xml;
using System.Web;
using Gov.Hhs.Cdc.CdcMediaValidationProvider;

namespace gov.hhs.cdc.xmlcatalog
{

    /// <summary>
    /// API for simple URI resolver classes that try a single resolution strategy,
    /// e.g. a "string match and replace" strategy.
    /// </summary>
    public interface SimpleUrlResolver
    {

        /// <summary>
        /// Resolves a URI, if possible, to a locally controlled or trusted URI.
        /// </summary>
        /// <returns>A resolved URI, or null if the resolver can't match the parameters</returns>
        /// <param name="baseUri">Base URI for relative URIs</param>
        /// <param name="relativeUri">An absolute or relative URI</param>
        Uri ResolveUri(Uri baseUri, String relativeUri);

    }

    /// <summary>
    /// Replacement for the built-in .NET "XmlUrlResolver" class.
    /// Implements OASIS XML Catalogs mechanism for resolving URIs, etc.
    /// </summary>
    /// <remarks>
    /// <para>This is the only class that users need to use directly.</para>
    /// </remarks>
    public class XmlCatalogResolver : XmlUrlResolver
    {

        //  protected ILog logger = LogManager.GetLogger(Constants.DEFAULT_LOGGER_NAME);


        /// <summary>
        /// Enumeration of the different types of resolver preference.
        /// </summary>
        public enum PreferType
        {
            /// <summary>
            /// Use preferences as defined in catalogs.
            /// </summary>
            Catalog = 1,
            /// <summary>
            /// Always prefer system IDs to public IDs (override settings in catalogs).
            /// </summary>
            System = 2,
            /// <summary>
            /// Always prefer public IDs to system IDs (override settings in catalogs).
            /// </summary>
            Public = 3
        };

        /// <summary>
        /// Preference for the order in which catalog substitutions are tried.
        /// </summary>
        public PreferType Prefer = PreferType.Catalog;

        /// <summary>
        /// List of resolver objects that are used in turn to try to do URI resolution,
        /// sorted in order based on catalog order and public/system preferences.
        /// </summary>
        private ArrayList AllResolvers = new ArrayList();

        /// <summary>
        /// Public ID resolver objects that are used in turn to try to do URI resolution.
        /// </summary>
        private ArrayList PublicResolvers = new ArrayList();

        /// <summary>
        /// System ID resolver objects that are used in turn to try to do URI resolution.
        /// </summary>
        private ArrayList SystemResolvers = new ArrayList();

        /// <summary>
        /// Other resolver objects that are used in turn to try to do URI resolution.
        /// </summary>
        private ArrayList OtherResolvers = new ArrayList();

        /// <summary>
        /// Default constructor.
        /// </summary>
        public XmlCatalogResolver()
        {
            // nothing to do
        }

        /// <summary>
        /// Constructor that takes a catalog path list.
        /// </summary>
        /// <param name="catalogPaths">A semicolon-separated list of catalog paths</param>
        public XmlCatalogResolver(String catalogPaths)
        {
            ReloadCatalogs(catalogPaths);
        }

        /// <summary>
        /// Constructor that takes a catalog file array.
        /// </summary>
        /// <param name="catalogUris">An array of catalog file URIs</param>
        public XmlCatalogResolver(Uri[] catalogUris)
        {
            ReloadCatalogs(catalogUris);
        }

        /// <summary>
        /// Resolves a URI if possible.
        /// </summary>
        /// <returns>A resolved URI, or the original relative URI if it cannot be resolved.</returns>
        /// <param name="baseUri">Base URI for relative URIs</param>
        /// <param name="relativeUri">An absolute or relative URI</param>
        public override Uri ResolveUri(Uri baseUri, String relativeUri)
        {
            /*
             * DTDs:
             * .NET calls this multiple times as required:
             * #1: no base URI, relative URI for XML file
             * #2: no base URI, relative URI is resolved URI for XML file (result from #1?)
             * #3: base URI is resolved URI for XML file, relative URI is public ID for DTD
             * #4: base URI is resolved URI for XML file, relative URI is relative URI for DTD
             *
             * Schemas:
             * .NET calls this multiple times as required:
             * #1: no base URI, relative URI for XML file
             * #2: base URI is resolved URI for XML file, relative URI is relative URI for Schema
             */
            //System.Diagnostics.Debug.WriteLine("ResolveUri:");
            //System.Diagnostics.Debug.WriteLine("  baseUri: " + baseUri);
            //System.Diagnostics.Debug.WriteLine("  relativeUri: " + relativeUri);
            //System.Diagnostics.Debug.WriteLine("  prefer: " + Prefer);
            Uri result = null;

            if (PreferType.Catalog == Prefer)
            {
                // Prefer public or system IDs as defined in catalogs (system is default).
                result = ResolveUri(baseUri, relativeUri, AllResolvers);
            }
            else
            {
                // Prefer either public or system IDs.
                if (PreferType.Public == Prefer)
                {
                    // Try public IDs first, if public IDs are preferred.
                    result = ResolveUri(baseUri, relativeUri, PublicResolvers);
                }
                if (result == null)
                {
                    // Try system IDs.
                    result = ResolveUri(baseUri, relativeUri, SystemResolvers);
                }
                if ((result == null) && (PreferType.Public != Prefer))
                {
                    // Try public IDs, if public IDs are not preferred.
                    result = ResolveUri(baseUri, relativeUri, PublicResolvers);
                }
                if (result == null)
                {
                    // Try all other URI substitutions.
                    result = ResolveUri(baseUri, relativeUri, OtherResolvers);
                }
            }

            if (result == null)
            {
                // If no substitutions matched, try resolving the relative URI against the base URI.
                result = base.ResolveUri(baseUri, relativeUri);
            }

            //System.Diagnostics.Debug.WriteLine("  resolved to: " + result);
            return result;
        }

        /// <summary>
        /// Resolves a URI if possible, using the given simple URL resolvers.
        /// </summary>
        /// <returns>A resolved URI, or the original relative URI if it cannot be resolved.</returns>
        /// <param name="baseUri">Base URI for relative URIs</param>
        /// <param name="relativeUri">An absolute or relative URI</param>
        /// <param name="resolvers">List of simple URL resolvers that are tried in turn to perform the URI resolution</param>
        private Uri ResolveUri(Uri baseUri, String relativeUri, ArrayList resolvers)
        {
            if (resolvers == null)
            {
                return null;
            }

            Uri result = null;
            foreach (SimpleUrlResolver resolver in resolvers)
            {
                result = resolver.ResolveUri(baseUri, relativeUri);
                if (result != null)
                {
                    break;
                }
            }

            return result;
        }

        /// <summary>
        /// Reloads catalog details from a set of catalog paths
        /// </summary>
        /// <param name="catalogPaths">A semicolon-separated list of catalog paths or URIs</param>
        public void ReloadCatalogs(String catalogPaths)
        {
            //System.Diagnostics.Debug.WriteLine("Converting catalog paths to URIs: " + catalogPaths);
            string[] splitPaths = catalogPaths.Split(';');
            if (splitPaths.Length >= 1)
            {
                Uri[] catalogUris = new Uri[splitPaths.Length];
                for (int idx = 0; idx < splitPaths.Length; idx++)
                {
                    //System.Diagnostics.Debug.WriteLine("  catalog path: " + splitPaths[idx]);

                    catalogUris[idx] = null;

                    try
                    {
                        // Try the path as something that can be directly converted to a URI.
                        catalogUris[idx] = new Uri(splitPaths[idx]);
                    }
                    catch (Exception e)
                    {
                        string message = e.Message; // this is to avoid a compiler warning
                        // Try the path as a relative file path.
                        catalogUris[idx] = new Uri(new FileInfo(splitPaths[idx]).FullName);
                    }

                    //System.Diagnostics.Debug.WriteLine("  catalog URI: " + catalogUris[idx]);
                }
                ReloadCatalogs(catalogUris);
            }
            else
            {
                throw new Exception("No catalog paths found in: " + catalogPaths);
            }
        }

        /// <summary>
        /// Reloads catalog details from a set of catalog files
        /// </summary>
        /// <param name="catalogUris">An array of catalog file URIs</param>
        public void ReloadCatalogs(Uri[] catalogUris)
        {
            //System.Diagnostics.Debug.WriteLine("Reloading catalogs ...");

            PublicResolvers = new ArrayList();
            SystemResolvers = new ArrayList();
            OtherResolvers = new ArrayList();
            foreach (Uri catalogUri in catalogUris)
            {
                //System.Diagnostics.Debug.WriteLine("  catalog: " + catalogUri);
                ProcessCatalogFile(catalogUri);
            }

            //System.Diagnostics.Debug.WriteLine("Finished loading catalogs.");
        }

        /// <summary>
        /// Processes a catalog file and adds resolver objects to the list as appropriate.
        /// </summary>
        /// <param name="catalogUri">URI for the catalog.</param>
        private void ProcessCatalogFile(
            Uri catalogUri
        )
        {
            //System.Diagnostics.Debug.WriteLine("Processing catalog file: " + catalogUri);
            String catalogUriString = catalogUri.GetComponents(
                UriComponents.Scheme | UriComponents.HostAndPort | UriComponents.Path | UriComponents.KeepDelimiter,
                UriFormat.Unescaped
            );
            int lastForwardSlashIndex = catalogUriString.LastIndexOf('/');
            string baseUriString = catalogUriString.Substring(0, lastForwardSlashIndex);
            //System.Diagnostics.Debug.WriteLine("xml:base URI string: " + baseUriString);
            Uri catalogBaseUri = new Uri(baseUriString);

            XmlBaseStack xmlBaseStack = new XmlBaseStack();
            xmlBaseStack.Push(catalogBaseUri);
            //System.Diagnostics.Debug.WriteLine("Initial xml:base is catalog directory: " + xmlBaseStack.GetXmlBase());

            Stack<bool> preferPublicStack = new Stack<bool>();
            preferPublicStack.Push(false);

            XmlReaderSettings settings = new XmlReaderSettings();
            settings.ValidationType = ValidationType.None; // no need for validation            
            settings.DtdProcessing = DtdProcessing.Parse;
            settings.XmlResolver = null; // try to stop any attempt to load a DTD or Schema for the catalog
            XmlReader catalogReader = XmlReader.Create(catalogUri.AbsoluteUri, settings);

            int AllResolversInsertIndex = AllResolvers.Count;

            while (catalogReader.Read())
            {
                if (XmlNodeType.Element == catalogReader.NodeType)
                {
                    // Handle "xml:base" attribute.
                    string xmlBase = catalogReader.GetAttribute("xml:base");
                    if (xmlBase != null)
                    {
                        xmlBaseStack.Push(xmlBase);
                    }
                    else
                    {
                        xmlBaseStack.Push(xmlBaseStack.GetXmlBase());
                    }

                    // Handle "prefer" attribute.
                    string preferValue = catalogReader.GetAttribute("prefer");
                    if ("public".Equals(preferValue))
                    {
                        preferPublicStack.Push(true);
                    }
                    else if ("system".Equals(preferValue))
                    {
                        preferPublicStack.Push(false);
                    }
                    else
                    {
                        preferPublicStack.Push(preferPublicStack.Peek());
                    }

                    // Now process the element.
                    if ("catalog".Equals(catalogReader.LocalName))
                    {
                        //System.Diagnostics.Debug.WriteLine("  <catalog> ...");
                        LogCommonAttributes(catalogReader, preferPublicStack, xmlBaseStack);
                    }
                    else if ("group".Equals(catalogReader.LocalName))
                    {
                        //System.Diagnostics.Debug.WriteLine("  <group> ...");
                        LogCommonAttributes(catalogReader, preferPublicStack, xmlBaseStack);
                    }
                    else if ("nextCatalog".Equals(catalogReader.LocalName))
                    {
                        //System.Diagnostics.Debug.WriteLine("  <nextCatalog> ...");
                        LogCommonAttributes(catalogReader, preferPublicStack, xmlBaseStack);

                        string nextCatalogPath = catalogReader.GetAttribute("catalog");
                        Uri nextCatalogUri = new Uri(xmlBaseStack.GetXmlBase(), nextCatalogPath);

                        //System.Diagnostics.Debug.WriteLine("    catalog: " + nextCatalogPath);
                        //System.Diagnostics.Debug.WriteLine("    resolved to: " + nextCatalogUri);

                        ProcessCatalogFile(nextCatalogUri);
                    }
                    else if ("public".Equals(catalogReader.LocalName))
                    {
                        // TODO: handle "publicid" URNs
                        //System.Diagnostics.Debug.WriteLine("  <public> ...");
                        LogCommonAttributes(catalogReader, preferPublicStack, xmlBaseStack);

                        string matchString = catalogReader.GetAttribute("publicId");
                        string normalizedMatchString = Normalize(matchString);
                        string replacementString = catalogReader.GetAttribute("uri");
                        String replacementUri = new Uri(xmlBaseStack.GetXmlBase(), replacementString).ToString();

                        //System.Diagnostics.Debug.WriteLine("    publicId: " + matchString);
                        //System.Diagnostics.Debug.WriteLine("    normalized publicId: " + normalizedMatchString);
                        //System.Diagnostics.Debug.WriteLine("    uri: " + replacementString);
                        //System.Diagnostics.Debug.WriteLine("    resolved to: " + replacementUri);

                        SimpleUrlResolver newResolver = new StringMatchUrlResolver(
                            "public", matchString, replacementUri,
                            StringMatchUrlResolver.StringMatchType.MatchAll
                        );
                        SystemResolvers.Add(newResolver);

                        if (preferPublicStack.Peek()) // prefer public
                        {
                            // Insert before other resolvers from this catalog.
                            AllResolvers.Insert(AllResolversInsertIndex, newResolver);
                            AllResolversInsertIndex++;
                        }
                        else // prefer system
                        {
                            AllResolvers.Add(newResolver);
                        }
                    }
                    else if ("rewriteSystem".Equals(catalogReader.LocalName))
                    {
                        //System.Diagnostics.Debug.WriteLine("  <rewriteSystem> ...");
                        LogCommonAttributes(catalogReader, preferPublicStack, xmlBaseStack);

                        string matchString = catalogReader.GetAttribute("systemIdStartString");
                        string replacementString = catalogReader.GetAttribute("rewritePrefix");

                        //System.Diagnostics.Debug.WriteLine("    systemIdStartString: " + matchString);
                        //System.Diagnostics.Debug.WriteLine("    rewritePrefix: " + replacementString);

                        SimpleUrlResolver newResolver = new StringMatchUrlResolver(
                            "rewriteSystem", matchString, replacementString,
                            StringMatchUrlResolver.StringMatchType.RewritePrefix
                        );
                        SystemResolvers.Add(newResolver);

                        if (preferPublicStack.Peek()) // prefer public
                        {
                            AllResolvers.Add(newResolver);
                        }
                        else // prefer system
                        {
                            // Insert before other resolvers from this catalog.
                            AllResolvers.Insert(AllResolversInsertIndex, newResolver);
                            AllResolversInsertIndex++;
                        }
                    }
                    else if ("rewriteUri".Equals(catalogReader.LocalName))
                    {
                        //System.Diagnostics.Debug.WriteLine("  <rewriteUri> ...");
                        LogCommonAttributes(catalogReader, preferPublicStack, xmlBaseStack);

                        string matchString = catalogReader.GetAttribute("uriStartString");
                        string replacementString = catalogReader.GetAttribute("rewritePrefix");

                        //System.Diagnostics.Debug.WriteLine("    uriStartString: " + matchString);
                        //System.Diagnostics.Debug.WriteLine("    rewritePrefix: " + replacementString);

                        SimpleUrlResolver newResolver = new StringMatchUrlResolver(
                            "rewriteUri", matchString, replacementString,
                            StringMatchUrlResolver.StringMatchType.RewritePrefix
                        );
                        OtherResolvers.Add(newResolver);
                        AllResolvers.Add(newResolver);
                    }
                    else if ("system".Equals(catalogReader.LocalName))
                    {
                        //System.Diagnostics.Debug.WriteLine("  <system> ...");
                        LogCommonAttributes(catalogReader, preferPublicStack, xmlBaseStack);

                        string matchString = catalogReader.GetAttribute("systemId");
                        string replacementString = catalogReader.GetAttribute("uri");
                        string replacementUri = new Uri(xmlBaseStack.GetXmlBase(), replacementString).ToString();

                        //System.Diagnostics.Debug.WriteLine("    systemId: " + matchString);
                        //System.Diagnostics.Debug.WriteLine("    uri: " + replacementString);
                        //System.Diagnostics.Debug.WriteLine("    resolved to: " + replacementUri);

                        SimpleUrlResolver newResolver = new StringMatchUrlResolver(
                            "system", matchString, replacementUri,
                            StringMatchUrlResolver.StringMatchType.MatchAll
                        );
                        SystemResolvers.Add(newResolver);

                        if (preferPublicStack.Peek()) // prefer public
                        {
                            AllResolvers.Add(newResolver);
                        }
                        else // prefer system
                        {
                            // Insert before other resolvers from this catalog.
                            AllResolvers.Insert(AllResolversInsertIndex, newResolver);
                            AllResolversInsertIndex++;
                        }
                    }
                    else if ("systemSuffix".Equals(catalogReader.LocalName))
                    {
                        //System.Diagnostics.Debug.WriteLine("  <systemSuffix> ...");
                        LogCommonAttributes(catalogReader, preferPublicStack, xmlBaseStack);

                        string matchString = catalogReader.GetAttribute("systemIdSuffix");
                        string replacementString = catalogReader.GetAttribute("uri");
                        string replacementUri = new Uri(xmlBaseStack.GetXmlBase(), replacementString).ToString();

                        //System.Diagnostics.Debug.WriteLine("    systemIdSuffix: " + matchString);
                        //System.Diagnostics.Debug.WriteLine("    uri: " + replacementString);
                        //System.Diagnostics.Debug.WriteLine("    resolved to: " + replacementUri);

                        SimpleUrlResolver newResolver = new StringMatchUrlResolver(
                            "systemSuffix", matchString, replacementUri,
                            StringMatchUrlResolver.StringMatchType.MatchSuffix
                        );
                        SystemResolvers.Add(newResolver);

                        if (preferPublicStack.Peek()) // prefer public
                        {
                            AllResolvers.Add(newResolver);
                        }
                        else // prefer system
                        {
                            // Insert before other resolvers from this catalog.
                            AllResolvers.Insert(AllResolversInsertIndex, newResolver);
                            AllResolversInsertIndex++;
                        }
                    }
                    else if ("uri".Equals(catalogReader.LocalName))
                    {
                        //System.Diagnostics.Debug.WriteLine("  <uri> ...");
                        LogCommonAttributes(catalogReader, preferPublicStack, xmlBaseStack);

                        string matchString = catalogReader.GetAttribute("name");
                        string replacementString = catalogReader.GetAttribute("uri");
                        string replacementUri = new Uri(xmlBaseStack.GetXmlBase(), replacementString).ToString();

                        //System.Diagnostics.Debug.WriteLine("    name: " + matchString);
                        //System.Diagnostics.Debug.WriteLine("    uri: " + replacementString);
                        //System.Diagnostics.Debug.WriteLine("    resolved to: " + replacementUri);

                        SimpleUrlResolver newResolver = new StringMatchUrlResolver(
                            "uri", matchString, replacementUri,
                            StringMatchUrlResolver.StringMatchType.MatchAll
                        );
                        OtherResolvers.Add(newResolver);
                        AllResolvers.Add(newResolver);
                    }
                    else if ("uriSuffix".Equals(catalogReader.LocalName))
                    {
                        //System.Diagnostics.Debug.WriteLine("  <uriSuffix> ...");
                        LogCommonAttributes(catalogReader, preferPublicStack, xmlBaseStack);

                        string matchString = catalogReader.GetAttribute("uriSuffix");
                        string replacementString = catalogReader.GetAttribute("uri");
                        string replacementUri = new Uri(xmlBaseStack.GetXmlBase(), replacementString).ToString();

                        //System.Diagnostics.Debug.WriteLine("    uriSuffix: " + matchString);
                        //System.Diagnostics.Debug.WriteLine("    uri: " + replacementString);
                        //System.Diagnostics.Debug.WriteLine("    resolved to: " + replacementUri);

                        SimpleUrlResolver newResolver = new StringMatchUrlResolver(
                            "uriSuffix", matchString, replacementUri,
                            StringMatchUrlResolver.StringMatchType.MatchSuffix
                        );
                        OtherResolvers.Add(newResolver);
                        AllResolvers.Add(newResolver);
                    }
                    else
                    {
                        // TODO: support "delegatePublic"
                        // TODO: support "delegateSystem"
                        // TODO: support "delegateURI"
                        throw new Exception("Unexpected or unsupported element: " + catalogReader.LocalName);
                    }
                }
                else if (XmlNodeType.EndElement == catalogReader.NodeType)
                {
                    xmlBaseStack.Pop();
                    preferPublicStack.Pop();
                }
            }
        }

        private static void LogCommonAttributes(
            XmlReader reader,
            Stack<bool> preferPublicStack, XmlBaseStack xmlBaseStack
        )
        {
            /*
            if (reader.GetAttribute("id") != null)
            {
                System.Diagnostics.Debug.WriteLine("    id: " + reader.GetAttribute("id"));
            }
            if (reader.GetAttribute("prefer") != null)
            {
                System.Diagnostics.Debug.WriteLine("    prefer: " + reader.GetAttribute("prefer"));
            }
            else
            {
                System.Diagnostics.Debug.WriteLine("    current prefer: " + (preferPublicStack.Peek() ? "public" : "system"));
            }
            if (reader.GetAttribute("xml:base") != null)
            {
                System.Diagnostics.Debug.WriteLine("    xml:base: " + reader.GetAttribute("xml:base"));
            }
            else
            {
                System.Diagnostics.Debug.WriteLine("    current xml:base: " + xmlBaseStack.GetXmlBase());
            }
            */
        }

        /// <summary>
        /// Normalizes the whitespace in the given string.
        /// This is required to support Mono.
        /// </summary>
        /// <returns>Whitespace normalized string</returns>
        /// <param name="str">A string</param>
        private static string Normalize(string str)
        {
            if ((str == null) || (str.Length < 1))
            {
                return str;
            }

            int startIndex = 0;
            while ((startIndex < str.Length) && (Char.IsWhiteSpace(str, startIndex)))
            {
                startIndex++;
            }

            int endIndex = str.Length;
            while ((endIndex > 0) && (Char.IsWhiteSpace(str, endIndex - 1)))
            {
                endIndex--;
            }

            StringBuilder result = new StringBuilder();

            bool lastWasWhitespace = false;
            for (int index = startIndex; index < endIndex; index++)
            {
                char ch = str[index];
                if (Char.IsWhiteSpace(ch))
                {
                    if (!lastWasWhitespace)
                    {
                        result.Append(' ');
                    }
                    lastWasWhitespace = true;
                }
                else
                {
                    result.Append(ch);
                    lastWasWhitespace = false;
                }
            }

            return result.ToString();
        }

    }

    public class StringMatchUrlResolver : SimpleUrlResolver
    {

        /// <summary>
        /// Log4net logger.
        /// </summary>

        /// <summary>
        /// Enumeration of the different types of string match.
        /// </summary>
        public enum StringMatchType
        {
            /// <summary>
            /// Perform a full string match and replace.
            /// </summary>
            MatchAll = 1,
            /// <summary>
            /// Perform a suffix match and replace.
            /// </summary>
            MatchSuffix = 2,
            /// <summary>
            /// Perform a prefix match and rewrite.
            /// </summary>
            RewritePrefix = 3
        };

        /// <summary>
        /// Type of string match that is used.
        /// </summary>
        StringMatchType MatchType = StringMatchType.MatchAll;

        /// <summary>
        /// Name of the catalog element from which this resolver was derived.
        /// </summary>
        String ElementName = null;

        /// <summary>
        /// URI or string which is matched against the URI being resolved.
        /// </summary>
        String MatchString = null;

        /// <summary>
        /// Absolute or relative URI, returned if the match is successful.
        /// </summary>
        String ReplacementString = null;

        /// <summary>
        /// Constructor that takes a match string and a replacement URI.
        /// </summary>
        /// <param name="elementName">Name of the catalog element from which this resolver was derived.</param>
        /// <param name="matchString">URI or string which is matched</param>
        /// <param name="replacementString">Replacement string that is used when there is a match</param>
        public StringMatchUrlResolver(String elementName, String matchString, String replacementString)
        {
            ElementName = elementName;
            MatchString = matchString;
            ReplacementString = replacementString;
        }

        /// <summary>
        /// Constructor that takes a match string, a replacement URI, and a match type.
        /// </summary>
        /// <param name="elementName">Name of the catalog element from which this resolver was derived.</param>
        /// <param name="matchString">URI or string which is matched</param>
        /// <param name="replacementString">Replacement string that is used when there is a match</param>
        /// <param name="matchType">Type of string match that is used</param>
        public StringMatchUrlResolver(String elementName, String matchString, String replacementString, StringMatchType matchType)
            : this(elementName, matchString, replacementString)
        {
            MatchType = matchType;
        }

        /// <summary>
        /// Resolves a URI or string, if possible, to a locally controlled or trusted URI.
        /// </summary>
        /// <returns>A System.Uri, or null if the resolver can't match the URI</returns>
        /// <param name="baseUri">Base URI for relative URIs</param>
        /// <param name="relativeUri">An absolute or relative URI</param>
        public Uri ResolveUri(Uri baseUri, String relativeUri)
        {
            System.Diagnostics.Debug.WriteLine("  " + ElementName + ": checking (" + MatchType.ToString() + ") against: " + MatchString);

            if ((relativeUri == null) || (MatchString == null))
            {
                return null;
            }

            Uri result = null;

            if (StringMatchType.MatchAll == MatchType)
            {
                if (MatchString.Equals(relativeUri))
                {
                    result = new Uri(ReplacementString);
                }
            }
            else if (StringMatchType.MatchSuffix == MatchType)
            {
                if (relativeUri.EndsWith(MatchString))
                {
                    result = new Uri(ReplacementString);
                }
            }
            else if (StringMatchType.RewritePrefix == MatchType)
            {
                if (relativeUri.StartsWith(MatchString))
                {
                    result = new Uri(ReplacementString + relativeUri.Substring(ReplacementString.Length));
                }
            }

            if (result != null)
            {
                System.Diagnostics.Debug.WriteLine("  " + ElementName + ": successful match");
            }

            return result;
        }

    }

}
