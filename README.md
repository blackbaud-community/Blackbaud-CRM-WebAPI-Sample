# Blackbaud-CRM-WebAPI-Sample

Today, as it has been, one of the core tenets of the Infinity platform is that it should be “web delivered.” Ben Lambert details this in his [post](https://www.bbdevnetwork.com/blogs/accessing-infinitybbec-php/), in which he outlines an example of accessing CRM via a simple PHP application. We recommend reading it if you haven’t already. Today, we’re happy to show that the same result can be achieved via .NET!

Built in to all Infinity-based applications is a web service API. We’ve taken all our web service endpoints and enabled you to call in to them via any programming language that can communicate with an IIS web server. Behind those endpoints, you have access to every data form, data list, record operation, query view, search list, etc. that Blackbaud has shipped, plus any that you write and load in to your installation. By going through these endpoints, even from an application that is separate from CRM, you also tie in to all the security features present when running the same code through the web shell interface. This is because our security rules are enforced at the web service level, eliminating the existence of a “back door” that could be used by third party code to access features and records that have not been granted to it.

In this post, we’ll look at a sample C# console application that accepts user input and uses it to search for constituents in CRM. Specifically, we use the **Constituent Search by Name or Lookup ID** search list spec to retrieve and display certain data within the search results. As with Ben’s example, you will need the following to call the web service for an Infinity-based application:

1. You must be an authenticated user in that application.
2. You must be granted security permission to the feature(s) being used.
3. If record-level security is enabled, only records for which you have permission to access will display in the results.

Once those are in place, you will need the following information to plug in to the code:

1. The URL of the CRM web service.
2. The name of the database to use.
3. Credentials for a user that meets the above security criteria.

The code below is used by the console application to construct a `SearchListLoadRequest`, send it to CRM, and return the results. The full code sample can be found here, in the Blackbaud GitHub Code Repository. To use it, you will need to ensure the `Blackbaud.AppFx.WebAPI` and `Blackbaud.AppFx.XmlTypes` assemblies are imported for use. You will also need to fill in the relevant information, outlined above, for the request to succeed.

``` csharp
private static ListOutputRow[] Search(string lastName, string firstName)
{
    // Initialize web service
    AppFxWebService service = new AppFxWebService
    {
        Url = "http://<web_server>/<virtual_directory>/AppFxWebService.asmx"
    };

    // Set credentials
    NetworkCredential credentials = new NetworkCredential
    {
        Domain = "<domain>",
        UserName = "<username>",
        Password = "<password>"
    };
    service.Credentials = credentials;

    // Create request
    SearchListLoadRequest request = new SearchListLoadRequest
    {
        SearchListID = new Guid("fdf9d631-5277-4300-80b3-fdf5fb8850ec"), // ConstituentByNameOrLookupId.Search.xml
        ClientAppInfo = new ClientAppInfoHeader
        {
            ClientAppName = "Sample Application",
            REDatabaseToUse = "<database>"
        }
    };

    // Define filters for the search list
    DataFormFieldValueSet fieldValueSet = new DataFormFieldValueSet()
    {
        new DataFormFieldValue("KEYNAME", lastName),
        new DataFormFieldValue("FIRSTNAME", firstName)
    };

    DataFormItem filter = new DataFormItem
    {
        Values = fieldValueSet
    };

    request.Filter = filter;

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
```

There are several important components here. First, we need an instance of the web service. While it’s possible to construct a service reference in Visual Studio and build a web service proxy based on that, it’s not necessary. We have done that for you! All that needs to be done is to create a new instance of the service by providing the correct web service URL and some valid user credentials. Then, a request object is needed. In this example, a `SearchListLoadRequest` is used. This can be substituted with a `DataFormLoadRequest`, `DataListLoadRequest`, or any other web server request object depending upon your individual needs. The code then fills out this request with information specific to that request’s type as well as some client application information (`ClientAppInfo`). Finally, the service is contacted using the appropriate service method and the request is provided. The web server processes it and returns the results, which we can then process and display for the user.

We invite you to download the full code so you can experiment with it!