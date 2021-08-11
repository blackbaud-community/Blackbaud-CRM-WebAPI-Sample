using Blackbaud.AppFx.WebAPI.ServiceProxy;
using Blackbaud.AppFx.XmlTypes.DataForms;
using System;
using System.Net;

namespace WebAPIDemo
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.Write("Enter your search critera: ");

            // Collect search criteria
            string input = Console.ReadLine();
            string firstName = string.Empty;
            string lastName = string.Empty;

            // Process input
            if (string.IsNullOrEmpty(input))
            {
                Console.WriteLine("No input was received. Press the 'Enter' key to exit.");
                Console.Read();
                Environment.Exit(0);
            }
            else
            {
                string[] words = input.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries);

                lastName = words[0].Trim();

                if (words.Length > 1)
                {
                    firstName = words[1].Trim();
                }
            }

            // Find results
            ListOutputRow[] results = Search(lastName, firstName);

            // Display results
            Console.WriteLine("Results:");
            if (results.Length == 0)
            {
                Console.WriteLine("There were no search results.");
            }
            else
            {
                Console.WriteLine("Lookup ID\tName");
                foreach (ListOutputRow row in results)
                {
                    Console.WriteLine(row.Values[1].ToString() + "\t" + row.Values[2].ToString() + "\t");
                }
            }

            // Pause
            Console.WriteLine("Press the 'Enter' key to exit.");
            Console.Read();
        }

        private static ListOutputRow[] Search(string lastName, string firstName)
        {
            // Set credentials
            NetworkCredential credentials = new NetworkCredential
            {
                Domain = "<domain>",
                UserName = "<username>",
                Password = "<password>"
            };
            
            // Initialize web service
            AppFxWebService service = new AppFxWebService
            {
                Url = "http://<web_server>/<virtual_directory>/AppFxWebService.asmx",
                Credentials = credentials
            };

            // Define filters for the search list
            DataFormFieldValueSet fieldValueSet = new DataFormFieldValueSet()
            {
                new DataFormFieldValue("KEYNAME", lastName),
                new DataFormFieldValue("FIRSTNAME", firstName)
            };

            // Create request
            SearchListLoadRequest request = new SearchListLoadRequest
            {
                SearchListID = new Guid("fdf9d631-5277-4300-80b3-fdf5fb8850ec"), // ConstituentByNameOrLookupId.Search.xml
                ClientAppInfo = new ClientAppInfoHeader
                {
                    ClientAppName = "Sample Application",
                    REDatabaseToUse = "<database>"
                },
                Filter = new DataFormItem
                {
                    Values = fieldValueSet
                }
            };

            // Run search
            SearchListLoadReply reply = service.SearchListLoad(request);
            if (reply != null && reply.Output != null && reply.Output.Rows != null)
            {
                return reply.Output.Rows;
            }
            else
            {
                return null;
            }
        }
    }
}
