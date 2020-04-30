using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Text;
using MTGAHelper.Entity;
using MTGAHelper.Lib.IO.Reader.MtgaOutputLog;
using Newtonsoft.Json;

namespace MTGAHelper.ConsoleSync.Services
{
    public class CardsResponse
    {
        public ICollection<Card> cards { get; set; }
    }

    public class PostOutputLogProcessedRequest
    {
        public PostOutputLogProcessedRequest()
        {
        }

        public PostOutputLogProcessedRequest(OutputLogResult result)
        {
            OutputLogResult = result;
        }

        public OutputLogResult OutputLogResult { get; set; }
    }

    public class ServerApiCaller
    {
        readonly string baseAddress = "https://mtgahelper.com";
        //readonly string baseAddress = "https://localhost:5001";

        void PostResponseSimple(string apiEndpoint, string userId, object body)
        {
            var cookieContainer = new CookieContainer();
            cookieContainer.SetCookies(new Uri(baseAddress), "userId=" + userId);
            var handler = new HttpClientHandler() { CookieContainer = cookieContainer };
            var client = new HttpClient(handler) { BaseAddress = new Uri(baseAddress) };

            {
                try
                {
                    var response = client.PostAsync(baseAddress + apiEndpoint, new StringContent(JsonConvert.SerializeObject(body), Encoding.UTF8, "application/json")).Result;
                    response.EnsureSuccessStatusCode();
                    var strResponse = response.Content.ReadAsStringAsync().Result;
                }
                catch (AggregateException ex)
                {
                    throw new HttpRequestException("Remote server unavailable", ex);
                }
            }
        }

        T GetResponseSimple<T>(string apiEndpoint)
        {
            var handler = new HttpClientHandler() { };
            var client = new HttpClient(handler) { BaseAddress = new Uri(baseAddress) };

            {
                try
                {
                    var response = client.GetAsync(baseAddress + apiEndpoint).Result;
                    response.EnsureSuccessStatusCode();
                    var strResponse = response.Content.ReadAsStringAsync().Result;
                    var data = JsonConvert.DeserializeObject<T>(strResponse);
                    return data;
                }
                catch (AggregateException ex)
                {
                    throw new HttpRequestException("Remote server unavailable", ex);
                }
            }
        }

        internal void UploadOutputLogResult(string userId, OutputLogResult result)
        {
            PostResponseSimple("/api/User/LogFileProcessed", userId, new PostOutputLogProcessedRequest(result));
        }

        internal ICollection<Card> GetCards()
        {
            var data = GetResponseSimple<CardsResponse>("/api/Misc/Cards");
            return data.cards;
        }
    }
}