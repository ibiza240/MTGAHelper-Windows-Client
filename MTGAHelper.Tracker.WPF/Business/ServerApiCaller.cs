using MTGAHelper.Web.Models.Response.User;
using MTGAHelper.Web.UI.Model.Response.Misc;
using MTGAHelper.Web.UI.Model.Response.User;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Net;
using System.Net.Http;
using System.Text;

namespace MTGAHelper.Tracker.WPF.Business
{
    public class ServerApiCaller
    {
//#if DEBUG
//        protected const string server = "https://localhost:5001";
//#else
        protected const string server = "https://mtgahelper.com";
//#endif

        public T GetResponseSimple<T>(string endpoint)
        {
            using (var w = new WebClient())
            {
                var responseRaw = w.DownloadString(server + endpoint);
                var parsed = JsonConvert.DeserializeObject<T>(responseRaw);
                return parsed;
            }
        }

        T GetResponseWithCookie<T>(string userId, string apiEndpoint)
        {
            var baseAddress = new Uri(server);
            var cookieContainer = new CookieContainer();
            using (var handler = new HttpClientHandler() { CookieContainer = cookieContainer })
            using (var client = new HttpClient(handler) { BaseAddress = baseAddress })
            {
                cookieContainer.Add(baseAddress, new Cookie("userid", userId));

                try
                {
                    var response = client.GetAsync(apiEndpoint).Result;
                    response.EnsureSuccessStatusCode();
                    var strResponse = response.Content.ReadAsStringAsync().Result;
                    var parsed = JsonConvert.DeserializeObject<T>(strResponse);
                    return parsed;
                }
                catch (AggregateException ex)
                {
                    throw new HttpRequestException("Remote server unavailable", ex);
                }
            }
        }

        public CollectionResponse UploadZippedLogFile(string userId, byte[] fileZipped)
        {
            var baseAddress = new Uri(server);
            var cookieContainer = new CookieContainer();

            using (var handler = new HttpClientHandler() { CookieContainer = cookieContainer })
            using (var client = new HttpClient(handler) { BaseAddress = baseAddress })
            {
                client.Timeout = new TimeSpan(0, 5, 0);
                var content = new MultipartFormDataContent();
                cookieContainer.Add(baseAddress, new Cookie("userid", userId));
                content.Add(new ByteArrayContent(fileZipped), "fileCollection", $"{userId}_{DateTime.Now.ToString("yyyyMMdd_HHmmss")}.zip");

                try
                {
                    var response = client.PostAsync("/api/User/Collection", content).Result;
                    response.EnsureSuccessStatusCode();
                    var strResponse = response.Content.ReadAsStringAsync().Result;
                    var parsed = JsonConvert.DeserializeObject<CollectionResponse>(strResponse);
                    return parsed;
                }
                catch (AggregateException ex)
                {
                    throw new HttpRequestException("Remote server unavailable", ex);
                }
            }
        }

        public bool IsLocalTrackerUpToDate()
        {
            try
            {
                System.Reflection.Assembly assembly = System.Reflection.Assembly.GetExecutingAssembly();
                FileVersionInfo fvi = FileVersionInfo.GetVersionInfo(assembly.Location);
                var latestVersion = GetResponseSimple<GetVersionTrackerResponse>("/api/Misc/VersionTracker").Version;
                return fvi.FileVersion == latestVersion;
            }
            catch (WebException)
            {
                // Ignore error on call
                return true;
            }
        }

        public bool IsSameLastUploadHash(string userId, string uploadHash)
        {
            var latestUploadHash = GetResponseWithCookie<LastUploadHashResponse>(userId, "/api/User/LastUploadHash").LastUploadHash;
            return latestUploadHash == uploadHash;
        }

        public GetCardsForDraftPickResponse GetWeightsForCard(string userId, ICollection<int> grpIds)
        {
            return GetResponseWithCookie<GetCardsForDraftPickResponse>(userId, $"/api/Dashboard/WeightByCard?grpIds={string.Join(',', grpIds)}");

        }

        public CollectionResponse GetCollection(string userId)
        {
            return GetResponseWithCookie<CollectionResponse>(userId, "/api/User/Collection");
        }
    }
}