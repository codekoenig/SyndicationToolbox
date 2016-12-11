# SyndicationToolbox

SyndicationToolbox is a simple RSS/Atom/RDF parser library that comes with a few additional helpers to simplyfy
handling of various syndication tasks.

It is built against NETStandard 1.2 and thus can be used with a wide variety of .NET projects, including but not limited to
.NET Core projects, Windows Phone 8.1, Universal Windows Platform and of course full .NET Framework projects.

## Add SyndicationToolbox to your project

In package manager console, execute:

```shell
Install-Package CodeKoenig.SyndicationToolbox -Pre
```

## Quickstart

Let's start with saving you 15 seconds and tell you the namespace SyndicationToolbox is using:

```csharp
using CodeKoenig.SyndicationToolbox;
```

Using SyndicationToolbox to parse any feed is as easy as passing the contents of a feed as a `string` to
the factory method `FeedParser.Create()`:

```csharp
    FeedParser feedParser = FeedParser.Create(feedContent);
    Feed feed = feedParser.Parse();
```

FeedParser will automatically figure out the kind of feed you passed (RSS, Atom or RDF) and will parse
it's contents into an instance of `Feed` - which offers a nice way to access all relevant contents of the feed:

|Property   |Type                 |Description                                  |
|-----------|---------------------|---------------------------------------------|
|Articles   |`List<FeedArticle>`  |List of all articles contained in the feed   |
|Categories |`List<FeedCategory>` |List of all categories the feed has assigned |
|Generator  |`string`             |RSS or Atom Generator information            |
|HubbubUri  |`string`             |PubSubHubbub URI, if the feed supports it    |
|Name       |`string`             |The name (title) of the feed                 |
|Uri        |`string`             |The URI at which the feed can be found       |
|WebUri     |`string`             |The website URI the feed belongs to          |

That's all you need to know to parse any feed you want.

## Helpers

### Feed download helper

Don't know how you can get a feed into a `string`? Just kidding, of course you do. But if you need a fast
way do download a feed from the web and put it's contents into a `string`, SyndicationToolbox provides you
with a helper class that will do this for you and will provide you with some useful metadata. You'll find
the `FeedDownloader` in the `CodeKoenig.SyndicationToolbox.Download` namespace.

```csharp
FeedDownloader feedDownloader = new FeedDownloader();
DownloadResult result = await FeedDownloader.DownloadFeedContent(feedUri);

string feedContent = result.Content;
```

The `DownloadResult` will contain the contents of the feed as a `string` in its `Content` property. Here's
the complete list of properties that the `DownloadResult` will provide:

|Property          |Type       |Description                                                    |
|------------------|-----------|---------------------------------------------------------------|
|Content           |`string`   |The content of the feed                                        |
|RedirectUri       |`Uri`      |If the HTTP request was redirected, contains the redirect URI  |
|HttpETag          |`string`   |Contains the HttpETag response header field, if present        |
|HttpLastModified  |`string`   |Contains the HttpLastModified header field, if present         |
|HttpStatusCode    |`int`      |The HTTP status code that resulted from the HTTP request       |
|DetectedEncoding  |`Encoding` |The detected encoding of the feed                              |

All those properties help you to manage your feed subscriptions, if needed. You can update the feed URI if
your feed requests get redirected and you can manage HTTP caching with support for ETag and LastModified
headers. If present, you should always pass an ETag or LastModified header with your next download request.
`DownloadFeedContent()` has according arguments:

```csharp
// With LastModified:
DownloadResult result = await FeedDownloader.DownloadFeedContent(feedUri, httpLastModified: lastModifiedValue);
// With ETag:
DownloadResult result = await FeedDownloader.DownloadFeedContent(feedUri, httpETag: eTagValue);
```

`FeedDownloader` will also make use of HTTP compression when possible.

Thus, `FeedDownloader` is overall a good starting point for downloading feed contents for use with `FeedParser`
or other use cases until you want or need to implement your own download logic.

### XHelper

Getting values from XML elements or attributes can be cumbersome with all the required `NULL` handling.
`SyndicationToolbox` thus makes use of some handy static helper methods to safely retrieve those values from
a feed document (or any XML document, really). Those methods are exposed publicly over the static `XHelper` class
in the `SyndicationToolbox.Tools` namespace.

`SyndicationToolbox` uses Linq-to-XML for parsing feed documents, so the usual Linq-to-XML types like
`XDocument`, `XElement` or `XAttribute` are supported. It does not work with the types in `System.Xml`.

The general idea is: you pass `XHelper` a `XElement` instance and it will return the contained value if
it can be parsed to the desired type. If not, or if the `XElement` is even `NULL`, `XHelper` will safely
return just `NULL` and never throw an exception:

```csharp
// Returns NULL if myXElement is NULL, or the value of the element is empty. Else returns the string.
string elementStringValue = XHelper.SafeGetString(myXElement);
// Returns NULL if mxXElement is NULL or the value of the element can't pe parsed to an int. Else returns the int.
int elementIntValue = XHelper.SafeGetInt(myXElement);
```

This also works to retrieve the value of an XML attribute of an XML element. Just also pass the desired
attribute name:

```csharp
// Returns NULL if no attribute "firstName" exists on the element, or the attribute value is empty
string elementStringValue = XHelper.SafeGetString(myXElement, "firstName");
```

Finally, here's a complete sample:

```Xml
<?xml version="1.0"?>
<customers>
    <customer id="1" lastOrder="2016-05-01T09:40:00">
        <firstName>Charly</firstName>
        <lastName>Kaufman</lastName>
        <customerValue>3</customerValue>
    </customer>
    <customer id="2">
        <firstName>Donald</firstName>
        <lastName>Kaufman</lastName>
        <customerValue>crapitsabug</customerValue>
    </customer>
</customers>
```

```csharp
public void ParseTheAboveXml(string aboveXml)
{
    XDocument doc = XDocument.Parse(aboveXml);

    var result = from e in doc.Element("customers").Elements("customer")
                 select new
                 {
                     Id = int.Parse(e.Attribute("id").Value),  // Mandatory, so just parse it
                     FirstName = XHelper.SafeGetString(e.Element("firstName")),
                     CustomerValue = XHelper.SafeGetInt(e.Element("customerValue")),
                     LastOrder = XHelper.SafeGetDateTime(e, "lastOrder")
                 };

    foreach (var customer in result)
    {
        Console.WriteLine(customer.Id);
        Console.WriteLine(customer.FirstName);
        Console.WriteLine(customer.CustomerValue);
        Console.WriteLine(customer.LastOrder);
        Console.WriteLine("---");
    }

    // Output:
    // 1
    // Charly
    // 3
    // 2016-05-01 09:40:00
    // ---
    // 2
    // Donald
    // ---
}
```

What you see is that despite on the 2nd customer, the `lastOrder` attribute is not present and the `CustomerValue`
element does not contain a valid int value, the program does not crash but instead just assigns `NULL` to the
according properties.

Simply speaking: whenever you have optional elements or attributes and want to avoid cluttering
your LINQ-to-XML queries with, maybe several, `NULL` and parsing checks, `XHelper` can help you with that.