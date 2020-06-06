using System;
using System.Collections.Generic;
using System.Linq;
using MTGAHelper.Lib.OutputLogParser.Exceptions;
using MTGAHelper.Lib.OutputLogParser.Models;
using Newtonsoft.Json;
using Serilog;

namespace MTGAHelper.Lib.OutputLogParser.Readers
{
    public abstract class ConverterBase<T, TRaw> : IReaderMtgaOutputLogJson<T>, ILogMessageReader where T : MtgaOutputLogPartResultBase<TRaw>
    {
        public abstract string LogTextKey { get; }
        public bool IsJson { get; protected set; } = true;

        protected abstract T CreateT(TRaw raw);

        protected virtual MtgaOutputLogPartResultBase<TRaw> ParseJsonTyped(string json)
        {
            if (json == null)
            {
                Log.Warning("JsonReader {jsonReader} json was NULL!", this.GetType().ToString());
                return null;
            }

            var raw = JsonConvert.DeserializeObject<TRaw>(json);
            return CreateT(raw);
        }

        public virtual IMtgaOutputLogPartResult ParseJson(string json)
        {
            try
            {
                var result = ParseJsonTyped(json);
                if (result == null)
                    return new IgnoredResult();

                return result;
            }
            catch (System.Exception ex)
            {
                throw;
            }
        }

        public virtual bool DoesParse(string part)
        {
            return part.StartsWith(LogTextKey);
        }

        public virtual IEnumerable<IMtgaOutputLogPartResult> ParsePart(string part)
        {
            var json = GetJson(part);
            return new[] { ParseJson(json) };
        }

        string GetJson(string part)
        {
            var subpart = GetSubpart(part);
            var jsonStartArray = subpart.IndexOf("[", StringComparison.Ordinal);
            var jsonStartObject = subpart.IndexOf("{", StringComparison.Ordinal);
            if (jsonStartArray < 0 && jsonStartObject < 0)
                throw new MtgaOutputLogInvalidJsonException("Invalid JSON");

            var lastChar = jsonStartArray != -1 && jsonStartArray < jsonStartObject ? "]" : "}";
            var lastCharIdx = subpart.LastIndexOf(lastChar, StringComparison.Ordinal);
            if (lastCharIdx < 0)
                throw new MtgaOutputLogInvalidJsonException("Invalid JSON");

            var jsonStart = new[] { jsonStartArray, jsonStartObject }.Where(i => i >= 0).Min();

            var json = subpart.Substring(jsonStart, lastCharIdx - jsonStart + 1);
            return json;
        }

        string GetSubpart(string part)
        {
            // Remove everything after this useless debugging log
            var endMaxIndex = part.IndexOf("(Filename:", StringComparison.Ordinal);
            return endMaxIndex == -1 ? part : part.Substring(0, endMaxIndex);
        }
    }
}