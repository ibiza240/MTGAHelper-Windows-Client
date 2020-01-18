using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace MTGAHelper.Tracker.WPF.ViewModels
{
    public abstract class ObservableObject : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        protected void RaisePropertyChangedEvent([CallerMemberName]string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }

    public class ObservableProperty<T> : ObservableObject
    {
        T value;
        public T Value
        {
            get => value;
            set { this.value = value; RaisePropertyChangedEvent(nameof(Value)); }
        }

        public ObservableProperty(T value)
        {
            Value = value;
        }
    }
}
