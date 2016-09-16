using System;
using System.Linq;
using System.Xml.Linq;
using bpk.SyndicationToolbox.Tools;

namespace bpk.SyndicationToolbox
{
    /// <summary>
    /// Parses a RSS XML document containing supported feed format into an feed object model
    /// </summary>
    public class RSSFeedParser : FeedParser
    {
        // Namespace prefixes for supporting funky RSS
        private XNamespace dcNamespace = "http://purl.org/dc/elements/1.1/";
        private XNamespace contentNamespace = "http://purl.org/rss/1.0/modules/content/";
        private XNamespace atomNamespace = "http://www.w3.org/2005/Atom";

        internal RSSFeedParser()
        {
        }

        public override ParsedFeed Parse()
        {
            // Parse feed XML into class hierarchy
            var feed = from e in this.FeedXmlDocument.Element("rss").Elements("channel")
                       let HubbubUri = e.Elements(atomNamespace + "link").FirstOrDefault(l => l.Attribute("rel") != null && l.Attribute("rel").Value == "hub")
                       let SelfRefLink = e.Elements(atomNamespace + "link").FirstOrDefault(l => l.Attribute("rel") != null && l.Attribute("rel").Value == "self")
                       select new ParsedFeed()
                       {
                           Name = XHelper.SafeGetString(e.Element("title")),
                           Uri = XHelper.SafeGetString(SelfRefLink, "href"),
                           WebUri = XHelper.SafeGetString(e.Element("link")),
                           HubbubUri = XHelper.SafeGetString(HubbubUri, "href"),
                           FeedItems = (from i in e.Elements("item")
                                        let itemId = XHelper.SafeGetString(i.Element("guid")) ?? XHelper.SafeGetString(i.Element("link"))   // Try use Link as GUID as some RSS feeds do not have a GUID
                                        let description = XHelper.SafeGetString(i.Element("description"))
                                        let content = XHelper.SafeGetString(i.Element(this.contentNamespace + "encoded"))
                                        select new ParsedFeedItem
                                        {
                                            ServerId = itemId,
                                            Title = XHelper.SafeGetString(i.Element("title")),
                                            WebUri = XHelper.SafeGetString(i.Element("link")),
                                            Author = XHelper.SafeGetString(i.Element("author")),
                                            CommentsUri = XHelper.SafeGetString(i.Element("comments")),
                                            Published = i.Element("pubDate") != null ? RFCDateParser.ParseRFC822Date(i.Element("pubDate").Value, DateTime.Now) : DateTime.Now,
                                            Content = content ?? description
                                        }).ToList()
                       };

            return feed.FirstOrDefault();
        }

    }
}
