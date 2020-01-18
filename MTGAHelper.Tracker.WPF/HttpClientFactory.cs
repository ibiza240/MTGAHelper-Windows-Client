using System;
using System.Net.Http;

namespace MTGAHelper.Tracker.WPF
{
    public class HttpClientFactory
    {
        public HttpClient Create(double timeoutSeconds)
        {
            return new HttpClient()
            {
                Timeout = TimeSpan.FromSeconds(timeoutSeconds)
            };
        }
    }
}
