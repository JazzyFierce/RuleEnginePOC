using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RuleEnginePOC
{
    public class BasicInfo
    {
        //FIELDS
        public int age;
        public string email;
        public string county;

        //PROPERTIES
        public int Age
        {
            get { return age; }
            set { age = value; }
        }

        //CONSTRUCTORS
        public BasicInfo(int age, string email, string county)
        {
            Age = age;
            this.email = email;
            this.county = county;
        }
    }
}
