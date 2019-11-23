using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;

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
