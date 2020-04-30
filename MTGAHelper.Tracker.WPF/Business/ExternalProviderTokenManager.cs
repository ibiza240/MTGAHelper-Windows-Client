using Google.Apis.Auth.OAuth2;
using MTGAHelper.Lib.OutputLogParser;
using MTGAHelper.Tracker.WPF.Views;
using Newtonsoft.Json;
using Serilog;
using System;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;

namespace MTGAHelper.Tracker.WPF.Business
{
    public class ExternalProviderTokenManager
    {
        private readonly XPS X = new XPS();

        private UserCredential UserCredential;

        internal async Task<string> GoogleSignin()
        {
            async Task<UserCredential> Signin()
            {
                (string i, string s) = X.G();
                try
                {
                    var credential = await GoogleWebAuthorizationBroker.AuthorizeAsync(
                        new ClientSecrets
                        {
                            ClientId = i,
                            ClientSecret = s
                        },
                        new[] { "email", "openid" },
                        "user",
                        CancellationToken.None);

                    return credential;
                }
                catch (Exception ex)
                {
                    const string msg = "There was a problem authenticating your Google account";
                    MessageBox.Show(msg, "MTGAHelper");
                    Log.Error(ex, msg);
                    return default;
                }
            };

            UserCredential = await Signin();

            //if (credential.Token.IsExpired(credential.Flow.Clock))
            {
                try
                {
                    UserCredential.RefreshTokenAsync(CancellationToken.None).Wait();
                }
                catch (Exception)
                {
                    // Retry in case rights were revoked and user wants to regive them
                    try
                    {
                        UserCredential.RevokeTokenAsync(CancellationToken.None).Wait();
                    }
                    catch (Exception)
                    {
                        // When already revoked, just ignore the error
                    }
                    await Signin();
                }
            }

            return UserCredential.Token.IdToken;
        }

        internal string GoogleRefresh()
        {
            if (UserCredential == null)
                return string.Empty;

            UserCredential.RefreshTokenAsync(CancellationToken.None);

            return UserCredential.Token.IdToken;
        }

        internal string FacebookSignin(Window windowParent)
        {
            (string appId, string appSecret) = X.F();

            var dlg = new DialogLoginFacebook(appId, "email") {Owner = windowParent};

            dlg.ShowDialog();

            string token;

            // Get long-lived token
            string url = $"https://graph.facebook.com/v4.0/oauth/access_token?grant_type=fb_exchange_token&client_id={appId}&client_secret={appSecret}&fb_exchange_token={dlg.AccessToken}";
           
            using (var client = new HttpClient())
            {
                string response = client.GetAsync(url).Result.Content.ReadAsStringAsync().Result;
                token = JsonConvert.DeserializeObject<dynamic>(response).access_token;
            }

            return token;
        }

        //internal string FacebookRefresh()
        //{
        //    throw new NotImplementedException();
        //}
    }
}
