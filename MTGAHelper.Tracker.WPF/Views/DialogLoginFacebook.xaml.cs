using System;
using System.Globalization;
using System.Web;
using MTGAHelper.Tracker.WPF.Config;

namespace MTGAHelper.Tracker.WPF.Views
{
    /// <summary>
    /// Interaction logic for DialogLoginFacebook.xaml
    /// </summary>
    public partial class DialogLoginFacebook
    {
        protected const string Server = DebugOrRelease.Server;

        private readonly string PAppId;
        private readonly string PScopes;
        private string PAccessToken;
        private DateTime PTokenExpires;
        private string PGrantedScopes;
        private string PDeniedScopes;
        private string PError;
        private string PErrorReason;
        private string PErrorDescription;
        public bool? result;

        public string AccessToken { get { return PAccessToken; } }
        public DateTime TokenExpires { get { return PTokenExpires; } }
        public string GrantedScopes { get { return PGrantedScopes; } }
        public string DeniedScopes { get { return PDeniedScopes; } }
        public string Error { get { return PError; } }
        public string ErrorReason { get { return PErrorReason; } }
        public string ErrorDescription { get { return PErrorDescription; } }

        /// <summary>
        /// Creates a new login dialog for Facebook
        /// </summary>
        /// <param name="inpAppId">ID of the app authenticating against Facebook</param>
        /// <param name="inpScopes">A comma seperated list of scoopes that the app will ask permission for</param>
        public DialogLoginFacebook(string inpAppId, string inpScopes)
        {
            PAppId = inpAppId;
            PScopes = inpScopes;
            InitializeComponent();

            string returnUrl = HttpUtility.UrlEncode("https://www.facebook.com/connect/login_success.html");
            string scopes = HttpUtility.UrlEncode(PScopes);
            string urlLogin = $"https://www.facebook.com/dialog/oauth?client_id={PAppId}&redirect_uri={returnUrl}&response_type=token%2Cgranted_scopes&scope={scopes}&display=popup";
            FBwebBrowser.Navigate(urlLogin);
        }

        private void ExtractUrlInfo(string inpTrimChar, string urlInfo)
        {
            string fragments = urlInfo.Trim(char.Parse(inpTrimChar)); // Trim the hash or the ? mark
            var parameters = fragments.Split(char.Parse("&")); // Split the url fragments / query string 

            // Extract info from url
            foreach (string parameter in parameters)
            {
                var nameValue = parameter.Split(char.Parse("=")); // Split the input

                switch (nameValue[0])
                {
                    case "access_token":
                        PAccessToken = nameValue[1];
                        break;
                    case "expires_in":
                        PTokenExpires =
                            double.TryParse(nameValue[1], NumberStyles.AllowDecimalPoint, CultureInfo.InvariantCulture,
                                out double expires)
                                ? DateTime.Now.AddSeconds(expires)
                                : DateTime.Now;
                        break;
                    case "granted_scopes":
                        PGrantedScopes = HttpUtility.UrlDecode(nameValue[1]);
                        break;
                    case "denied_scopes":
                        PDeniedScopes = HttpUtility.UrlDecode(nameValue[1]);
                        break;
                    case "error":
                        PError = HttpUtility.UrlDecode(nameValue[1]);
                        break;
                    case "error_reason":
                        PErrorReason = HttpUtility.UrlDecode(nameValue[1]);
                        break;
                    case "error_description":
                        PErrorDescription = HttpUtility.UrlDecode(nameValue[1]);
                        break;
                }
            }
        }

        private void WebBrowser_Navigated(object sender, System.Windows.Navigation.NavigationEventArgs e)
        {
            // Check to see if we hit return url
            if (FBwebBrowser.Source.AbsoluteUri.Contains("/connect/login_success.html"))
            {
                // Check for error
                if (FBwebBrowser.Source.Query.Contains("error"))
                {
                    // Error detected
                    result = false;
                    ExtractUrlInfo("?", FBwebBrowser.Source.Query);
                }
                else
                {
                    result = true;
                    ExtractUrlInfo("#", FBwebBrowser.Source.Fragment);
                }
                // Close the dialog
                Close();
            }
        }
    }
}
