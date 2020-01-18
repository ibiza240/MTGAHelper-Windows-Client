using Hardcodet.Wpf.TaskbarNotification;
using MTGAHelper.Tracker.WPF.Views;
using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Markup;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace MTGAHelper.Tracker.WPF
{
    public class NotifyIconManager
    {
        TaskbarIcon notifyIcon;
        NotifyIconViewModel vm;

        public void AddNotifyIcon(MainWindow mainWindow)
        {
            vm = new NotifyIconViewModel { Window = mainWindow };
            notifyIcon = new TaskbarIcon
            {
                ContextMenu = new ContextMenu
                {
                    Background = new SolidColorBrush(Color.FromRgb(0x27, 0x2b, 0x30)),
                    ItemsPanel = XamlReader.Parse("<ItemsPanelTemplate xmlns='http://schemas.microsoft.com/winfx/2006/xaml/presentation' xmlns:x='http://schemas.microsoft.com/winfx/2006/xaml'><StackPanel Margin='-30,0,0,0' Background='#272b30' /></ItemsPanelTemplate>") as ItemsPanelTemplate
                },
                IconSource = new BitmapImage(new Uri("pack://application:,,,/Assets/Images/wcC.ico")),
                //ToolTipText = "Click for window, right-click for menu",
            };
            notifyIcon.ContextMenu.Items.Add(new MenuItem
            {
                Header = "Maximize",
                Command = vm.MaximizeWindow,
            });
            notifyIcon.ContextMenu.Items.Add(new MenuItem
            {
                Header = "Exit",
                Command = vm.ExitApplicationCommand,
            });
            notifyIcon.LeftClickCommand = vm.ShowWindowCommand;
            notifyIcon.DataContext = vm;
        }

        public void RemoveNotifyIcon()
        {
            if (notifyIcon != null)
            {
                notifyIcon.Visibility = Visibility.Hidden;
                notifyIcon.Dispose();
            }
        }

        public void ShowWindowFromTray()
        {
            vm.ShowWindowCommand.Execute(null);
        }
    }
}
