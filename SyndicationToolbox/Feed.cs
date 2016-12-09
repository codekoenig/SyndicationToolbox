using System.Collections.Generic;

namespace CodeKoenig.SyndicationToolbox
{
    public class Feed
    {
        public string Name { get; set; }
        public string Uri { get; set; }
        public string WebUri { get; set; }
        public string HubbubUri { get; set; }
        public string Generator { get; set; }
        public List<FeedArticle> Articles { get; set; }
        public List<FeedCategory> Categories { get; set; }
    }
}
