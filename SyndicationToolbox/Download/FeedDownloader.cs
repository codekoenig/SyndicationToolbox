using System;
using System.Diagnostics;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace CodeKoenig.SyndicationToolbox.Download
{
    public class FeedDownloader
    {
        HttpClient httpClient;

        public FeedDownloader()
            : this(null)
        {
        }

        public FeedDownloader(ProductInfoHeaderValue productInfo)
        {
            if (productInfo == null)
            {
                productInfo = new ProductInfoHeaderValue("CodeKoenig.SyndicationToolbox", new AssemblyName(GetType().GetTypeInfo().Assembly.FullName).Version.ToString());
            }

            HttpClientHandler handler = new HttpClientHandler();

            if (handler.SupportsAutomaticDecompression)
            {
                handler.AutomaticDecompression = DecompressionMethods.GZip | DecompressionMethods.Deflate;
            }

            // Initialize HttpClient
            httpClient = new HttpClient(handler);
            httpClient.Timeout = TimeSpan.FromSeconds(15);
            httpClient.DefaultRequestHeaders.UserAgent.Add(productInfo);
        }

        /// <summary>
        /// Downloads feed contents from the given feedUri
        /// </summary>
        /// <param name="feedUri">The URI to download the feed contents from</param>
        /// <param name="httpLastModified">If available, provide LastModified value for caching support.</param>
        /// <param name="httpETag">If available, provide ETag value for caching support.</param>
        /// <returns>Result of the feed download</returns>
        public async Task<FeedDownloadResult> DownloadFeedContent(Uri feedUri, string httpLastModified = null, string httpETag = null)
        {
            FeedDownloadResult result = new FeedDownloadResult();

            HttpRequestMessage reqMessage = new HttpRequestMessage(HttpMethod.Get, feedUri);

            // Add headers for conditional GET
            if (!string.IsNullOrEmpty(httpLastModified))
            {
                reqMessage.Headers.IfModifiedSince = DateTimeOffset.Parse(httpLastModified);
            }

            if (!string.IsNullOrEmpty(httpETag))
            {
                reqMessage.Headers.IfNoneMatch.Add(new EntityTagHeaderValue(string.Format("\"{0}\"", httpETag.Replace("\"", string.Empty))));
            }

            try
            {
                HttpResponseMessage response = await httpClient.SendAsync(reqMessage).ConfigureAwait(false);

                if (response.StatusCode == HttpStatusCode.NotModified)
                {
                    return new FeedDownloadResult { Content = null, HttpETag = httpETag, HttpLastModified = httpLastModified, HttpStatusCode = (int)response.StatusCode };
                }

                response.EnsureSuccessStatusCode();

                // If original URI request was redirected, update result with this info
                if (!response.RequestMessage.RequestUri.ToString().Equals(feedUri.ToString(), StringComparison.CurrentCultureIgnoreCase))
                {
                    result.RedirectUri = response.RequestMessage.RequestUri;
                }

                // Update feed with conditional GET info if available
                result.HttpLastModified = response.Content.Headers.LastModified != null ? response.Content.Headers.LastModified.ToString() : null;
                result.HttpETag = response.Headers.ETag != null ? response.Headers.ETag.Tag : null;

                // Try get encoding from response or use UTF8 if it fails
                if (response.Content.Headers.ContentType != null && !string.IsNullOrEmpty(response.Content.Headers.ContentType.CharSet))
                {
                    try
                    {
                        result.DetectedEncoding = Encoding.GetEncoding(response.Content.Headers.ContentType.CharSet.Replace("\"", string.Empty).Replace("'", string.Empty));
                    }
                    catch (Exception ex)
                    {
                        Debug.WriteLine(string.Format("Error: Unable to get encoding from feed on URI '{0}': {1}", feedUri.ToString(), ex.Message));
                        result.DetectedEncoding = Encoding.UTF8;
                    }
                }
                else
                {
                    result.DetectedEncoding = Encoding.UTF8;
                }

                result.Content = await response.Content.ReadAsStringAsync();
                result.HttpStatusCode = (int)response.StatusCode;
            }
            catch (Exception ex)
            {
                Debug.WriteLine(string.Format("Error during downloading feed content from URI '{0}': {1}", feedUri.ToString(), ex.Message));
                throw;
            }

            return result;
        }
    }
}
