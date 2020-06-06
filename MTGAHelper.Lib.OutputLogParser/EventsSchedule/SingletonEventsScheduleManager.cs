using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using MTGAHelper.Entity.Config.App;
using MTGAHelper.Lib.Logging;
using MTGAHelper.Lib.OutputLogParser.Models.UnityCrossThreadLogger;
using Newtonsoft.Json;

namespace MTGAHelper.Lib.OutputLogParser.EventsSchedule
{
    public interface IEventTypeCache
    {
        IReadOnlyCollection<GetActiveEventsV2Raw> AddEvents(ICollection<GetActiveEventsV2Raw> currentEvents);
        string GetEventType(string internalEventName);
        ICollection<GetActiveEventsV2Raw> Events { get; }
    }

    public class SimpleEventCache : IEventTypeCache
    {
        readonly ConcurrentDictionary<string, GetActiveEventsV2Raw> events = new ConcurrentDictionary<string, GetActiveEventsV2Raw>();
        public ICollection<GetActiveEventsV2Raw> Events => events.Values;

        public string GetEventType(string internalEventName) => events[internalEventName].EventType;

        public IReadOnlyCollection<GetActiveEventsV2Raw> AddEvents(ICollection<GetActiveEventsV2Raw> currentEvents)
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
        readonly object lockWriteFile = new object();
        readonly IEventTypeCache cache;

        public string GetEventType(string internalEventName) => cache.GetEventType(internalEventName);
        public ICollection<GetActiveEventsV2Raw> Events => cache.Events;

        readonly string path;

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
                var eventCollection =
                    JsonConvert.DeserializeObject<ICollection<GetActiveEventsV2Raw>>(File.ReadAllText(path));
                cache.AddEvents(eventCollection);
            }
        }

        public IReadOnlyCollection<GetActiveEventsV2Raw> AddEvents(ICollection<GetActiveEventsV2Raw> currentEvents)
        {
            var newEvents = cache.AddEvents(currentEvents);
            if (newEvents.Any())
                lock (lockWriteFile) // lock to prevent races on file write. New events are rare so this lock is rarely reached
                    File.WriteAllText(path, JsonConvert.SerializeObject(cache.Events));

            return newEvents;
        }
    }
}
