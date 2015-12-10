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

using System;
using System.Collections.Generic;

namespace Gov.Hhs.Cdc.CdcMediaValidationProvider
{

    /// <summary>
    /// Stack used for tracking xml:base paths while parsing XML Catalogs.
    /// </summary>
    public class XmlBaseStack
    {

        /// <summary>
        /// Private stack used for storing URIs.
        /// </summary>
        private Stack<Uri> UriStack = new Stack<Uri>();

        /// <summary>
        /// Returns the current cumulative "xml:base" value.
        /// </summary>
        /// <returns>The current xml:base URI</returns>
        public Uri GetXmlBase()
        {
            if (UriStack.Count >= 1)
            {
                return UriStack.Peek();
            }
            else
            {
                return null;
            }
        }

        /// <summary>
        /// Pushes an xml:base URI onto the top of the stack.
        /// </summary>
        /// <param name="baseUri">An absolute or relative URI</param>
        public void Push(String baseUri)
        {
            System.Diagnostics.Debug.WriteLine("Push: baseUri = " + baseUri);

            Uri currentXmlBase = GetXmlBase();
            Uri absoluteUri = null;
            if (currentXmlBase == null)
            {
                absoluteUri = new Uri(baseUri);
            }
            else
            {
                absoluteUri = new Uri(currentXmlBase, baseUri);
            }

            // Relative path resolution only works correctly if all xml:base URIs end in "/".
            String absoluteUriString = absoluteUri.ToString();
            if (!absoluteUriString.EndsWith("/"))
            {
                absoluteUri = new Uri(absoluteUriString + "/");
            }

            System.Diagnostics.Debug.WriteLine("Push: absoluteUri = " + absoluteUri);

            UriStack.Push(absoluteUri);
        }

        /// <summary>
        /// Pushes an xml:base URI onto the top of the stack.
        /// </summary>
        /// <param name="baseUri">An absolute URI</param>
        public void Push(Uri baseUri)
        {
            if (baseUri == null)
            {
                UriStack.Push(null);
            }
            else
            {
                Push(baseUri.ToString());
            }
        }

        /// <summary>
        /// Pops (removes) the xml:base URI at the top of the stack and returns it.
        /// </summary>
        /// <returns>The xml:base URI from the top of the stack</returns>
        public Uri Pop()
        {
            return UriStack.Pop();
        }

        /// <summary>
        /// Returns the xml:base URI from the top of the stack, without removing it.
        /// </summary>
        /// <returns>The xml:base URI from the top of the stack</returns>
        public Uri Peek()
        {
            return UriStack.Peek();
        }

    }
}
