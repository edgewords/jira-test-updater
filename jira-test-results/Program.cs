using Atlassian.Jira;
using System;
using System.Xml;

namespace jira_test_results
{
    class Program
    {
        static int Main(string[] args)
        {

            if (args.Length < 4)
            {
                Console.WriteLine("Incorrect number of parameters, parameters are:");
                Console.WriteLine("- URL - JIRA URL e.g. https://mycompany.atlassian.net");
                Console.WriteLine("- Username - JIRA email e.g. me@mycomapny.com");
                Console.WriteLine("- JIRA Password");
                Console.WriteLine(@"- Path to xml results - e.g. c:\my\path\to\bi\TestResult.xml");
                Console.WriteLine();
                Console.WriteLine("Example:");
                Console.WriteLine("jira-test-results https://mycompany.atlassian.net me@company.com mypassword c:\\TestResult.xml");
                return -1;
            }

            string url = args[0];
            string username = args[1];
            string password = args[2];
            string xmlPath = args[3];

            XmlDocument xmlDoc = new XmlDocument();
            xmlDoc.Load(xmlPath);

            XmlNodeList testSuitesName = xmlDoc.GetElementsByTagName("test-suite");

            for (int i = 0; i < testSuitesName.Count; i++)
            {
                if (testSuitesName[i].Attributes["type"].Value == "TestFixture")
                {
                    string featureName = testSuitesName[i].Attributes["name"].Value.Replace("Feature", "").Replace("_", "-");

                    try
                    {
                        var jira = Jira.CreateRestClient(url, username, password);
                        var issue = jira.Issues.GetIssueAsync(featureName).Result;
                        issue["Test Status"] = testSuitesName[i].Attributes["result"].Value;
                        issue.SaveChanges();
                        Console.WriteLine(featureName + " Issue Test Status Updated to: " + testSuitesName[i].Attributes["result"].Value);
                    }
                    catch (Exception e)
                    {
                        Console.WriteLine(e.Message + " Error with issue: " + featureName);
                        Console.WriteLine();
                    }
                }
            }
            //Console.ReadLine();
            return 0;
        }
    }
}
