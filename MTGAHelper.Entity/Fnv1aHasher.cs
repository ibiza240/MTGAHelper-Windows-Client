using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MTGAHelper.Entity
{
    // IMPROVE: get this out of Entity eventually, when DeckBase doesn't need it for Id generation anymore
    public static class Fnv1aHasher
    {
        const uint FnvPrime32 = 16777619;
        const uint FnvOffset32 = 2166136261;
        //public static readonly ulong FnvPrime64 = 1099511628211;
        //public static readonly ulong FnvOffset64 = 14695981039346656037;
        public static uint To32BitFnv1aHash(string toHash, bool separateUpperByte = false)
        {
            if (toHash == null)
                return 0;

            //unchecked
            {
                IEnumerable<byte> bytesToHash;

                if (separateUpperByte)
                    bytesToHash = toHash.ToCharArray()
                        .Select(c => new[] { (byte)((c - (byte)c) >> 8), (byte)c })
                        .SelectMany(c => c);
                else
                    bytesToHash = toHash.ToCharArray()
                        .Select(i => Encoding.ASCII.GetBytes(i.ToString())[0]);

                //this is the actual hash function; very simple
                uint hash = FnvOffset32;

                foreach (var chunk in bytesToHash)
                {
                    hash ^= chunk;
                    hash *= FnvPrime32;
                }

                return hash;
            }
        }
    }
}
