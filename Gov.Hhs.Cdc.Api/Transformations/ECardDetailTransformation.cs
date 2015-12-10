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

using Gov.Hhs.Cdc.MediaProvider;

namespace Gov.Hhs.Cdc.Api
{
    public static class ECardDetailTransformation
    {
        public static ECardDetail CreateNewECardObject(SerialECardDetail eCard)
        {
            if (eCard == null)
                return null;
            return new ECardDetail()
            {
                MobileCardName = eCard.mobileCardName,
                Html5Source = eCard.html5Source,
                Caption = eCard.caption,
                CardText = eCard.cardText,
                CardTextOutside = eCard.cardTextOutside,
                CardTextInside = eCard.cardTextInside,
                ImageSourceInsideLarge = eCard.imageSourceInsideLarge,
                ImageSourceInsideSmall = eCard.imageSourceInsideSmall,
                ImageSourceOutsideLarge = eCard.imageSourceOutsideLarge,
                ImageSourceOutsideSmall = eCard.imageSourceOutsideSmall,
                MobileTargetUrl = eCard.mobileTargetUrl,
                IsMobile = eCard.isMobile,
                DisplayOrdinal = eCard.displayOrdinal,
                IsActive = eCard.isActive
            };

        }

        public static SerialECardDetail GetSerialECardDetail(ECardDetail eCard)
        {
            if (eCard == null)
            {
                return new SerialECardDetail();
            }
            return new SerialECardDetail()
            {
                mobileCardName = eCard.MobileCardName,
                html5Source = eCard.Html5Source,
                caption = eCard.Caption,
                cardText = eCard.CardText,
                cardTextOutside = eCard.CardTextOutside,
                cardTextInside = eCard.CardTextInside,
                imageSourceInsideLarge = eCard.ImageSourceInsideLarge,
                imageSourceInsideSmall = eCard.ImageSourceInsideSmall,
                imageSourceOutsideLarge = eCard.ImageSourceOutsideLarge,
                imageSourceOutsideSmall = eCard.ImageSourceOutsideSmall,
                isMobile = eCard.IsMobile,
                mobileTargetUrl = eCard.MobileTargetUrl,
                displayOrdinal = eCard.DisplayOrdinal,
                isActive = eCard.IsActive
            };
        }

    }
}
