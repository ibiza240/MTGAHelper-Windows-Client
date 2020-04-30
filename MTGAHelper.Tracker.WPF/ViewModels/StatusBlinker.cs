using System;
using System.Linq;
using System.Threading.Tasks;

namespace MTGAHelper.Tracker.WPF.ViewModels
{
    public class StatusBlinker
    {
        public event Action<object, NetworkStatusEnum> EmitStatus;

        private NetworkStatusEnum FlagsStatus;
        private readonly object LockFlags = new object();

        public StatusBlinker()
        {
            Task.Factory.StartNew(() =>
            {
                while (true)
                {
                    Task.Delay(1000).Wait();

                    NetworkStatusEnum[] activeFlags;
                    lock (LockFlags)
                    {
                        activeFlags = Enum.GetValues(typeof(NetworkStatusEnum)).Cast<NetworkStatusEnum>()
                            .Where(i => FlagsStatus.HasFlag(i))
                            .ToArray();
                    }

                    var idx = 0;
                    foreach (var f in activeFlags)
                    {
                        idx++;
                        EmitStatus?.Invoke(this, f);
                        if (idx < activeFlags.Length)
                            Task.Delay(1000).Wait();
                    }
                }
            });
        }

        public void SetNetworkStatus(NetworkStatusEnum flag, bool isActive)
        {
            lock (LockFlags)
            {
                if (isActive)
                    FlagsStatus |= flag;  // Set flag
                else
                    FlagsStatus &= ~flag;  // Remove flag
            }
        }

        public bool HasFlag(NetworkStatusEnum flag)
        {
            lock (LockFlags)
                return FlagsStatus.HasFlag(flag);
        }

        internal NetworkStatusEnum GetFlags()
        {
            lock (LockFlags)
                return FlagsStatus;
            //return Enum.GetValues(typeof(TrackerStatusEnum)).Cast<TrackerStatusEnum>()
            //    .Where(i => flagsStatus.HasFlag(i))
            //    .ToArray();
        }
    }
}
