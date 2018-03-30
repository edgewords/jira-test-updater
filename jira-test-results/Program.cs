using Atlassian.Jira;
using System;
using System.IO;
using System.Xml;

namespace jira_test_results
{
    class Program
    {
        static string url;
        static string username;
        static string password;
        static string xmlPath;

        static int Main(string[] args)
        {        
            // Only 2 args, then get the jira-config input file
            if (args.Length == 2)
            {
                string filePath = args[0];
                xmlPath = args[1];

                if (!File.Exists(filePath))
                {
                    Console.WriteLine(filePath + " File does not exits!");
                    return -1;
                }
                string[] readText = File.ReadAllLines(filePath);
                url = readText[0];
                username = readText[1];
                password = readText[2];
            }
            else
            {
                //If wrong number of args, let the user know:
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
                //otherwise, 4 args passed in, so set the values
                url = args[0];
                username = args[1];
                password = args[2];
                xmlPath = args[3];
            }

            XmlDocument xmlDoc = new XmlDocument();
            xmlDoc.Load(xmlPath);
            // get all the xml nodes for the Features
            XmlNodeList testSuitesName = xmlDoc.GetElementsByTagName("test-suite");

            for (int i = 0; i < testSuitesName.Count; i++)
            {
                //Features have an xml attribute of 'TestFixture'
                if (testSuitesName[i].Attributes["type"].Value == "TestFixture")
                {
                    string featureName = testSuitesName[i].Attributes["name"].Value.Replace("Feature", "").Replace("_", "-");

                    try
                    {
                        //set the Jira 'Test Status' field value
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
