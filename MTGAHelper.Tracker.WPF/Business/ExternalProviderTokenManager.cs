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
        readonly XPS x = new XPS();
        UserCredential userCredential;

        async internal Task<string> GoogleSignin()
        {
            async Task<UserCredential> Signin()
            {
                var s = x.G();
                try
                {
                    UserCredential credential = await GoogleWebAuthorizationBroker.AuthorizeAsync(
                        new ClientSecrets
                        {
                            ClientId = s.i,
                            ClientSecret = s.s
                        },
                        new[] { "email", "openid" },
                        "user",
                        CancellationToken.None);

                    return credential;
                }
                catch (Exception ex)
                {
                    var msg = "There was a problem authenticating your Google account";
                    MessageBox.Show(msg, "MTGAHelper");
                    Log.Error(ex, msg);
                    return default(UserCredential);
                }
            };

            userCredential = await Signin();

            //if (credential.Token.IsExpired(credential.Flow.Clock))
            {
                try
                {
                    userCredential.RefreshTokenAsync(CancellationToken.None).Wait();
                }
                catch (Exception)
                {
                    // Retry in case rights were revoked and user wants to regive them
                    try
                    {
                        userCredential.RevokeTokenAsync(CancellationToken.None).Wait();
                    }
                    catch (Exception ex)
                    {
                        // When already revoked, just ignore the error
                    }
                    await Signin();
                }
            }

            return userCredential.Token.IdToken;
        }

        internal string GoogleRefresh()
        {
            userCredential.RefreshTokenAsync(CancellationToken.None);
            return userCredential.Token.IdToken;
        }

        internal string FacebookSignin(Window windowParent)
        {
            var (appId, appSecret) = x.F();
            var dlg = new DialogLoginFacebook(appId, "email");
            dlg.Owner = windowParent;
            dlg.ShowDialog();
            var token = dlg.access_token;

            // Get long-lived token
            var url = $"https://graph.facebook.com/v4.0/oauth/access_token?grant_type=fb_exchange_token&client_id={appId}&client_secret={appSecret}&fb_exchange_token={dlg.access_token}";
            using (var client = new HttpClient())
            {
                var response = client.GetAsync(url).Result.Content.ReadAsStringAsync().Result;
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
