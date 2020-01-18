using System;
using System.Windows;
using System.Windows.Input;

namespace MTGAHelper.Tracker.WPF.Views
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow
    {
        async void Window_Loaded(object sender, RoutedEventArgs e)
        {
            //SetSignedIn(new Web.Models.Response.Account.AccountResponse { Provider = "Google" }, null);
            Height = Math.Min(Height, SystemParameters.PrimaryScreenHeight - 32);

            UpdateCardPopupPosition();

            //ServerApiGetCollection();

            if (string.IsNullOrWhiteSpace(configApp.SigninProvider) == false)
            {
                string token = null;
                switch (configApp.SigninProvider)
                {
                    case "Google":
                        token = await tokenManager.GoogleSignin();
                        break;
                    case "Facebook":
                        token = tokenManager.FacebookSignin(this);
                        break;
                }
                ValidateExternalToken(configApp.SigninProvider, token);
            }
            else if (string.IsNullOrWhiteSpace(configApp.SigninEmail) == false)
            {
                if (string.IsNullOrWhiteSpace(configApp.SigninPassword) == false)
                {
                    var info = api.AutoSigninLocalUser(configApp.SigninEmail, configApp.SigninPassword);
                    if (info == null)
                    {
                        MessageBox.Show("Cannot auto-signin the local account", "MTGAHelper");
                    }
                    else if (info.IsAuthenticated)
                    {
                        SetSignedIn(info);
                    }
                }
                else
                {
                    vm.SigninEmail.Value = configApp.SigninEmail;
                    ucWelcome.SetRememberEmail();
                }
            }
        }

        void Window_Activated(object sender, EventArgs e)
        {
            Topmost = vm.AlwaysOnTop.Value;
            //if (vm.AlwaysOnTop)
            //{
            //    //Top = 0;
            //    //Left = 0;
            //}
        }

        void Window_Deactivated(object sender, EventArgs e)
        {
            Topmost = vm.AlwaysOnTop.Value;
            //if (vm.AlwaysOnTop)
            //{
            //    //Top = 0;
            //    //Left = 0;
            //    Activate();
            //}
        }

        void Window_Closed(object sender, EventArgs e)
        {
            //trayIcon.Visible = false;
            notifyIconManager.RemoveNotifyIcon();
            Application.Current.Shutdown();
        }

        void StatusBarTop_OnMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            base.OnMouseLeftButtonDown(e);

            if (this.WindowState == WindowState.Maximized)
            {
                var mousePos = PointToScreen(Mouse.GetPosition(this));
                this.Top = 0;
                this.Left = mousePos.X - 20;
                this.WindowState = WindowState.Normal;
            }

            // Begin dragging the window
            this.DragMove();
        }

        //Storyboard storyboardHighlightCard = new Storyboard();
        //private void InitStoryBoard()
        //{
        //    // Create a NameScope for the page so
        //    // that Storyboards can be used.
        //    NameScope.SetNameScope(this, new NameScope());

        //    // Background="#272b30"
        //    var animatedBrush = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#272b30"));
        //    this.Background = animatedBrush;

        //    // Register the brush's name with the page
        //    // so that it can be targeted by storyboards.
        //    this.RegisterName("MyAnimatedBrush", animatedBrush);

        //    ColorAnimation highlightCardColorAnimation = new ColorAnimation();
        //    highlightCardColorAnimation.From = Colors.Blue;
        //    //highlightCardColorAnimation.To = Colors.Red;
        //    highlightCardColorAnimation.Duration = TimeSpan.FromSeconds(1);
        //    Storyboard.SetTargetName(highlightCardColorAnimation, "MyAnimatedBrush");
        //    Storyboard.SetTargetProperty(highlightCardColorAnimation, new PropertyPath(SolidColorBrush.ColorProperty));
        //    storyboardHighlightCard.Children.Add(highlightCardColorAnimation);

        //    storyboardHighlightCard.Begin(this);
        //}

        void Slider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            vm.RefreshOpacity();
        }

        private void Window_LocationChanged(object sender, EventArgs e)
        {
            UpdateCardPopupPosition();
        }

        private void Window_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            UpdateCardPopupPosition();
        }

        public void UpdateCardPopupPosition()
        {
            if (double.IsNaN(this.Top) || double.IsNaN(this.Left) ||
                double.IsNaN(this.Width) || double.IsNaN(this.Height))
                return;

            var top = (int)this.Top;
            var left = (int)this.Left;
            var width = (int)this.Width;
            var side = GetCardPopupSide();

            ucDraftHelper.SetCardPopupPosition(side, top, left, width);
            ucPlaying.SetCardPopupPosition(side, top, left, width);
        }

        public ForceCardPopupSideEnum GetCardPopupSide()
        {
            ForceCardPopupSideEnum side = ForceCardPopupSideEnum.None;
            if (configApp.ForceCardPopup)
            {
                side = configApp.ForceCardPopupSide.Contains("left") ? ForceCardPopupSideEnum.Left : ForceCardPopupSideEnum.Right;
            }

            return side;
        }
    }
}
