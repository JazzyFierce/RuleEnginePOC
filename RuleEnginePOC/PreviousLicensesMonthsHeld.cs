using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RuleEnginePOC
{
    public class PreviousLicensesMonthsHeld
    {
        //FIELDS
        public int instructionPermit;
        public int restrictedPermit;

        //CONSTRUCTOR
        public PreviousLicensesMonthsHeld(int instructionPermit, int restrictedPermit)
        {
            this.instructionPermit = instructionPermit;
            this.restrictedPermit = restrictedPermit;
        }
    }
}
