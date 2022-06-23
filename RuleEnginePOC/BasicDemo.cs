using Newtonsoft.Json;
using RulesEngine.HelperFunctions;
using RulesEngine.Models;
using System;
using System.Collections.Generic;
using System.Dynamic;
using System.IO;
using static RulesEngine.Extensions.ListofRuleResultTreeExtension;

namespace RuleEnginePOC
{
    public class BasicDemo
    {
        public void Run()
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
            var bre = new RulesEngine.RulesEngine(workflow?.ToArray(), null, reSettingsWithCustomTypes);

            Console.WriteLine($"Running BasicDemo.....");
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

                //Basic tests
                List<RuleResultTree> resultList = bre.ExecuteAllRulesAsync("LicenseRules", rp1, rp2, rp3, rp4).Result;
                string licenseGiven = "No licenses available";

                //the first rule satisfied (if any) assigns a new value to "license given"
                resultList.OnSuccess((eventName) => {
                    licenseGiven = $"{eventName}";
                });

                //if no rules are satisfied
                resultList.OnFail(() => {
                    licenseGiven = "The user is not eligible for any license.";
                });

                Console.WriteLine("\n" + licenseGiven);

            }

            

            //check each rule
            /*foreach (var result in resultList)
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
            }*/



            //check a specific rule
            /*foreach (var result in resultList)
            {
                if (result.Rule.RuleName == "Instruction Permit Ages 14, 15, 16")
                {
                    Console.Write($"{result.Rule.RuleName}: {result.IsSuccess}");

                    IEnumerable<RuleResultTree> childResults = result.ChildResults;
                    if (childResults != null && !result.IsSuccess)
                    {
                        foreach (var childResult in childResults)
                        {
                            Console.WriteLine($"{childResult.Rule.RuleName}: {childResult.IsSuccess}");
                        }
                    }
                }
            }*/
        }
    }
}

