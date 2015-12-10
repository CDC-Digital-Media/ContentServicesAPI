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

using Gov.Hhs.Cdc.CdcMediaProvider.Dal;
using Gov.Hhs.Cdc.DataServices.Bo;
using Gov.Hhs.Cdc.MediaProvider;
using Gov.Hhs.Cdc.MediaValidation.Dal;

namespace Gov.Hhs.Cdc.CdcMediaProvider
{
    public class CsMediaProviderIocContainer : IMediaProviderIocContainer
    {
        private static IValueProvider _valueProvider;
        public IValueProvider ValueProvider { get { return _valueProvider; } }

        private static IMediaProvider _mediaProvider;
        public IMediaProvider MediaProvider { get { return _mediaProvider; } }

        public CsMediaProviderIocContainer()
        {
            _valueProvider = new CsValueProvider();
            _mediaProvider = new CsMediaProvider();
        }

        public static void Inject()
        {
            IObjectContextFactory mediaObjectContextFactory = new MediaObjectContextFactory();
            IObjectContextFactory mediaValidationObjectContextFactory = new MediaValidationObjectContextFactory();

            CsMediaSearchProvider.Inject(mediaObjectContextFactory);
        }
    }
}
