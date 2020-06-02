using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SME.SR.Workers.SGP.Commons.Attributes
{
    [System.AttributeUsage(System.AttributeTargets.Class)]
    public class WorkerAttribute : System.Attribute
    {
        private string _workerQueue;

        public WorkerAttribute(string workerQueue)
        {
            this._workerQueue = workerQueue;
        }

        public string WorkerQueue {
            get
            {
                return this._workerQueue;
            }
        }
    }
}
