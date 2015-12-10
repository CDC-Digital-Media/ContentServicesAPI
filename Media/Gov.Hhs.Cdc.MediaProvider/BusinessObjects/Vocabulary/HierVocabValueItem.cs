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


namespace Gov.Hhs.Cdc.MediaProvider
{
    [FilterSelection(Code="IsInUse",CriterionType = FilterCriterionType.Boolean)]
    [FilterSelection(Code = "Language", CriterionType = FilterCriterionType.MultiSelect, ListKeyType = ListItem.KeyType.StringKey)]
    [FilterSelection(Code = "MediaType", CriterionType = FilterCriterionType.MultiSelect, ListKeyType = ListItem.KeyType.StringKey)]
    [FilterSelection(Code = "ValueSet", CriterionType = FilterCriterionType.MultiSelect, ListKeyType=ListItem.KeyType.IntKey)]
    [FilterSelection(Code = "ValueSetName", CriterionType = FilterCriterionType.MultiSelect, ListKeyType = ListItem.KeyType.StringKey)]
    [FilterSelection(Code = "Id", CriterionType = FilterCriterionType.SingleSelect, ListKeyType = ListItem.KeyType.IntKey)]

    [FilteredDataSet]
    public class HierVocabValueItem
    {
        public ValueKey ValueKey { get; set; }
        public string ValueName { get; set; }
        public string Description { get; set; }
        public int DisplayOrdinal { get; set; }
        [FilterSelection(CriterionType=FilterCriterionType.Boolean)]
        public bool IsActive { get; set; }

        public int DescendantMediaUsageCount { get; set; }
        public int MediaCount { get; set; } 
        public List<HierVocabValueItem> Children { get; set; }

        public HierVocabValueItem()
            : this(
            new ValueKey(), string.Empty, string.Empty, 
            default(int), false, default(int), new List<HierVocabValueItem>()) {}

        public HierVocabValueItem(VocabularyItemDtoBase dto)
            : this(
            dto.ValueKey, dto.ValueName, dto.Description, 
            dto.DisplayOrdinal, dto.IsActive, dto.MediaCount ?? 0, 
            new List<HierVocabValueItem>()) {}

        public HierVocabValueItem(
            ValueKey valueKey, string valueName, string description,
            int displayOrdinal, bool isActive, int mediaCount,
            List<HierVocabValueItem> children)
        {
            ValueKey = valueKey;
            ValueName = valueName;
            Description = description;
            DisplayOrdinal = displayOrdinal;
            IsActive = isActive;
            MediaCount = mediaCount;
            Children = children;
        }

        public override string ToString()
        {
            return ValueKey.ToString() + "(" + DescendantMediaUsageCount.ToString() + ")";
        }

        public string ToHierarchicalString(string pad)
        {
            string results = pad + ToString() + "(" + MediaCount.ToString() + ")";
            if( Children != null)
            {
                foreach (HierVocabValueItem item in Children)
                {
                    results = results + "\n" + pad + item.ToHierarchicalString(pad + "    ");
                }

            }
            return results;
        }

        public void AddChildren(HierVocabValueItem otherItem)
        {
            if (otherItem == null) { return; }
            if (otherItem.Children != null)
            {
                Children = Children == null ? otherItem.Children :
                            Children.Union(otherItem.Children).Distinct().ToList();
            }
            MediaCount = MediaCount + otherItem.MediaCount;
        }
        /// </summary>
        /// <param name="obj">Object to compare to the current Person</param>
        /// <returns>True if equal, false if not</returns>
        public override bool Equals(object obj)
        {

            // Try to cast the object to compare to to be a Person
            HierVocabValueItem value = obj as HierVocabValueItem;
            return IsEqualTo(value);
        }

        public override int GetHashCode()
        {
            return ValueKey.GetHashCode();
        }

        public bool Equals(HierVocabValueItem compareTo)
        {
            return IsEqualTo(compareTo);
        }

        private bool IsEqualTo(HierVocabValueItem compareTo)
        {
            if (compareTo == null) { return false; }
            return ValueKey.Equals(compareTo.ValueKey);
        }

        public static string ToHierarchicalString(IEnumerable<HierVocabValueItem> items)
        {
            List<string> result = new List<string>();
            foreach (HierVocabValueItem item in items)
            {
                result.Add(item.ToHierarchicalString(""));
            }
            return string.Join("\n", result.ToArray()); ;
        }
    }

}
