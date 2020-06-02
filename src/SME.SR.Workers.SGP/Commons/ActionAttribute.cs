using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SME.SR.Workers.SGP.Commons
{
    [System.AttributeUsage(System.AttributeTargets.Method)]
    public class ActionAttribute : System.Attribute
    {
        private string _name;

        public ActionAttribute(string name)
        {
            this._name = name;
        }

        public string Name
        {
            get
            {
                return this._name;
            }
        }
    }
}

