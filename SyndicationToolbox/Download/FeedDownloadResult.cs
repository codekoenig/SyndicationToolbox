using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace bpk.SyndicationToolbox.Download
{
    public class FeedDownloadResult
    {
        public string Content { get; set; }
        public Uri RedirectUri { get; set; }
        public string HttpETag { get; set; }
        public string HttpLastModified { get; set; }
        public int HttpStatusCode { get; set; }
        public Encoding DetectedEncoding { get; set; }
    }
}
