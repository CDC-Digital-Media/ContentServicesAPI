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

using System.Collections.Generic;
using System.Linq;
using Gov.Hhs.Cdc.Bo;
using Gov.Hhs.Cdc.MediaProvider;

namespace Gov.Hhs.Cdc.CdcMediaProvider
{
    public class HierVocabularyUtility
    {
        Dictionary<ValueKey, HierVocabValueItem> allValueItems = new Dictionary<ValueKey, HierVocabValueItem>();
        ChildDictionary children = new ChildDictionary();
        ChildDictionary use = new ChildDictionary();

        internal List<HierVocabValueItem> Build(IEnumerable<VocabularyItemHierarchicalDto> allValues, bool returnOnlyInUseItems)
        {
            List<HierVocabValueItem> topLevelItems = new List<HierVocabValueItem>();
            foreach (VocabularyItemHierarchicalDto dto in allValues)
            {
                HierVocabValueItem valueItem = new HierVocabValueItem(dto);
                if (allValueItems.ContainsKey(dto.ValueKey))
                {
                    valueItem = allValueItems[dto.ValueKey];    //Get Existing Value Item
                }
                else
                {
                    valueItem = new HierVocabValueItem(dto);
                    allValueItems.Add(dto.ValueKey, valueItem);
                }

                if (dto.Level == 1)
                {
                    topLevelItems.Add(valueItem);
                }
                if (dto.ParentKey != null)
                {
                    children.Add(dto.ParentKey, dto.ValueKey, valueItem);
                }
                if (dto.UsedForKey != null)
                {
                    use.Add(dto.UsedForKey, dto.ValueKey, valueItem);
                }
            }

            ReplaceUseItems(topLevelItems);
            topLevelItems = topLevelItems.Distinct().ToList();
            
            LinkParentsAndChildren();
            topLevelItems.Sum(c => GetMediaCount(c));

            if (returnOnlyInUseItems)
            {
                topLevelItems = FilterOnlyInUseItems(topLevelItems);
            }

            return topLevelItems;

        }

        private List<HierVocabValueItem> FilterOnlyInUseItems(List<HierVocabValueItem> items)
        {
            List<HierVocabValueItem> filterItems = items.Where(i => i.DescendantMediaUsageCount > 0).ToList();

            foreach (HierVocabValueItem item in items)
            {
                if (item.Children != null)
                {
                    item.Children = FilterOnlyInUseItems(item.Children);
                }
            }
            return filterItems;
        }



        private void ReplaceUseItems(List<HierVocabValueItem> topLevelItems)
        {
            //Example results set.  We return Dollar twice.  Once as 'Used For' Buck and once as 'Child of' Bill
            //Dollar will replace Buck in the hierarchy.  It will alsy be the child of Bill.  If buck is also the child of bill, we will need to 
            //remove duplicates
            //          Children, parentKey        usedForKey => Use, 
            //Bill      Dollar, Bill
            //GreenBack                             GreenBack => Buck,
            //Buck	                                Buck => Dollar,
            //Dollar	Silver Dollar, Dollar
            //Buck	    Silver Dollar, Buck


            //Use contains 'Dollar', with usedForKey of 'Buck'.  Find 'Buck and replace with 'Dollar'
            foreach (ValueKey trivialKey in use.Keys)
            {
                //We need to
                //  1) replace all Bucks with Dollars
                //  2) Add the Bucks media counts to Dollars
                HierVocabValueItem priorityItem = use[trivialKey][0];

                if (allValueItems.ContainsKey(trivialKey))
                {
                    HierVocabValueItem trivialItem = allValueItems[trivialKey];
                    priorityItem.AddChildren(trivialItem);

                    //trivialItem and trivialItemKey now point to item that don't exist.

                    //Any 'Use' item where we use "Buck", change it to "Dollar" instead
                    children.ReplaceInvalidItem(trivialKey, priorityItem);
                    use.ReplaceInvalidItem(trivialKey, priorityItem);

                    ChildDictionary.ReplaceTrivialItemWithPrority(topLevelItems, trivialKey, priorityItem);

                    //Replace any trivial children that should not point to the priority item instead

                    //Find all usages of dontUseItem and replace with priorityItem
                    //Scenario:
                    //We dontUseItem = 'Buck' and priorityItem = 'Dollar'
                    //Two Scenarios:
                    //1) Greenback has already been replaced with Buck
                    //2) Greenback will be replaced with Buck Later


                    //children.Replace
                    ////use[trivialKey]


                    //allValueItems[trivialKey].Use(use[trivialKey][0]);  
                }
            }
        }

        private void LinkParentsAndChildren()
        {
            foreach (ValueKey parentKey in children.Keys)
            {
                if (allValueItems.ContainsKey(parentKey))
                {
                    //Filter out any duplicate children, and select the ones that have the highest MediaCount.  i.e., if 
                    //Buck had 4 usages and Dollar had 12 usages, then Buck will now have 16 usages and be called Dollar.
                    //We will need to select that one if there are dups

                    //Note... When searching for Dollar, we will find all 'Bucks' as well as 'Dollar'.  So we really need to 
                    //just sum the counts for bucks and dollars, and put in both
                    allValueItems[parentKey].Children =
                        (from c in children[parentKey]
                         group c by c.ValueKey into g
                         let s = g.OrderByDescending(c2 => c2.MediaCount).First()    //Don't really need the OrderBy because we should have identical items
                         select s).ToList();
                    //children[parentKey].GroupBy(i => i.ValueKey).Select(y => y.Max(i => i.MediaCount)).ToList();
                }

            }
        }

        private static int GetMediaCount(HierVocabValueItem valueItem)
        {
            valueItem.DescendantMediaUsageCount = valueItem.MediaCount + (valueItem.Children == null ? 0 :
                valueItem.Children.Sum(c => GetMediaCount(c)));
            return valueItem.DescendantMediaUsageCount;
        }
    }

    public class ChildDictionary : ListDictionary<ValueKey, HierVocabValueItem>
    {
        ListDictionary<ValueKey, ValueKey> parentsThatHaveAChildOf;
        public ChildDictionary()
            : base()
        {
            parentsThatHaveAChildOf = new ListDictionary<ValueKey, ValueKey>();
        }

        public void Add(ValueKey parentKey, ValueKey childKey, HierVocabValueItem childItem)
        {
            this.Add(parentKey, childItem);
            parentsThatHaveAChildOf.Add(childKey, parentKey);
        }

        public void ReplaceInvalidItem(ValueKey trivialKey, HierVocabValueItem priorityItem)
        {
            if (!parentsThatHaveAChildOf.ContainsKey(trivialKey))
            {
                return;
            }

            List<ValueKey> allParentsOfATrivialItem = parentsThatHaveAChildOf[trivialKey];
            foreach (ValueKey parentKeyOfATrivialItem in allParentsOfATrivialItem)
            {
                //Use used to point to dontUseItem
                if (this.ContainsKey(parentKeyOfATrivialItem))
                {
                    ReplaceTrivialItemWithPrority(this[parentKeyOfATrivialItem], trivialKey, priorityItem);
                }
            }
        }

        public static void ReplaceTrivialItemWithPrority(List<HierVocabValueItem> items, ValueKey trivialKey, HierVocabValueItem priorityItem)
        {
            for (int i = 0; i < items.Count; ++i)
            {
                if (items[i].ValueKey.Equals(trivialKey))
                {
                    items[i] = priorityItem;
                }
            }
        }
    }

}
