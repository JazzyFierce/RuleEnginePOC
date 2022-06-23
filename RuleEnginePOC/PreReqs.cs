using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RuleEnginePOC
{
    public class PreReqs
    {
        //FIELDS
        public bool visionTestPassed;
        public bool driversEdPassed;
        public bool parentalApproval;
        public bool writtenTestPassed;
        public int hoursDrivenAffadavit;

        //CONSTRUCTORS
        public PreReqs(bool visionTestPassed, bool driversEdPassed, bool parentalApproval, bool writtenTestPassed, int hoursDrivenAffadavit)
        {
            this.visionTestPassed = visionTestPassed;
            this.driversEdPassed = driversEdPassed;
            this.parentalApproval = parentalApproval;
            this.writtenTestPassed = writtenTestPassed;
            this.hoursDrivenAffadavit = hoursDrivenAffadavit;
        }
    }
}
