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

using Gov.Hhs.Cdc.Bo;

namespace Gov.Hhs.Cdc.MediaValidatonProvider
{
    public class CollectedData
    {
        private bool _collectionIsValid;
        public bool CollectionIsValid { set { _collectionIsValid = value; } }
        public bool IsCollectionValid { get { return _collectionIsValid && Messages.NumberOfErrors == 0; } }

        public bool IsAvailable { get; set; }
        public bool IsLoadable { get; set; }

        public string Data { get; set; }
        public string MimeType { get; set; }

        private ValidationMessages _messages = null;
        public ValidationMessages Messages
        {
            get
            {
                if (_messages == null)
                {
                    _messages = new ValidationMessages();
                }
                return _messages;
            }
        }

        public CollectedData()
        {
            CollectionIsValid = true;
            IsAvailable = false;
            IsLoadable = false;
            Data = "";
        }



    }
}
