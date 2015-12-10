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
using System.Threading;
using Gov.Hhs.Cdc.DataServices.Bo;

namespace Gov.Hhs.Cdc.TransactionLogProvider
{
    public class CsTransactionLogProvider
    {
        private static Queue<TransactionEntryObject> transactionQueue = new Queue<TransactionEntryObject>();
        private static object QueueLock = new object();
        private static Semaphore QueueSemaphore = new Semaphore(0, int.MaxValue);

        protected static IObjectContextFactory ObjectContextFactory { get; set; }
        public static void Inject(IObjectContextFactory objectContextFactory)
        {
            ObjectContextFactory = objectContextFactory;
        }

        public CsTransactionLogProvider()
        {
            ObjectContextFactory = new TransactionLogObjectContextFactory();
        }

        public void LogTransaction(TransactionEntryObject transaction)
        {
            LogTheTransaction(transaction);
        }

        private void LogTheTransaction(TransactionEntryObject transaction)
        {
            if (ObjectContextFactory == null)
            {
                ObjectContextFactory = new TransactionLogObjectContextFactory();
            }
            using (TransactionLogObjectContext context = (TransactionLogObjectContext)ObjectContextFactory.Create())
            {
                TransactionEntryCtl.Create(context, transaction).AddToDb();
                context.SaveChanges();
            }
        }

        public TransactionEntryObject GetTransactionEntry(int transactionId)
        {
            using (TransactionLogObjectContext context = (TransactionLogObjectContext)ObjectContextFactory.Create())
            {
                return TransactionEntryCtl.Get(context).Where(t => t.TransactionId == transactionId).FirstOrDefault();
            }
        }

    }
}
