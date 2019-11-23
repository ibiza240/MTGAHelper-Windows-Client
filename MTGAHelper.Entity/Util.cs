using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;

namespace MTGAHelper.Entity
{
    public class Util
    {
        public readonly Regex regexPrefix = new Regex(@"^\[(.*?)\](?:(\d.+? \d.+?(?: (?:A|P)M)?)(?:$|: | [a-zA-Z]))?", RegexOptions.Compiled);

        public string AppFolder =>
            Directory.GetCurrentDirectory();

        public readonly uint FnvPrime32 = 16777619;
        public readonly uint FnvOffset32 = 2166136261;
        //public static readonly ulong FnvPrime64 = 1099511628211;
        //public static readonly ulong FnvOffset64 = 14695981039346656037;
        public uint To32BitFnv1aHash(string toHash, bool separateUpperByte = false)
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

        //private string GetHash(byte[] bytes)
        //{
        //    StringBuilder sb = new StringBuilder();

        //    using (var hash = SHA256.Create())
        //    {
        //        var result = hash.ComputeHash(bytes);

        //        foreach (var b in result)
        //            sb.Append(b.ToString("x2"));
        //    }

        //    return sb.ToString();
        //}

        //public string GetHash(string value)
        //{
        //    //Encoding enc = Encoding.UTF8;
        //    //return GetHash(enc.GetBytes(value));
        //    return GetDeterministicHashCode(value).ToString();
        //}

        public string RemoveInvalidCharacters(string filename)
        {
            var newFilename = filename;
            foreach (char c in Path.GetInvalidFileNameChars())
                newFilename = newFilename.Replace(c.ToString(), "");

            return newFilename;
        }

        //int GetDeterministicHashCode(string str)
        //{
        //    unchecked
        //    {
        //        int hash1 = (5381 << 16) + 5381;
        //        int hash2 = hash1;

        //        for (int i = 0; i < str.Length; i += 2)
        //        {
        //            hash1 = ((hash1 << 5) + hash1) ^ str[i];
        //            if (i == str.Length - 1)
        //                break;
        //            hash2 = ((hash2 << 5) + hash2) ^ str[i + 1];
        //        }

        //        return hash1 + (hash2 * 1566083941);
        //    }
        //}

        public string GetThumbnailUrl(string imageArtUrl)
        {
            if (imageArtUrl == null)
                return "/images/cardArt/thumbnail/NA.png";

            var idWithQueryParams = imageArtUrl.Split(new[] { "/" }, StringSplitOptions.None).Last();
            var id = idWithQueryParams.Split(new[] { "?" }, StringSplitOptions.None).First();
            return "/images/cardArt/thumbnail/" + id;
        }
    }
}
