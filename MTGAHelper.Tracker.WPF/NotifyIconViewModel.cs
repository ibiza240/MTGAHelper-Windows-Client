using MTGAHelper.Tracker.WPF.Views;
using System;
using System.Windows;
using System.Windows.Input;

namespace MTGAHelper.Tracker.WPF
{
    /// <summary>
    /// Provides bindable properties and commands for the NotifyIcon. In this sample, the
    /// view model is assigned to the NotifyIcon in XAML. Alternatively, the startup routing
    /// in App.xaml.cs could have created this view model, and assigned it to the NotifyIcon.
    /// </summary>
    public class NotifyIconViewModel
    {
        public ICommand ShowWindowCommand => new DelegateCommand
        {
            CanExecuteFunc = () => true,
            CommandAction = () =>
            {
                Window.Visibility = Visibility.Visible;
                Window.Activate();
            }
        };

        public ICommand ExitApplicationCommand => new DelegateCommand { CommandAction = () => Application.Current.Shutdown() };

        public ICommand MaximizeWindow => new DelegateCommand
        {
            CommandAction = () =>
            {
                Window.Visibility = Visibility.Visible;
                Window.WindowState = WindowState.Maximized;
                Window.Activate();
            }
        };

        public MainWindow Window { get; internal set; }
    }

    /// <summary>
    /// Simplistic delegate command for the demo.
    /// </summary>
    public class DelegateCommand : ICommand
    {
        public Action CommandAction { get; set; }
        public Func<bool> CanExecuteFunc { get; set; }

        public void Execute(object parameter)
        {
            CommandAction();
        }

        public bool CanExecute(object parameter)
        {
            return CanExecuteFunc == null || CanExecuteFunc();
        }

        public event EventHandler CanExecuteChanged
        {
            add { CommandManager.RequerySuggested += value; }
            remove { CommandManager.RequerySuggested -= value; }
        }
    }
}
