using Newtonsoft.Json;
using RulesEngine.HelperFunctions;
using RulesEngine.Models;
using System.Dynamic;
using static RulesEngine.Extensions.ListofRuleResultTreeExtension;

namespace RuleEnginePOC
{
    public class BasicDemo
    {
        public async void Run()
        {
            //obtaining file with rules
            var files = Directory.GetFiles("Workflows");
            if (files == null || files.Length == 0)
            {
                throw new Exception("Rules not found.");
            }

            //converting json to workflow
            var fileData = File.ReadAllText(files[0]);
            var workflow = JsonConvert.DeserializeObject<List<Workflow>>(fileData);

            //rules engine setup and initialize
#pragma warning disable CS0436 // Type conflicts with imported type
            var reSettingsWithCustomTypes = new ReSettings { CustomTypes = new Type[] { typeof(Utils) } };
#pragma warning restore CS0436
            var re = new RulesEngine.RulesEngine(workflow?.ToArray(), null, reSettingsWithCustomTypes);

            Console.WriteLine("Summary\n-------------------------------");
            //driverinfo

            var driversInfo = Directory.GetFiles("Input");
            List<ExpandoObject> drivers = new List<ExpandoObject>();
            
            foreach (var driverInfo in driversInfo)
            {
                var driverData = File.ReadAllText(driverInfo);
                dynamic? driver = JsonConvert.DeserializeObject<ExpandoObject>(driverData);
                drivers.Add(driver);
            }

            foreach (dynamic driver in drivers)
            {
                dynamic input1 = driver.basicInfo;
                dynamic input2 = driver.preReqs;
                dynamic input3 = driver.previousLicenseMonthsHeld;
                dynamic input4 = driver.activeRestrictions;

                var rp1 = new RuleParameter("basicInfo", input1);
                var rp2 = new RuleParameter("preReqs", input2);
                var rp3 = new RuleParameter("previousLicenseMonthsHeld", input3);
                var rp4 = new RuleParameter("activeRestrictions", input4);

                var ruleParameters = new RuleParameter[]
                {
                    rp1,
                    rp2,
                    rp3,
                    rp4
                };

                var result = await re.ExecuteActionWorkflowAsync("LicenseRules", "BasicRequirementsMet", ruleParameters);
                string messageToWrite = $"Driver: {input1.name}\nResult: {result.Output}";

                if (result.Output is "No license available")
                {
                    messageToWrite += "\nDetails:";
                    var basicRequirementsTreeList = result.Results; //i.e. every rule that was executed before failure condition
                    var lastExecutedRule = basicRequirementsTreeList[0]; //Rule that will give relevant error messages
                    var lastExecutedRuleLeaves = lastExecutedRule.ChildResults; 
                    
                    if (lastExecutedRuleLeaves != null)
                    {
                        foreach (var childResult in lastExecutedRuleLeaves)
                        {
                            if (!childResult.IsSuccess)
                            {
                                messageToWrite += $"\nRule Name: {lastExecutedRule.Rule.RuleName} --- Message: {childResult.Rule.ErrorMessage}";
                            }
                        }
                    }                  
                }

                Console.WriteLine(messageToWrite +"\n");
            }
        }
    }
}

