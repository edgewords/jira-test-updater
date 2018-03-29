using Atlassian.Jira;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace jira_test_results
{
    class Program
    {
        static int Main(string[] args)
        {

            if (args.Length < 4)
            {
                Console.WriteLine("Incorrect number of parameters");
                return -1;
            }

            string url = args[0];
            string username = args[1];
            string password = args[2];
            string xmlPath = args[3];

            Console.WriteLine("Looking through xml file");
            Console.WriteLine();

            XmlDocument xmlDoc = new XmlDocument();
            xmlDoc.Load(xmlPath);

            XmlNodeList testSuitesName = xmlDoc.GetElementsByTagName("test-suite");

            for (int i = 0; i < testSuitesName.Count; i++)
            {
                if (testSuitesName[i].Attributes["type"].Value == "TestFixture")
                {
                    string featureName = testSuitesName[i].Attributes["name"].Value.Replace("Feature", "").Replace("_", "-");
                    Console.WriteLine(featureName);
                    Console.WriteLine("Tests Passed: " + testSuitesName[i].Attributes["passed"].Value);
                    Console.WriteLine("Tests Failed: " + testSuitesName[i].Attributes["failed"].Value);
                    Console.WriteLine("Overall Test Result: " + testSuitesName[i].Attributes["result"].Value);
                    Console.WriteLine();

                    try
                    {
                        var jira = Jira.CreateRestClient(url, username, password);
                        var issue = jira.Issues.GetIssueAsync(featureName).Result;
                        issue["Test Status"] = testSuitesName[i].Attributes["result"].Value;
                        issue.SaveChanges();
                    }
                    catch (Exception e)
                    {
                        Console.WriteLine(e.Message + " Error with issue: " + featureName);
                        Console.WriteLine();
                    }
                }
            }
            Console.ReadLine();
            return 0;
        }
    }
}
