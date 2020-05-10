//using Microsoft.Extensions.Options;
//using MTGAHelper.Lib.Config;
//using Serilog.Core;
//using Serilog.Events;
//using System.IO;

//namespace MTGAHelper.Lib.Logging
//{
//    internal class AuditByUserEnricher : ILogEventEnricher
//    {
//        ConfigModelApp configApp;

//        public AuditByUserEnricher(IOptionsSnapshot<ConfigModelApp> configApp)
//        {
//            this.configApp = configApp.Value;
//        }

//        public void Enrich(LogEvent logEvent, ILogEventPropertyFactory propertyFactory)
//        {
//            if (logEvent.Properties.ContainsKey("userId"))
//            {
//                string userId = null;
//                using (StringWriter writer = new StringWriter())
//                    logEvent.Properties["userId"].Render(writer);
                
//                var path = Path.Combine(configApp.FolderConfigUsers, "audit", $"{userId}_audit.txt");

//                File.AppendAllLines(path, new [] { logEvent.RenderMessage() });
//            }
//        }
//    }
//}
