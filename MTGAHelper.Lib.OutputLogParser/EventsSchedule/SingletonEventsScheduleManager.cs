using MTGAHelper.Entity.Config.App;
using MTGAHelper.Lib.Logging;
using MTGAHelper.Lib.OutputLogParser.Models.UnityCrossThreadLogger.GetActiveEventsV3;
using Newtonsoft.Json;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace MTGAHelper.Lib.OutputLogParser.EventsSchedule
{
    public interface IEventTypeCache
    {
        IReadOnlyCollection<GetActiveEventsV3Raw> AddEvents(ICollection<GetActiveEventsV3Raw> currentEvents);

        string GetEventType(string internalEventName);

        ICollection<GetActiveEventsV3Raw> Events { get; }
    }

    public class SimpleEventCache : IEventTypeCache
    {
        private readonly ConcurrentDictionary<string, GetActiveEventsV3Raw> events = new ConcurrentDictionary<string, GetActiveEventsV3Raw>();
        public ICollection<GetActiveEventsV3Raw> Events => events.Values;

        public string GetEventType(string internalEventName) => events.ContainsKey(internalEventName) ? (events[internalEventName].EventType ?? "Unknown") : "Unknown";

        public IReadOnlyCollection<GetActiveEventsV3Raw> AddEvents(ICollection<GetActiveEventsV3Raw> currentEvents)
        {
            var newEvents = currentEvents.Where(i => !events.ContainsKey(i.InternalEventName)).ToArray();

            foreach (var newEvent in newEvents)
            {
                events.TryAdd(newEvent.InternalEventName, newEvent);
            }

            return newEvents;
        }
    }

    public class SingletonEventsScheduleManager : IEventTypeCache
    {
        private readonly object lockWriteFile = new object();
        private readonly IEventTypeCache cache;

        public string GetEventType(string internalEventName) => cache.GetEventType(internalEventName);

        public ICollection<GetActiveEventsV3Raw> Events => cache.Events;

        private readonly string path;

        public SingletonEventsScheduleManager(IEventTypeCache cache, IDataPath folderData)
        {
            this.cache = cache ?? throw new ArgumentNullException(nameof(cache));
            var folder = folderData?.FolderData ?? throw new ArgumentNullException(nameof(folderData));
            path = Path.Combine(folder, "events.json");
        }

        public void LoadEventsFromDisk()
        {
            if (File.Exists(path))
            {
                LogExt.LogReadFile(path);
                var fileText = File.ReadAllText(path);
                ICollection<GetActiveEventsV3Raw> eventCollection;
                try
                {
                    eventCollection = JsonConvert.DeserializeObject<ICollection<GetActiveEventsV3Raw>>(fileText) ?? new GetActiveEventsV3Raw[0];
                }
                catch (Exception)
                {
                    eventCollection = new GetActiveEventsV3Raw[0];
                }
                cache.AddEvents(eventCollection);
            }
        }

        public IReadOnlyCollection<GetActiveEventsV3Raw> AddEvents(ICollection<GetActiveEventsV3Raw> currentEvents)
        {
            var newEvents = cache.AddEvents(currentEvents);
            if (newEvents.Any())
                lock (lockWriteFile) // lock to prevent races on file write. New events are rare so this lock is rarely reached
                    File.WriteAllText(path, JsonConvert.SerializeObject(cache.Events));

            return newEvents;
        }
    }
}