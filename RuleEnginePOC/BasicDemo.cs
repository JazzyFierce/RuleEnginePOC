using Newtonsoft.Json;
using RulesEngine.HelperFunctions;
using RulesEngine.Models;
using System;
using System.Collections.Generic;
using System.Dynamic;
using System.IO;
using System.Reflection;
using static RulesEngine.Extensions.ListofRuleResultTreeExtension;

namespace RuleEnginePOC
{
    public class BasicDemo
    {
        public void Run()
        {
            //obtaining file with rules
            var files = Directory.GetFiles("C:/Users/Izzy.Loney/source/repos/RuleEnginePOC/RuleEnginePOC/Workflows");
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
            var bre = new RulesEngine.RulesEngine(workflow?.ToArray(), null, reSettingsWithCustomTypes);

            Console.WriteLine($"Running BasicDemo.....");
            //driverinfo
            dynamic input1 = new BasicInfo(6, "abc", "canada");
            dynamic input2 = new PreReqs(true, true, true, true, 90);
            dynamic input3 = new PreviousLicensesMonthsHeld(12, 4);
            dynamic input4 = new ActiveRestrictions(false, false, false);

            var rp1 = new RuleParameter("basicInfo",input1);
            var rp2 = new RuleParameter("preReqs", input2);
            var rp3 = new RuleParameter("previousLicenseMonthsHeld", input3);
            var rp4 = new RuleParameter("activeRestrictions", input4);

            //Basic tests
            List<RuleResultTree> resultList = bre.ExecuteAllRulesAsync("LicenseRules", rp1, rp2, rp3, rp4).Result;
            string licenseGiven = "No licenses available";

            //check each rule
            foreach (var result in resultList)
            {
                Console.WriteLine($"\nRule - {result.Rule.RuleName} - {result.IsSuccess}");
                if (!result.IsSuccess)
                {
                    Console.WriteLine($"Details: {result.ExceptionMessage}");
                }

                IEnumerable<RuleResultTree> childResults = result.ChildResults;
                if (childResults != null)
                {
                    foreach (var childResult in childResults)
                    {
                        //childResult.ExceptionMessage results in empty string
                        Console.WriteLine($"{childResult.Rule.RuleName}: {childResult.IsSuccess}");
                    }
                }
            }

            //the first rule satisfied (if any) assigns a new value to "license given"
            resultList.OnSuccess((eventName) => {
                licenseGiven = $"{eventName}";
            });

            //if no rules are satisfied
            resultList.OnFail(() => {
                licenseGiven = "The user is not eligible for any license.";
            });

            Console.WriteLine("\n" + licenseGiven);

            //check a specific rule
            foreach (var result in resultList)
            {
                if (result.Rule.RuleName == "Instruction Permit Ages 14, 15, 16")
                {
                    Console.Write($"{result.Rule.RuleName}: {result.IsSuccess}");

                    IEnumerable<RuleResultTree> childResults = result.ChildResults;
                    if (childResults != null && !result.IsSuccess)
                    {
                        foreach (var childResult in childResults)
                        {
                            Console.WriteLine($"{childResult.Rule.RuleName}: {childResult.IsSuccess}: {childResult.ExceptionMessage}");
                        }
                    }
                }
            }
        }
    }
}