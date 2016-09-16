using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace bpk.FeedParser
{
    public interface IFeed<FI, C>
        where FI : IFeedItem<C>, new()
        where C : ICategory, new()
    {
        string Name { get; set; }
        string Uri { get; set; }
        string WebUri { get; set; }
        List<FI> FeedItems { get; set; }
        List<C> Categories { get; set; }
    }
}
