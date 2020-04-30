using MTGAHelper.Tracker.WPF.Business;
using System.Windows;
using System.Windows.Input;

namespace MTGAHelper.Tracker.WPF.Views.UserControls
{
    /// <summary>
    /// Interaction logic for Welcome.xaml
    /// </summary>
    public partial class Welcome
    {
        private ExternalProviderTokenManager TokenManager;

        public Welcome()
        {
            InitializeComponent();
        }

        internal void Init(ExternalProviderTokenManager tokenManager)
        {
            TokenManager = tokenManager;
        }

        private async void SigninGoogle_Click(object sender, RoutedEventArgs e)
        {
            string token = await TokenManager.GoogleSignin();

            var mainWindow = (MainWindow)Window.GetWindow(this);

            mainWindow?.ValidateExternalToken("Google", token);
        }

        private void SigninFacebook_Click(object sender, RoutedEventArgs e)
        {
            var mainWindow = (MainWindow)Window.GetWindow(this);

            string token = TokenManager.FacebookSignin(mainWindow);

            mainWindow?.ValidateExternalToken("Facebook", token);
        }

        internal void SetRememberEmail()
        {
            RememberEmailCheckBox.IsChecked = true;
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
            var mainWindow = (MainWindow) Window.GetWindow(this);

            mainWindow?.ValidateLocalUser(PasswordBox.Password, RememberEmailCheckBox.IsChecked == true,
                RememberPasswordCheckBox.IsChecked == true);
        }
    }
}
