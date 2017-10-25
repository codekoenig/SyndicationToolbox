using CodeKoenig.SyndicationToolbox.Tools;
using System;
using System.Linq;
using System.Xml.Linq;

namespace CodeKoenig.SyndicationToolbox
{
    /// <summary>
    /// Parses a RDF XML document containing supported feed format into an feed object model
    /// </summary>
    internal class RDFFeedParser : FeedParser
    {
        // RDF namespace prefix
        private XNamespace rdfNamespace = "http://www.w3.org/1999/02/22-rdf-syntax-ns#";

        // Default namespace
        private XNamespace defaultNamespace;

        // Namespace prefixes for supporting funky RDF
        private XNamespace dcNamespace = "http://purl.org/dc/elements/1.1/";
        private XNamespace contentNamespace = "http://purl.org/rss/1.0/modules/content/";

        internal RDFFeedParser()
        {
        }

        public override Feed Parse()
        {
            // Get default namespace
            defaultNamespace = XHelper.SafeGetString(FeedXmlDocument.Root, "xmlns");

            // Parse feed XML into class hierarchy
            var feed = from e in this.FeedXmlDocument.Root.Elements(this.defaultNamespace + "channel")
                       select new Feed()
                       {
                           Name = XHelper.SafeGetString(e.Element(this.defaultNamespace + "title")),
                           WebUri = XHelper.SafeGetString(e.Element(this.defaultNamespace + "link")),
                           Articles = (from i in this.FeedXmlDocument.Root.Elements(this.defaultNamespace + "item")
                                        let itemId = XHelper.SafeGetString(i.Element(this.defaultNamespace + "guid")) ?? XHelper.SafeGetString(i.Element(this.defaultNamespace + "link"))   // Try use Link as GUID as some RDF feeds do not have a GUID
                                        let description = XHelper.SafeGetString(i.Element(this.defaultNamespace + "description"))
                                        let content = XHelper.SafeGetString(i.Element(this.contentNamespace + "encoded"))
                                        select new FeedArticle
                                        {
                                            ServerId = itemId,
                                            Title = XHelper.SafeGetString(i.Element(this.defaultNamespace + "title")),
                                            WebUri = XHelper.SafeGetString(i.Element(this.defaultNamespace + "link")),
                                            Author = XHelper.SafeGetString(i.Element(this.defaultNamespace + "author")),
                                            CommentsUri = XHelper.SafeGetString(i.Element(this.defaultNamespace + "comments")),
                                            Published = i.Element(this.defaultNamespace + "pubDate") != null ? RFCDateParser.ParseRFC822Date(i.Element(this.defaultNamespace + "pubDate").Value, null) : null,
                                            Content = content ?? description
                                        }).ToList()
                       };

            return feed.FirstOrDefault();
        }
    }
}
