using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace bpk.FeedParser
{
    public interface IFeedItem<C>
        where C : ICategory, new()
    {
        string ServerId { get; set; }
        string Title { get; set; }
        string WebUri { get; set; }
        string Author { get; set; }
        DateTime Published { get; set; }
        string Content { get; set; }
        string CommentsUri { get; set; }
        List<C> Categories { get; set; }
        int? ParentFeedId { get; set; }
    }
}
