using System;
using System.Globalization;
using System.Web;
using System.Windows;

namespace MTGAHelper.Tracker.WPF.Views
{
    /// <summary>
    /// Interaction logic for DialogLoginFacebook.xaml
    /// </summary>
    public partial class DialogLoginFacebook : Window
    {
#if DEBUG
        protected const string server = "https://localhost:5001";
#else
        protected const string server = "https://mtgahelper.com";
#endif

        private readonly string p_appID;
        private readonly string p_scopes;
        private string p_access_token;
        private DateTime p_token_expires;
        private string p_granted_scopes;
        private string p_denied_scopes;
        private string p_error;
        private string p_error_reason;
        private string p_error_description;
        public bool? result = null;

        public string access_token { get { return p_access_token; } }
        public DateTime token_expires { get { return p_token_expires; } }
        public string granted_scopes { get { return p_granted_scopes; } }
        public string denied_scopes { get { return p_denied_scopes; } }
        public string error { get { return p_error; } }
        public string error_reason { get { return p_error_reason; } }
        public string error_description { get { return p_error_description; } }

        /// <summary>
        /// Creates a new login dialog for Facebook
        /// </summary>
        /// <param name="inpAppID">ID of the app authenticating against Facebook</param>
        /// <param name="inpScopes">A comma seperated list of scoopes that the app will ask permission for</param>
        public DialogLoginFacebook(string inpAppID, string inpScopes)
        {
            p_appID = inpAppID;
            p_scopes = inpScopes;
            InitializeComponent();

            string returnURL = HttpUtility.UrlEncode("https://www.facebook.com/connect/login_success.html");
            string scopes = HttpUtility.UrlEncode(p_scopes);
            var urlLogin = $"https://www.facebook.com/dialog/oauth?client_id={p_appID}&redirect_uri={returnURL}&response_type=token%2Cgranted_scopes&scope={scopes}&display=popup";
            FBwebBrowser.Navigate(urlLogin);
        }

        private void ExtractURLInfo(string inpTrimChar, string urlInfo)
        {
            string fragments = urlInfo.Trim(char.Parse(inpTrimChar)); // Trim the hash or the ? mark
            string[] parameters = fragments.Split(char.Parse("&")); // Split the url fragments / query string 

            // Extract info from url
            foreach (string parameter in parameters)
            {
                string[] name_value = parameter.Split(char.Parse("=")); // Split the input

                switch (name_value[0])
                {
                    case "access_token":
                        this.p_access_token = name_value[1];
                        break;
                    case "expires_in":
                        double expires = 0;
                        if (double.TryParse(name_value[1], NumberStyles.AllowDecimalPoint, CultureInfo.InvariantCulture, out expires))
                        {
                            this.p_token_expires = DateTime.Now.AddSeconds(expires);
                        }
                        else
                        {
                            this.p_token_expires = DateTime.Now;
                        }
                        break;
                    case "granted_scopes":
                        this.p_granted_scopes = HttpUtility.UrlDecode(name_value[1]);
                        break;
                    case "denied_scopes":
                        this.p_denied_scopes = HttpUtility.UrlDecode(name_value[1]);
                        break;
                    case "error":
                        this.p_error = HttpUtility.UrlDecode(name_value[1]);
                        break;
                    case "error_reason":
                        this.p_error_reason = HttpUtility.UrlDecode(name_value[1]);
                        break;
                    case "error_description":
                        this.p_error_description = HttpUtility.UrlDecode(name_value[1]);
                        break;
                    default:
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
                    this.result = false;
                    ExtractURLInfo("?", FBwebBrowser.Source.Query);
                }
                else
                {
                    this.result = true;
                    ExtractURLInfo("#", FBwebBrowser.Source.Fragment);
                }
                // Close the dialog
                this.Close();
            }
        }
    }
}
