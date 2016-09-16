using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace bpk.SyndicationToolbox
{
    public class ParsedFeedItem
    {
        public string ServerId { get; set; }
        public string Title { get; set; }
        public string WebUri { get; set; }
        public string Author { get; set; }
        public DateTime Published { get; set; }
        public string Content { get; set; }
        public string CommentsUri { get; set; }
        public List<ParsedCategory> Categories { get; set; }
        public int? ParentFeedId { get; set; }
    }
}
