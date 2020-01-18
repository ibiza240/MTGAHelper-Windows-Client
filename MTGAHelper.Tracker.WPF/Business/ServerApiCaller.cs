using Microsoft.Extensions.Options;
using MTGAHelper.Lib.IO.Reader.MtgaOutputLog;
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
using WebApplication1.Model.Account;

namespace MTGAHelper.Tracker.WPF.Business
{
    public class ServerApiCaller
    {
#if DEBUG
        protected const string server = "https://localhost:5001";
#else
        protected const string server = "https://mtgahelper.com";
#endif

        protected const string serverTest = "https://localhost:5001";

        readonly Uri baseAddress = new Uri(server);

        readonly CookieContainer cookieContainer;
        readonly HttpClientHandler handler;
        readonly HttpClient client;

        LogFileZipper zipper;
        ConfigModelApp configApp;

        public ServerApiCaller(LogFileZipper zipper, IOptionsMonitor<ConfigModelApp> configApp)
        {
            cookieContainer = new CookieContainer();
            handler = new HttpClientHandler() { CookieContainer = cookieContainer };
            client = new HttpClient(handler) { BaseAddress = baseAddress };
            this.zipper = zipper;
            this.configApp = configApp?.CurrentValue;
        }

        public void SetUserId(string userId)
        {
            cookieContainer.SetCookies(baseAddress, "userId=" + userId);
        }

        //    public AccountResponse Init(AccountResponse account, string password)
        //    {
        //        cookieContainer = new CookieContainer();
        //        handler = new HttpClientHandler() { CookieContainer = cookieContainer };
        //        client = new HttpClient(handler) { BaseAddress = baseAddress };

        //        AccountResponse response = null;

        //        if (account.Provider == null)
        //        {
        //            response = ParseResponseGet<AccountResponse>($"api/account/signin?email={account.Email}&password={password}");
        //        }
        //        else
        //        {
        //UserCredential credential = GoogleWebAuthorizationBroker.AuthorizeAsync(
        //    new ClientSecrets
        //    {
        //        ClientId = "",
        //        ClientSecret = ""
        //    },
        //    new[] { "email", "openid" },
        //    "user",
        //    CancellationToken.None).Result;

        //            if (credential.Token.IssuedUtc.AddSeconds(credential.Token.ExpiresInSeconds ?? 0) < DateTime.UtcNow)
        //            {
        //                credential.RefreshTokenAsync(CancellationToken.None).Wait();
        //            }

        //            var test = GoogleJsonWebSignature.ValidateAsync(credential.Token.IdToken,
        //                new GoogleJsonWebSignature.ValidationSettings { Audience = new []
        //                { "appId" } }).Result;

        //            //// Create the service.
        //            //var service = new BooksService(new BaseClientService.Initializer()
        //            //{
        //            //    HttpClientInitializer = credential,
        //            //    ApplicationName = "Books API Sample",
        //            //});

        //            //credential.Initialize().CreateAuthorizationCodeRequest("https://localhost:5001/api/External/Callback").Build;
        //            //var test = new ConfigurableHttpClient(new ConfigurableMessageHandler(new HttpClientHandler())).

        //            //var bookshelves = await service.Mylibrary.Bookshelves.List().ExecuteAsync();
        //            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", credential.Token.IdToken);

        //            response = ParseResponseGet<AccountResponse>($"External/Callback");
        //        }

        //        cookieContainer.Add(baseAddress, new Cookie("userId", account.MtgaHelperUserId));

        //        return response;
        //    }

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

        //internal TReturn PostResponseWithCookie<TReturn>(string userId, string apiEndpoint, object body)
        //{
        //    var baseAddress = new Uri(server);
        //    var cookieContainer = new CookieContainer();
        //    using (var handler = new HttpClientHandler() { CookieContainer = cookieContainer })
        //    using (var client = new HttpClient(handler) { BaseAddress = baseAddress })
        //    {
        //        cookieContainer.Add(baseAddress, new Cookie("userId", userId));
        //        //var test = JsonConvert.SerializeObject(body);

        //        try
        //        {
        //            var response = client.PostAsync(apiEndpoint, new StringContent(JsonConvert.SerializeObject(body), Encoding.UTF8, "application/json")).Result;
        //            response.EnsureSuccessStatusCode();
        //            var strResponse = response.Content.ReadAsStringAsync().Result;
        //            var parsed = JsonConvert.DeserializeObject<TReturn>(strResponse);
        //            return parsed;
        //        }
        //        catch (AggregateException ex)
        //        {
        //            throw new HttpRequestException("Remote server unavailable", ex);
        //        }
        //    }
        //}

