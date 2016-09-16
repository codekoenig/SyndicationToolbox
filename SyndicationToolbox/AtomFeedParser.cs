using System;
using System.Linq;
using System.Xml.Linq;
using bpk.SyndicationToolbox.Tools;

namespace bpk.SyndicationToolbox
{
    /// <summary>
    /// Parses an Atom XML document containing supported feed format into an feed object model
    /// </summary>
    public class AtomFeedParser : FeedParser
    {
        private XNamespace defaultNamespace;
        private XNamespace nsNamespace = "http://www.newssync.net/schemas/atom";

        internal AtomFeedParser()
        {
        }

        public override ParsedFeed Parse()
        {
            // Get default namespace
            defaultNamespace = XHelper.SafeGetString(FeedXmlDocument.Root, "xmlns");

            var feed = from e in this.FeedXmlDocument.Elements()
                       let HubbubUri = e.Elements(this.defaultNamespace + "link").FirstOrDefault(l => l.Attribute("rel") != null && l.Attribute("rel").Value == "hub")
                       let SelfRefLink = e.Elements(this.defaultNamespace + "link").FirstOrDefault(l => l.Attribute("rel") != null && l.Attribute("rel").Value == "self")
                       let ViaLink = e.Elements(this.defaultNamespace + "link").FirstOrDefault(l => l.Attribute("rel") != null && l.Attribute("rel").Value == "via")
                       let AlternateLink = e.Elements(this.defaultNamespace + "link").FirstOrDefault(l => l.Attribute("rel") != null && l.Attribute("rel").Value == "alternate")
                       let DefaultLink = e.Elements(this.defaultNamespace + "link").FirstOrDefault(l => !l.Attributes().Any())
                       select new ParsedFeed()
                       {
                           Name = XHelper.SafeGetString(e.Element(this.defaultNamespace + "title")),
                           Uri = XHelper.SafeGetString(SelfRefLink, "href"),
                           HubbubUri = XHelper.SafeGetString(HubbubUri, "href"),
                           WebUri = XHelper.SafeGetString(AlternateLink, "href") ?? XHelper.SafeGetString(DefaultLink, "href"),
                           Categories = (from c in e.Elements(this.defaultNamespace + "category")
                                         select new ParsedCategory
                                         {
                                             Term = XHelper.SafeGetString(c.Element(this.defaultNamespace + "term")),
                                             Label = XHelper.SafeGetString(c.Element(this.defaultNamespace + "label"))
                                         }).ToList(),
                           FeedItems = (from i in e.Elements(this.defaultNamespace + "entry")
                                        let publishedDate = XHelper.SafeGetDateTime(i.Element(this.defaultNamespace + "published"))
                                        let updatedDate = XHelper.SafeGetDateTime(i.Element(this.defaultNamespace + "updated"))
                                        let itemId = XHelper.SafeGetString(i.Element(this.defaultNamespace + "id")) ?? XHelper.SafeGetString(i.Element(this.defaultNamespace + "link"), "href")
                                        let description = XHelper.SafeGetString(i.Element(this.defaultNamespace + "summary"))
                                        let content = XHelper.SafeGetString(i.Element(this.defaultNamespace + "content"))
                                        select new ParsedFeedItem()
                                        {
                                            ServerId = itemId,
                                            Title = XHelper.SafeGetString(i.Element(this.defaultNamespace + "title")),
                                            WebUri = XHelper.SafeGetString(i.Element(this.defaultNamespace + "link"), "href"),
                                            Author = i.Element(this.defaultNamespace + "author") != null
                                                     ? XHelper.SafeGetString(i.Element(this.defaultNamespace + "author").Element(this.defaultNamespace + "name"))
                                                     : null,
                                            Published = publishedDate ?? updatedDate ?? DateTime.Now,
                                            Content = content ?? description,
                                            Categories = (from c in i.Elements(this.defaultNamespace + "category")
                                                          select new ParsedCategory
                                                          {
                                                              Term = XHelper.SafeGetString(c, "term"),
                                                              Label = XHelper.SafeGetString(c, "label")
                                                          }).ToList(),
                                            ParentFeedId = XHelper.SafeGetInt(i, nsNamespace + "parentfeed-id")
                                        }).ToList()
                       };

            return feed.FirstOrDefault();
        }
    }
}
