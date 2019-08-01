using MTGAHelper.Tracker.WPF.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MTGAHelper.Tracker.WPF.ViewModels
{
    public class StatusBlinker
    {
        public event Action<object, NetworkStatusEnum> EmitStatus;

        NetworkStatusEnum flagsStatus;
        object lockFlags = new object();

        public StatusBlinker()
        {
            Task.Factory.StartNew(() =>
            {
                while (true)
                {
                    Task.Delay(1000).Wait();

                    var activeFlags = new NetworkStatusEnum[0];
                    lock(lockFlags)
                    {
                        activeFlags = Enum.GetValues(typeof(NetworkStatusEnum)).Cast<NetworkStatusEnum>()
                            .Where(i => flagsStatus.HasFlag(i))
                            .ToArray();
                    }

                    foreach (var f in activeFlags)
                    {
                        EmitStatus?.Invoke(this, f);
                        Task.Delay(1000).Wait();
                    }
                }
            });
        }

        public void SetNetworkStatus(NetworkStatusEnum flag, bool isActive)
        {
            lock(lockFlags)
            {
            if (isActive)
                flagsStatus |= flag;  // Set flag
            else
                flagsStatus &= ~flag;  // Remove flag
            }
        }

        public bool HasFlag(NetworkStatusEnum flag)
        {
            lock (lockFlags)
                return flagsStatus.HasFlag(flag);
        }

        internal NetworkStatusEnum GetFlags()
        {
            lock (lockFlags)
                return flagsStatus;
                //return Enum.GetValues(typeof(TrackerStatusEnum)).Cast<TrackerStatusEnum>()
                //    .Where(i => flagsStatus.HasFlag(i))
                //    .ToArray();
        }
    }
}