        internal TReturn PostResponseSimple<TReturn>(string apiEndpoint, object body)
        {
            //using (var client = new HttpClient() { BaseAddress = new Uri(server) })
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

        //internal T GetResponseSimple<T>(string endpoint)
        //{
        //    using (var w = new WebClient())
        //    {
        //        var responseRaw = w.DownloadString(server + endpoint);
        //        var parsed = JsonConvert.DeserializeObject<T>(responseRaw);
        //        return parsed;
        //    }
        //}

        T GetResponseWithCookie<T>(string apiEndpoint)
        {
            //var baseAddress = new Uri(server);
            //var cookieContainer = new CookieContainer();
            //using (var handler = new HttpClientHandler() { CookieContainer = cookieContainer })
            //using (var client = new HttpClient(handler) { BaseAddress = baseAddress })
            {
                try
                {
                    var response = client.GetAsync(apiEndpoint).Result;
                    response.EnsureSuccessStatusCode();
                    var strResponse = response.Content.ReadAsStringAsync().Result;

                    if (typeof(T) == typeof(string))
                        return (T)Convert.ChangeType(strResponse, typeof(T));

                    var parsed = JsonConvert.DeserializeObject<T>(strResponse);
                    return parsed;
                }
                catch (AggregateException ex)
                {
                    throw new HttpRequestException("Remote server unavailable", ex);
                }
                catch (Exception ex)
                {
                    Debugger.Break();
                    return default(T);
                }
            }
        }

        internal string GetAccountSalt(string signinEmail)
        {
            return GetResponseWithCookie<string>("api/WpfLogin/AccountSalt");
        }

        //T ParseResponseGet<T>(string apiEndpoint)
        //{
        //    try
        //    {
        //        //var baseAddress = new Uri(server);
        //        //var cookieContainer = new CookieContainer();
        //        //using (var handler = new HttpClientHandler() { CookieContainer = cookieContainer })
        //        //using (var client = new HttpClient(handler) { BaseAddress = baseAddress })
        //        {
        //            //cookieContainer.Add(baseAddress, new Cookie("userId", userId));

        //            try
        //            {
        //                var response = client.GetAsync(apiEndpoint).Result;

        //                var testCookiesResponse = cookieContainer.GetCookies(new Uri(baseAddress + apiEndpoint)).Cast<Cookie>();

        //                response.EnsureSuccessStatusCode();
        //                var strResponse = response.Content.ReadAsStringAsync().Result;
        //                var parsed = JsonConvert.DeserializeObject<T>(strResponse);
        //                return parsed;
        //            }
        //            catch (AggregateException ex)
        //            {
        //                throw new HttpRequestException("Remote server unavailable", ex);
        //            }
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        Debugger.Break();
        //        return default(T);
        //    }
        //}

        //internal bool IsLocalTrackerUpToDate()
        //{
        //    try
        //    {
        //        System.Reflection.Assembly assembly = System.Reflection.Assembly.GetExecutingAssembly();
        //        FileVersionInfo fvi = FileVersionInfo.GetVersionInfo(assembly.Location);
        //        using (var w = new WebClient())
        //        {
        //            var responseRaw = w.DownloadString((configApp.Test ? serverTest : server) + "/api/Misc/VersionTracker");
        //            var latestVersion = JsonConvert.DeserializeObject<GetVersionTrackerResponse>(responseRaw).Version;
        //            return new Version(fvi.FileVersion) >= new Version(latestVersion);
        //        }
        //    }
        //    catch (WebException)
        //    {
        //        // Ignore error on call
        //        return true;
        //    }
        //}

        internal DraftRaredraftingInfoResponse GetRaredraftingInfo(string mtgaHelperUserId)
        {
            return GetResponseWithCookie<DraftRaredraftingInfoResponse>("api/User/Compare");
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
        //        cookieContainer.Add(baseAddress, new Cookie("userId", userId));
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
            return PostResponseSimple<CollectionResponse>("/api/User/LogFileProcessed", new PostOutputLogProcessedRequest(result));
        }

        internal bool IsSameLastUploadHash(string userId, uint uploadHash)
        {
            var latestUploadHash = GetResponseWithCookie<LastHashResponse>("/api/User/LastUploadHash").LastHash;
            return latestUploadHash == uploadHash.ToString();
        }

        //internal GetCardsForDraftPickResponse GetCardsForDraftPick(string userId, ICollection<int> grpIds, string source)
        //{
        //    if (grpIds == null || grpIds.Count == 0)
        //        return new GetCardsForDraftPickResponse(new Entity.CardForDraftPick[0]);

        //    return GetResponseWithCookie<GetCardsForDraftPickResponse>($"/api/User/DraftPick?grpIds={string.Join(',', grpIds)}&source={HttpUtility.UrlEncode(source)}");

        //}

        internal CollectionResponse GetCollection(string userId)
        {
            //return GetResponseWithCookie<CollectionResponse>(userId, "/api/User/Collection");
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