using System;
using System.Collections.Generic;

namespace CodeKoenig.SyndicationToolbox
{
    public class FeedArticle
    {
        public string ServerId { get; set; }
        public string Title { get; set; }
        public string WebUri { get; set; }
        public string Author { get; set; }
        public DateTime? Published { get; set; }
        public string Content { get; set; }
        public string CommentsUri { get; set; }
        public List<FeedCategory> Categories { get; set; }
        public int? ParentFeedId { get; set; }
    }
}
