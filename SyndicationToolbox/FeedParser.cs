using System;
using System.Xml;
using System.Xml.Linq;

namespace CodeKoenig.SyndicationToolbox
{
    /// <summary>
    /// Parses a XML document containing supported feed format into an feed object model
    /// </summary>
    public abstract class FeedParser
    {
        public enum FeedVersion
        {
            RSS10 = 1,
            RSS20 = 2,
            Atom10 = 4,
            RDF = 8
        }

        private XDocument feedXmlDocument;
        protected XDocument FeedXmlDocument
        {
            get
            {
                return this.feedXmlDocument;
            }
            set
            {
                this.feedXmlDocument = value;
            }
        }

        /// <summary>
        /// Default contstuctor of FeedParser
        /// </summary>
        protected FeedParser()
        {
        }

        /// <summary>
        /// Creates the correct feed parser instance for the given feed content
        /// </summary>
        /// <param name="feedContent">String containing the feed's XML content</param>
        /// <returns>FeedParser instance that can parse the given feed content</returns>
        public static FeedParser Create(string feedContent)
        {
            if (feedContent == null)
            {
                throw new ArgumentNullException(nameof(feedContent));
            }

            XDocument feedXml;
            FeedParser parser = null;

            try
            {
                feedXml = XDocument.Parse(feedContent);
            }
            catch (XmlException ex)
            {
                // Try fixing invalid XML documents
                if (ex.Message.Contains("nbsp"))
                {
                    string fixedContent = feedContent.Replace("&nbsp;", " ");

                    feedXml = XDocument.Parse(fixedContent);

                    // If successful now, overwrite content
                    feedContent = fixedContent;
                }
                else
                {
                    throw;
                }
            }

            if ((feedXml != null))
            {
                // Get Feed Format
                switch (feedXml.Root.Name.LocalName.ToLower())
                {
                    case "rss":
                        // Expect RSS feed, get version
                        switch (feedXml.Root.Attribute("version").Value)
                        {
                            case "1.0":
                                parser = new RSSFeedParser();
                                break;
                            case "2.0":
                                parser = new RSSFeedParser();
                                break;
                            default:
                                // Version not supported, try parsing by RSS 1.0
                                parser = new RSSFeedParser();
                                break;
                        }
                        break;
                    case "feed":
                        // This is a standard Atom feed
                        parser = new AtomFeedParser();
                        break;
                    case "rdf":
                        // Expect old RDF Feed
                        parser = new RDFFeedParser();
                        break;
                }
            }

            // Assign XML document for further processing in subclass
            if (parser != null)
            {
                parser.FeedXmlDocument = feedXml;
            }

            return parser;
        }

        /// <summary>
        /// Parses the given feed XML document into a Feed
        /// </summary>
        /// <returns>A Feed object</returns>
        public abstract Feed Parse();
    }
}
