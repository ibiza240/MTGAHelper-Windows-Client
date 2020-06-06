using MTGAHelper.Lib.OutputLogParser.Models;
using MTGAHelper.Tracker.WPF.Config;
using MTGAHelper.Web.Models.Request;
using MTGAHelper.Web.Models.Response.Account;
using MTGAHelper.Web.Models.Response.SharedDto;
using MTGAHelper.Web.Models.Response.User;
using MTGAHelper.Web.UI.Model.Request;
using MTGAHelper.Web.UI.Model.Response.User;
using Newtonsoft.Json;
using System;
using System.Diagnostics;
using System.Net;
using System.Net.Http;
using System.Text;
using MTGAHelper.Entity.MtgaOutputLog;
using WebApplication1.Model.Account;

namespace MTGAHelper.Tracker.WPF.Business
{
    public class ServerApiCaller
    {
        private const string SERVER = DebugOrRelease.Server;

        private readonly Uri BaseAddress = new Uri(SERVER);

        private readonly CookieContainer CookieContainer;
        private readonly HttpClient Client;


        public ServerApiCaller()
        {
            CookieContainer = new CookieContainer();
            var handler = new HttpClientHandler { CookieContainer = CookieContainer };
            Client = new HttpClient(handler) { BaseAddress = BaseAddress };
        }

        public void SetUserId(string userId)
        {
            CookieContainer.SetCookies(BaseAddress, "userId=" + userId);
        }

        internal AccountResponse ValidateExternalToken(string provider, string token)
        {
            return PostResponseSimple<AccountResponse>($"api/WpfLogin/ValidateToken{provider}", new ValidatExternalTokenRequest(token));
        }

        internal AccountResponse ValidateLocalUser(string email, string password)
        {
            return GetResponseWithCookie<AccountResponse>($"api/Account/Signin?email={System.Web.HttpUtility.UrlEncode(email)}&password={System.Web.HttpUtility.UrlEncode(password)}");
        }

        internal AccountResponse AutoSigninLocalUser(string email, string hash)
        {
            return GetResponseWithCookie<AccountResponse>($"api/WpfLogin/AutoSigninLocalUser?email={System.Web.HttpUtility.UrlEncode(email)}&hash={System.Web.HttpUtility.UrlEncode(hash)}");
        }

        internal TReturn PostResponseSimple<TReturn>(string apiEndpoint, object body, bool isPut = false)
        {
            //using (var client = new HttpClient() { BaseAddress = new Uri(server) })
            {
                try
                {
                    HttpResponseMessage response = null;
                    if (isPut)
                        response = Client.PutAsync(apiEndpoint, new StringContent(JsonConvert.SerializeObject(body), Encoding.UTF8, "application/json")).Result;
                    else
                        response = Client.PostAsync(apiEndpoint, new StringContent(JsonConvert.SerializeObject(body), Encoding.UTF8, "application/json")).Result;

                    response.EnsureSuccessStatusCode();
                    string strResponse = response.Content.ReadAsStringAsync().Result;
                    var parsed = JsonConvert.DeserializeObject<TReturn>(strResponse);
                    return parsed;
                }
                catch (AggregateException ex)
                {
                    throw new HttpRequestException("Remote server unavailable", ex);
                }
            }
        }

        private T GetResponseWithCookie<T>(string apiEndpoint)
        {
            //var baseAddress = new Uri(server);
            //var cookieContainer = new CookieContainer();
            //using (var handler = new HttpClientHandler() { CookieContainer = cookieContainer })
            //using (var client = new HttpClient(handler) { BaseAddress = baseAddress })
            {
                try
                {
                    var response = Client.GetAsync(apiEndpoint).Result;
                    response.EnsureSuccessStatusCode();
                    string strResponse = response.Content.ReadAsStringAsync().Result;

                    if (typeof(T) == typeof(string))
                        return (T)Convert.ChangeType(strResponse, typeof(T));

                    var parsed = JsonConvert.DeserializeObject<T>(strResponse);
                    return parsed;
                }
                catch (AggregateException ex)
                {
                    throw new HttpRequestException("Remote server unavailable", ex);
                }
                catch (Exception)
                {
                    Debugger.Break();
                    return default;
                }
            }
        }

        internal string GetAccountSalt()
        {
            return GetResponseWithCookie<string>("api/WpfLogin/AccountSalt");
        }

        internal bool IsSupporter(string email)
        {
            return GetResponseWithCookie<bool>("api/user/issupporter?userEmail=" + email);
        }

        internal string GetCustomDraftRatings()
        {
            return GetResponseWithCookie<string>("api/user/customdraftratings");
        }

        internal void SaveCustomDraftRating(int grpId, int? ratingValue, string ratingNote)
        {
            var body = new PutUserCustomDraftRatingDto
            {
                IdArena = grpId,
                Rating = ratingValue,
                Note = ratingNote,
            };

            _ = PostResponseSimple<string>("api/user/customdraftrating", body, true);
        }

        internal DraftRaredraftingInfoResponse GetRaredraftingInfo(string mtgaHelperUserId)
        {
            return GetResponseWithCookie<DraftRaredraftingInfoResponse>("api/User/Compare");
        }
        internal CollectionResponse UploadOutputLogResult(string userId, OutputLogResult result)
        {
            return PostResponseSimple<CollectionResponse>("/api/User/LogFileProcessed", new PostOutputLogProcessedRequest(result));
        }
        internal CollectionResponse UploadOutputLogResult2(string userId, OutputLogResult2 result2)
        {
            return PostResponseSimple<CollectionResponse>("/api/User/LogFileProcessed2", new PostOutputLogProcessedRequest2(result2));
        }

        internal bool IsSameLastUploadHash(uint uploadHash)
        {
            string latestUploadHash = GetResponseWithCookie<LastHashResponse>("/api/User/LastUploadHash").LastHash;
            return latestUploadHash == uploadHash.ToString();
        }

        internal CollectionResponse GetCollection()
        {
            return GetResponseWithCookie<CollectionResponse>("api/User/Collection");
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
            //var bytes = zipper.ZipText(logContent);
            //var fileContent = new StreamContent(new MemoryStream(bytes))
            //{
            //    Headers =
            //       {
            //           ContentLength = bytes.Length,
            //           ContentType = new MediaTypeHeaderValue("application/zip")
            //       }
            //};

            //var formDataContent = new MultipartFormDataContent();
            //formDataContent.Add(fileContent, "fileOutputLog", filename);

            //var baseAddress = new Uri(server);
            //var cookieContainer = new CookieContainer();
            //using (var handler = new HttpClientHandler() { CookieContainer = cookieContainer })
            //using (var client = new HttpClient(handler) { BaseAddress = baseAddress })
            //{
            //    cookieContainer.Add(baseAddress, new Cookie("userId", userId));
            //    try
            //    {
            //        var response = client.PostAsync("/api/Misc/LogRemoteErrorFile", formDataContent).Result;
            //    }
            //    catch
            //    {
            //        // Ignore error
            //        //throw new HttpRequestException("Remote server unavailable", ex);
            //    }
            //}
        }
    }
}