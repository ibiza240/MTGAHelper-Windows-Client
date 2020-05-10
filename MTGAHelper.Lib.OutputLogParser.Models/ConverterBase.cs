using Newtonsoft.Json;
using Serilog;

namespace MTGAHelper.Lib.IO.Reader.MtgaOutputLog
{
    public abstract class ConverterBase<T, TRaw> : IReaderMtgaOutputLogJson<T> where T : MtgaOutputLogPartResultBase<TRaw>
    {
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
    }
}