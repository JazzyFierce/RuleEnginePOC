using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RuleEnginePOC
{
    public class ActiveRestrictions
    {
        //FIELDS
        public bool DUI;
        public bool unpaidFines;
        public bool insuranceMissing;

        //Constructor
        public ActiveRestrictions(bool DUI, bool unpaidFines, bool insuranceMissing)
        {
            this.DUI = DUI;
            this.unpaidFines = unpaidFines;
            this.insuranceMissing = insuranceMissing;
        }
    }
}
