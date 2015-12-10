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
using System.Linq;
using Gov.Hhs.Cdc.Bo;
using Gov.Hhs.Cdc.CdcMediaProvider.Dal;
using Gov.Hhs.Cdc.DataServices;
using Gov.Hhs.Cdc.MediaProvider;

namespace Gov.Hhs.Cdc.CdcMediaProvider
{
    public partial class SyndicationListUpdateMgr : BaseUpdateMgr<SyndicationListObject>
    {
        private class CombinedSyndicationListObject
        {
            public SyndicationListObject SyndicationList { get; set; }
 
            public List<SyndicationListMediaObject> SyndicationListMediaObjects { get; set; }
            public SyndicationListMediaUpdateMgr SyndicationListMediaUpdateMgr { get; set; }
        }

        List<CombinedSyndicationListObject> theObjects; 
        List<SyndicationListMediaUpdateMgr> syndicationListUpdateMgrs;

        public override string ObjectName { get { return "SyndicationList"; } }

        public SyndicationListUpdateMgr(SyndicationListObject item)
        {
            Items = ValidatorHelper.GetWithValidationKey(item, ObjectName);
            Validator = new SyndicationListObjectValidator();
            CreateChildUpdateMgrs(Items);
        }

        private void CreateChildUpdateMgrs(IList<SyndicationListObject> items)
        {
            theObjects = (from u in items
                          select new CombinedSyndicationListObject()
                          {
                              SyndicationList = u,
                              SyndicationListMediaObjects = u.Medias.ToList(),
                              SyndicationListMediaUpdateMgr = new SyndicationListMediaUpdateMgr(u.Medias.ToList())
                          }).ToList();
            syndicationListUpdateMgrs = theObjects.Select(u => u.SyndicationListMediaUpdateMgr).ToList();
        }

        public override void PreSaveValidate(ref ValidationMessages validationMessages)
        {
            Validator.PreSaveValidate(ref validationMessages, Items);
            foreach (SyndicationListMediaUpdateMgr syndicationListUpdateMgr in syndicationListUpdateMgrs)
            {
                syndicationListUpdateMgr.PreSaveValidate(ref validationMessages);
            }
        }

        public override void PreDeleteValidate(ValidationMessages validationMessages)
        {
            Validator.PreDeleteValidate(validationMessages, Items);
            foreach (SyndicationListMediaUpdateMgr syndicationListUpdateMgr in syndicationListUpdateMgrs)
            {
                syndicationListUpdateMgr.PreDeleteValidate(validationMessages);
            }
        }

        public override void ValidateSave(IDataServicesObjectContext objectContext, ValidationMessages validationMessages)
        {
            Validator.ValidateSave(objectContext, validationMessages, Items);
            foreach (SyndicationListMediaUpdateMgr syndicationListUpdateMgr in syndicationListUpdateMgrs)
            {
                syndicationListUpdateMgr.ValidateSave(objectContext, validationMessages);
            }
        }

        public override void ValidateDelete(IDataServicesObjectContext objectContext, ValidationMessages validationMessages)
        {
            Validator.ValidateDelete(objectContext, validationMessages, Items);
            foreach (SyndicationListMediaUpdateMgr syndicationListUpdateMgr in syndicationListUpdateMgrs)
            {
                syndicationListUpdateMgr.ValidateDelete(objectContext, validationMessages);
            }
        }

        public override void Save(IDataServicesObjectContext objectContext, ValidationMessages validationMessages)
        {
            foreach (CombinedSyndicationListObject combinedSyndicationListObject in theObjects)
            {
                SyndicationListObject persistedSyndicationList =
                    SyndicationListCtl.Get((MediaObjectContext)objectContext, forUpdate: true)
                    .Where(u => u.SyndicationListGuid == combinedSyndicationListObject.SyndicationList.SyndicationListGuid).FirstOrDefault();

                if (persistedSyndicationList == null)
                    Insert(objectContext, combinedSyndicationListObject.SyndicationList);
                else
                    Update(objectContext, persistedSyndicationList, combinedSyndicationListObject.SyndicationList);
            }
        }

        private static void Update(IDataServicesObjectContext objectContext, SyndicationListObject persistedSyndicationList, SyndicationListObject syndicationList)
        {
            SyndicationListCtl syndicationListCtl = SyndicationListCtl.Update(objectContext, persistedSyndicationList, syndicationList, enforceConcurrency: true);

            List<SyndicationListMediaObject> newMedias = (from n in syndicationList.Medias
                                                          join p in persistedSyndicationList.Medias on n.MediaId equals p.MediaId
                                                          into missing
                                                          from p in missing.DefaultIfEmpty()
                                                          where missing == null || !missing.Any()
                                                          select n).ToList();

            AddSyndicationListMedias(objectContext, syndicationListCtl, newMedias);

            var updatedMedias = (from p in persistedSyndicationList.Medias
                                 join n in syndicationList.Medias on p.MediaId equals n.MediaId
                                 into newMedia
                                 from n in newMedia.DefaultIfEmpty()
                                 select new { Persisted = p, New = n }).ToList();

            foreach (var set in updatedMedias)
            {
                if (set.New == null || !set.New.IsActive)
                    SyndicationListMediaCtl.Delete(objectContext, set.Persisted);
                else
                    SyndicationListMediaCtl.Update(objectContext, set.Persisted, set.New, enforceConcurrency: false);
            }
        }

        private void Insert(IDataServicesObjectContext objectContext, SyndicationListObject syndicationList)
        {
            SyndicationListCtl syndicationListCtl = SyndicationListCtl.Create(objectContext, syndicationList);
            syndicationListCtl.AddToDb();
            AddSyndicationListMedias(objectContext, syndicationListCtl, syndicationList.Medias);
            InsertedDataControls.Add(syndicationListCtl);
        }

        private static void AddSyndicationListMedias(IDataServicesObjectContext objectContext, SyndicationListCtl syndicationListCtl, IEnumerable<SyndicationListMediaObject> newMedias)
        {
            foreach (SyndicationListMediaObject syndicationListMedia in newMedias)
            {
                syndicationListCtl.Add(SyndicationListMediaCtl.Create(objectContext, syndicationListMedia));
            }
        }

        public static ValidationMessages Delete(IDataServicesObjectContext objectContext, params Guid[] syndicationListGuids)
        {
            ValidationMessages validationMessages = new ValidationMessages();
            foreach (Guid syndicationListGuid in syndicationListGuids)
            {
                SyndicationListObject persistedSyndicationList =
                    SyndicationListCtl.Get((MediaObjectContext)objectContext, forUpdate: true)
                    .Where(u => u.SyndicationListGuid == syndicationListGuid).FirstOrDefault();

                if (persistedSyndicationList != null)
                {
                    foreach (SyndicationListMediaObject syndicationListMedia in persistedSyndicationList.Medias)
                    {
                        SyndicationListMediaCtl.Delete(objectContext, syndicationListMedia);
                    }
                    SyndicationListCtl.Delete(objectContext, persistedSyndicationList);
                }
            }
            objectContext.SaveChanges();
            return validationMessages;
        }

    }

}
