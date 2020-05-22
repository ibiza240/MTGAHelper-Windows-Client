using MTGAHelper.Tracker.WPF.Exceptions;
using MTGAHelper.Tracker.WPF.Tools;
using Serilog;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace MTGAHelper.Tracker.WPF.Business
{
    public class DataDownloader
    {
        private readonly HttpClientFactory httpClientFactory = new HttpClientFactory();

        public async Task HttpClientDownloadFile_WithTimeoutNotification(string requestUri, double timeout, string filePath)
        {
            using HttpClient client = httpClientFactory.Create(timeout);

            using var request = new HttpRequestMessage(HttpMethod.Get, requestUri);

            using Stream contentStream = await (await client.SendAsync(request)).Content.ReadAsStreamAsync(),
                stream = new FileStream(filePath, FileMode.Create, FileAccess.Write, FileShare.Read);

            await contentStream.CopyToAsync(stream);
        }

        public string HttpClientGet_WithTimeoutNotification(string requestUri, double timeout)
        {
            using var client = httpClientFactory.Create(timeout);

            while (true)
            {
                try
                {
                    string raw = client.GetAsync(requestUri).Result.Content.ReadAsStringAsync().Result;
                    return raw;
                }
                catch (WebException ex) when (ex.Status == WebExceptionStatus.Timeout)
                {
                    Log.Warning(ex, "TimeoutNotification({requestUri}, {timeout})", requestUri, timeout);
                    bool doRequest = MessageBox.Show($"It appears the server didn't reply in time ({timeout} seconds). Do you want to retry? Choosing No will stop the program.{Environment.NewLine}{Environment.NewLine}Maybe the server is down? Go check https://mtgahelper.com and if that is the case, please retry later.",
                        "MTGAHelper", MessageBoxButton.YesNo) == MessageBoxResult.Yes;

                    if (doRequest == false)
                        throw new ServerNotAvailableException();
                }
                catch (Exception ex)
                {
                    Log.Error(ex, "Unknown error in TimeoutNotification({requestUri}, {timeout})", requestUri, timeout);
                    return string.Empty;
                }
            }
        }

    }
}
