using Serilog;
using System.Diagnostics;

namespace MTGAHelper.Lib.Logging
{
    public static class LogExt
    {
        public static void LogReadFile(string filePath, string userId = "null")
        {
            var callerMethod = new StackFrame(1, true).GetMethod();
            var context = $"{callerMethod.DeclaringType.Name}.{callerMethod.Name}";
            Log.Information("{logExt} {filePath} from [{context}] (user: {userId})", "Reading file", filePath, context, userId);
        }
    }
}
