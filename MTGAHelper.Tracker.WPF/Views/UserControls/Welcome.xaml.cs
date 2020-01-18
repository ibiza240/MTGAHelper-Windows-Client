using MTGAHelper.Tracker.WPF.Business;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace MTGAHelper.Tracker.WPF.Views.UserControls
{
    /// <summary>
    /// Interaction logic for Welcome.xaml
    /// </summary>
    public partial class Welcome : UserControl
    {
        ExternalProviderTokenManager tokenManager;

        public Welcome()
        {
            InitializeComponent();
        }

        internal void Init(ExternalProviderTokenManager tokenManager)
        {
            this.tokenManager = tokenManager;
        }

        private async void SigninGoogle_Click(object sender, RoutedEventArgs e)
        {
            var token = await tokenManager.GoogleSignin();
            var mainWindow = (MainWindow)Window.GetWindow(this);
            mainWindow.ValidateExternalToken("Google", token);
        }

        private void SigninFacebook_Click(object sender, RoutedEventArgs e)
        {
            var mainWindow = (MainWindow)Window.GetWindow(this);
            var token = tokenManager.FacebookSignin(mainWindow);

            mainWindow.ValidateExternalToken("Facebook", token);

            //FBDialog

            //string appId = "";
            //string appSecret = "";

            //var client = new FacebookClient
            //{
            //    AppId = appId, // get this from developer.facebook
            //    AppSecret = appSecret, // get this from developer.facebook
            //};

            //dynamic appTokenQueryResponse = client.Get("oauth/access_token"
            //                                            , new
            //                                            {
            //                                                client_id = appId,
            //                                                client_secret = appSecret,
            //                                                grant_type = "client_credentials"
            //                                            });

            //var token = appTokenQueryResponse.access_token;
        }

        internal void SetRememberEmail()
        {
            chkRememberEmail.IsChecked = true;
        }

        private void ButtonLoginLocal_Click(object sender, RoutedEventArgs e)
        {
            LoginLocal();
        }

        private void txtPassword_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
                LoginLocal();
        }

        private void LoginLocal()
        {
            var mainWindow = (MainWindow)Window.GetWindow(this);
            mainWindow.ValidateLocalUser(txtPassword.Password, chkRememberEmail.IsChecked == true, chkRememberPassword.IsChecked == true);
        }
    }
}
