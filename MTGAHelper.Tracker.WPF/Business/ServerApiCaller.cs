using MTGAHelper.Lib.IO.Reader.MtgaOutputLog;
using MTGAHelper.Web.Models.Request;
using MTGAHelper.Web.Models.Response.SharedDto;
using MTGAHelper.Web.Models.Response.User;
using MTGAHelper.Web.UI.Model.Request;
using MTGAHelper.Web.UI.Model.Response.Misc;
using MTGAHelper.Web.UI.Model.Response.User;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;

namespace MTGAHelper.Tracker.WPF.Business
{
    public class ServerApiCaller
    {
#if DEBUG
        protected const string server = "https://localhost:5001";
#else
        protected const string server = "https://mtgahelper.com";
#endif

        LogFileZipper zipper;

        public ServerApiCaller(LogFileZipper zipper)
        {
            this.zipper = zipper;
        }

        internal TReturn PostResponseWithCookie<TReturn>(string userId, string apiEndpoint, object body)
        {
            var baseAddress = new Uri(server);
            var cookieContainer = new CookieContainer();
            using (var handler = new HttpClientHandler() { CookieContainer = cookieContainer })
            using (var client = new HttpClient(handler) { BaseAddress = baseAddress })
            {
                cookieContainer.Add(baseAddress, new Cookie("userid", userId));
                //var test = JsonConvert.SerializeObject(body);

                try
                {
                    var response = client.PostAsync(apiEndpoint, new StringContent(JsonConvert.SerializeObject(body), Encoding.UTF8, "application/json")).Result;
                    response.EnsureSuccessStatusCode();
                    var strResponse = response.Content.ReadAsStringAsync().Result;
                    var parsed = JsonConvert.DeserializeObject<TReturn>(strResponse);
                    return parsed;
                }
                catch (AggregateException ex)
                {
                    throw new HttpRequestException("Remote server unavailable", ex);
                }
            }
        }

        internal TReturn PostResponseSimple<TReturn>(string apiEndpoint, object body)
        {
            using (var client = new HttpClient() { BaseAddress = new Uri(server) })
            {
                try
                {
                    var response = client.PostAsync(apiEndpoint, new StringContent(JsonConvert.SerializeObject(body), Encoding.UTF8, "application/json")).Result;
                    response.EnsureSuccessStatusCode();
                    var strResponse = response.Content.ReadAsStringAsync().Result;
                    var parsed = JsonConvert.DeserializeObject<TReturn>(strResponse);
                    return parsed;
                }
                catch (AggregateException ex)
                {
                    throw new HttpRequestException("Remote server unavailable", ex);
                }
            }
        }

        internal T GetResponseSimple<T>(string endpoint)
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

        internal bool IsLocalTrackerUpToDate()
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


        //public CollectionResponse UploadZippedLogFile(string userId, byte[] fileZipped)
        //{
        //    var baseAddress = new Uri(server);
        //    var cookieContainer = new CookieContainer();

        //    using (var handler = new HttpClientHandler() { CookieContainer = cookieContainer })
        //    using (var client = new HttpClient(handler) { BaseAddress = baseAddress })
        //    {
        //        client.Timeout = new TimeSpan(0, 5, 0);
        //        var content = new MultipartFormDataContent();
        //        cookieContainer.Add(baseAddress, new Cookie("userid", userId));
        //        content.Add(new ByteArrayContent(fileZipped), "fileCollection", $"{userId}_{DateTime.Now.ToString("yyyyMMdd_HHmmss")}.zip");

        //        try
        //        {
        //            var response = client.PostAsync("/api/User/Collection", content).Result;
        //            response.EnsureSuccessStatusCode();
        //            var strResponse = response.Content.ReadAsStringAsync().Result;
        //            var parsed = JsonConvert.DeserializeObject<CollectionResponse>(strResponse);
        //            return parsed;
        //        }
        //        catch (AggregateException ex)
        //        {
        //            throw new HttpRequestException("Remote server unavailable", ex);
        //        }
        //    }
        //}
        internal CollectionResponse UploadOutputLogResult(string userId, OutputLogResult result)
        {
            return PostResponseWithCookie<CollectionResponse>(userId, "/api/User/LogFileProcessed", new PostOutputLogProcessedRequest(result));
        }

        internal bool IsSameLastUploadHash(string userId, uint uploadHash)
        {
            var latestUploadHash = GetResponseWithCookie<LastHashResponse>(userId, "/api/User/LastUploadHash").LastHash;
            return latestUploadHash == uploadHash.ToString();
        }

        internal GetCardsForDraftPickResponse GetCardsForDraftPick(string userId, ICollection<int> grpIds)
        {
            return GetResponseWithCookie<GetCardsForDraftPickResponse>(userId, $"/api/User/DraftPick?grpIds={string.Join(',', grpIds)}");

        }

        internal CollectionResponse GetCollection(string userId)
        {
            return GetResponseWithCookie<CollectionResponse>(userId, "/api/User/Collection");
        }

        internal void LogErrorRemote(string userId, ErrorTypeEnum errorType, Exception ex)
        {
            try
            {
                PostResponseSimple<string>("/api/Misc/LogRemoteError", new PostLogRemoteErrorRequest
                {
                    UserId = userId,
                    ErrorType = errorType,
                    Exception = ex.ToString()
                });
            }
            catch
            {
                // Ignore error
            }
        }

        internal void LogErrorRemoteFile(string userId, string logContent, string filename)
        {
            var bytes = zipper.ZipText(logContent);
            var fileContent = new StreamContent(new MemoryStream(bytes))
            {
                Headers =
                   {
                       ContentLength = bytes.Length,
                       ContentType = new MediaTypeHeaderValue("application/zip")
                   }
            };

            var formDataContent = new MultipartFormDataContent();
            formDataContent.Add(fileContent, "fileOutputLog", filename);

            var baseAddress = new Uri(server);
            var cookieContainer = new CookieContainer();
            using (var handler = new HttpClientHandler() { CookieContainer = cookieContainer })
            using (var client = new HttpClient(handler) { BaseAddress = baseAddress })
            {
                cookieContainer.Add(baseAddress, new Cookie("userid", userId));
                try
                {
                    var response = client.PostAsync("/api/Misc/LogRemoteErrorFile", formDataContent).Result;
                }
                catch
                {
                    // Ignore error
                    //throw new HttpRequestException("Remote server unavailable", ex);
                }
            }
        }
    }
}