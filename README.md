# Cloud CMS C# Driver

The [Cloud CMS](https://www.cloudcms.com/) C# driver is a .NET core client library to facilitate connections to Cloud CMS. The driver handles OAuth authentication and token management, HTTPS calls, and provides convenient methods to perform operations. It works against Cloud CMS instances on our SaaS platform as well as on-premise installations.

## Installation

Command Line:

````
dotnet add package cloudcms
````

Visual Studio:

````
Install-Package cloudcms
````

## Connecting to Cloud CMS

To connect to Cloud CMS, use the static `CloudCMSDriver.ConnectAsync` method. This takes either a file path to a `gitana.json` file, a JObject json object, dictionary, or ConnectionObject.

The required API key properties for this are:

- `clientKey`
- `clientSecret`
- `username`
- `password`
- `baseURL`

Connection examples:

````csharp
string path = "gitana.json";
IPlatform platform1 = await CloudCMSDriver.ConnectAsync(path);

JObject configObj = ...;
IPlatform platform2 = await CloudCMSDriver.ConnectAsync(configObj);

IDictionary<string, string> configDict = ...;
IPlatform platform3 = await CloudCMSDriver.ConnectAsync(configDict);

ConnectionConfig config = ...;
IPlatform platform4 = await CloudCMSDriver.ConnectAsync(config);
````

## Examples

Below are some examples of how you might use this driver:

````csharp
// Connect to Cloud CMS
string path = "gitana.json";
IPlatform platform = await CloudCMSDriver.ConnectAsync(path);

// Read repository
IRepository repository = await platform.ReadRepositoryAsync("<repositoryId>");

// Read branch
IBranch branch = await repository.ReadBranchAsync("<branchId>");

// Read node
INode node = await branch.ReadNodeAsync("<nodeID>");

// Update node
node.Data["title"] = "A new title";
await node.UpdateAsync();

// Delete node
await node.DeleteAsync();

// Create node
JObject obj = new JObject(
    new JProperty("title", "Twelfth Night"),
    new JProperty("description", "An old play")
);
INode newNode = await branch.CreateNodeAsync(obj);

// Query nodes
JObject query = new JObject(
    new JProperty("_type", "store:book")
);
JObject pagination = new JObject(
    new JProperty("limit", 2)
);
List<INode> queryNodes = await branch.QueryNodesAsync(query, pagination);

// Search/Find nodes
JObject find = new JObject(
    new JProperty("search", "Shakespeare"),
    new JProperty("query",
        new JObject(
            new JProperty("_type", "store:book")
        )
    )
);
List<INode> findNodes = await branch.FindNodesAsync(find, pagination);
````

## Tests
To run the tests for this driver, ensure you have your `gitana.json` file in the driver directory, then run:

````
dotnet test
````

## Resources

* Cloud CMS: https://www.cloudcms.com
* Github: http://github.com/gitana/cloudcms-csharp-driver
* C# Driver Download: https://www.nuget.org/packages/cloudcms
* Cloud CMS Documentation: https://www.cloudcms.com/documentation.html
* Developers Guide: https://www.cloudcms.com/developers.html

## Support

For information or questions about the C# Driver, please contact Cloud CMS
at [support@cloudcms.com](mailto:support@cloudcms.com).