# BaseCampApi

This is a C# wrapper to (most of) the [Basecamp 4 API](https://github.com/basecamp/bc3-api).

It is provided with a Visual Studio 2019 build solution for .NET Standard, so can be used with any version of .NET.

There is a test project (for net core only) which also demonstrates usage.
-----------------------------------
# EDIT BY Craig Wright @SOELexicon
This fork was made as the original wouldnt work with my setup, this is a non async version with updated code for use with ASP.Net webforms.

Below is an example of implementation
might need to add the following using to your class.
```cs

using BaseCampApi;

```

Declare the following in your webform or class
```cs

static Settings _settings;
static Api _api;
string code;

public static Api Api
{
    get
    {
        if (_api == null)
        {
            _api = new Api(Settings);
        }
        return _api;
    }
}

public static bool HasValue(object o)
{
    if (o == null)
    { return false; }
    if (o is DateTime)
    {
        if (string.Format("{0:dd/MM/yyyy}", (DateTime)o) == "01/01/0001")
        {
            return false;
        }
    }
    if (o == System.DBNull.Value)
    { return false; }
    if (o is String)
    {
        if (((String)o).Trim() == String.Empty)
        {
            return false;
        }
    }

    return true;
}

public static Settings Settings
{
    get
    {
        if (_settings == null)
        {

            _settings = new Settings();
            string json = "{ " +
                " \"ApplicationName\": \"BaseCampAPI For ASP.Net\"," +
                "\"Contact\": \"Application Contact Goes Here\","+
                "\"RedirectUri\": \"http://localhost/basecamp\"," +
                "\"RedirectAfterLogin\": \"http://localhost/basecamp\"," +
                "\"PageToSendAfterLogin\": \"http://localhost/basecamp\"," +
                "\"ClientId\": \"CLIENTSIDGOESHERE\"," +
                "\"ClientSecret\": \"CLIENTSECRETSHHHH\", " +
                "\"AccessToken\": \"\"," +//this is got on redirect from oauth
                "\"RefreshToken\": \"\"," +
                "\"LogRequest\": 0,  " +
                "\"LogResult\": 0, " +
                "\"CompanyId\":0}";



            _settings.LoadJson(json);
            List<string> errors = _settings.Validate();
            if (errors.Count > 0)
                throw new ApplicationException(string.Join("\r\n", errors));
        }
        return _settings;
    }
}
```

 for me as the Syntax Project was already taken i had to add the following using to the top of the cs for my page
```cs
using Proj = BaseCampApi.Project;
```
then on Page_Init or load do something like

```cs
if (!HasValue(Session["AccessToken"]))
if (HasValue(Request["code"]))
{

    ViewState["code"] = Request["code"].ToString();
    code = Request["code"].ToString();
    Api.Settings.Code = code;
}
else
{
    string uri = BaseCampApi.Api.AddGetParams(BaseCampApi.Api.AuthUri, new
    {
        type = "web_server",    // Or user_agent
        client_id = HttpUtility.UrlEncode( Settings.ClientId),
        redirect_uri = Settings.RedirectUri.ToString(),
        state = HttpUtility.UrlEncode(Guid.NewGuid().ToString()),
    });
    Response.Redirect(uri,true);
}
else
{
    Api.Settings.RefreshToken = (string)Session["RefreshToken"];
    Api.Settings.AccessToken = (string)Session["AccessToken"];
    Api.Settings.TokenExpires = (DateTime)Session["TokenExpires"];
}
Authorization a = new Authorization();
try
{
    if (!HasValue(Session["AccessToken"]))
    {
        Api.LoginAsyncWithCode();
        Session["AccessToken"] = Api.Settings.AccessToken;
        Session["RefreshToken"] = Api.Settings.RefreshToken;
        Session["TokenExpires"] = Api.Settings.TokenExpires;
    }

}
catch(Exception ex)
{
    lblMessage.Text = ex.ToString();
}
try
{
        
    a =  Api.GetAuthorization();
    Session["BC_UserID"] = a.identity.id;
    Session["BC_Email"] = a.identity.email_address;
    Session["BC_FirstName"] = a.identity.first_name;
    Session["BC_LastName"] = a.identity.last_name;

    IEnumerable<Proj> projects = Proj.GetAllProjects(Api).All(Api);

    var results = from p in projects
                    select new
                    {
                        p.id,
                        p.created_at,
                        p.name,
                        p.description,
                        p.status,
                        p.purpose,
                        p.url
                    };
    if (projects.Count() > 0) {
        grdData.DataSource = results.ToList();
        grdData.AutoGenerateColumns = true;
        grdData.DataBind();
    }
}
catch(Exception ex)
{
    
}
```

## Setup before using the API

In order to use the Basecamp API you need to register your application at (launchpad.37signals.com/integrations)[launchpad.37signals.com/integrations]. This returns a Client Id and Client Secret. When registering, you have to provide a redirect uri for the OAuth2 authorisation process. For simple use, provide something like http://localhost/basecamp or your prefered redirect url to handle auth.

This information has to be provided in an object that implements the [ISettings](../master/BaseCampApi/Settings.cs) interface, which is then used to create a BaseCampApi instance. A Settings class which imnplements this interface is provided, to save you work. This provides a static Load method, reads the settings from *LocalApplicationData*/BaseCampApi/Settings.json. On a Windows 10 machine, *LocalApplicationData* is `C:\Users\<USER>\AppData\Local`, on Linux it is `~user/.local/share`.

Theres also an example below of loading these settings via code. which is the method i use


## Hooks for more complex uses

You do not have to use the provided Settings class, provided you have a class that implements ISettings.

As part of the OAuth2 process, the default implementation redirects to obtain authorisation. This is done by calling Response.Redirect like the code sample above. You can provide an alternative action to open a browser, or otherwise call the 37signals page to ask for authorisation.

Once authorisation is complete, the OAuth2 process will redirect the browser to the redirect url you provide in the settings. The default implementation by default you will need to be able to handle the redirect using the examples ive listed above, and collect the `code=` parameter from the request.

These options would be useful if you were using the Api in your own Web Server, for instance.

## License

This wrapper is licensed under creative commons share-alike, see [license.txt](../master/license.txt).

## Using the Api

The Unit Test file should give you a rough idea of how to query the data. except for its written in an async format which doesnt work. ill get around to changing these at some point. but if you follow the code examples above it should get you started.

An Api instance is created by passing it an object that implements ISettings (a default class is provided which will read the settings from a json file). The Api instance is IDisposable, so should be Disposed when no longer needed (this is because it contains an HttpClient).

C# classes are provided for the objects you can send to or receive from the BaseCampApi. For instance the Channel object represents channels. These main objects have methods which call the BaseCampApi api - such as Channel.Create to create a new channel, Channel.GetById to get channel details, etc.

Some Api calls return a list of items (such as Team.GetChannelsForUser). These are returned as an ApiList<Channel>. The BaseCampApi api itself usually only returns the first few items in the list, and needs to be called again to return the next chunk of items. This is all done for you by ApiList - it has a method called All(Api) which will return an IEnumerable of the appropriate listed object. Enumerating the enumerable will return all the items in the first chunk, then call the BaseCampApi api to get the next chunk, return them and so on. It hides all that work from the caller, while remaining as efficient as possible by only getting data when needed - for instance, using Linq calls like Any or First will stop getting data when the first item that matches the selection function is found.

-------------------------------------------

