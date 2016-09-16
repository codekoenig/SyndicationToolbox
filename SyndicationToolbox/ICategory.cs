using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace bpk.FeedParser
{
    public interface ICategory
    {
        string Term { get; set; }
        string Label { get; set; }
    }
}
