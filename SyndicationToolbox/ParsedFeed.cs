using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace bpk.SyndicationToolbox
{
    public class ParsedFeed
    {
        public string Name { get; set; }
        public string Uri { get; set; }
        public string WebUri { get; set; }
        public string HubbubUri { get; set; }
        public List<ParsedFeedItem> FeedItems { get; set; }
        public List<ParsedCategory> Categories { get; set; }
    }
}
