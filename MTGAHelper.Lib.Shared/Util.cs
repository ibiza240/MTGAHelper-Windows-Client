using System;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using MTGAHelper.Entity;

namespace MTGAHelper.Lib
{
    public class Util
    {
        public readonly Regex regexPrefix = new Regex(@"^\[(.*?)\](?:(\d.+? \d.+?(?: (?:A|P)M)?)(?:$|: | (?=[a-zA-Z])))?(.*)", RegexOptions.Compiled);

        public string AppFolder =>
            Directory.GetCurrentDirectory();

        public uint To32BitFnv1aHash(string toHash, bool separateUpperByte = false) =>
            Fnv1aHasher.To32BitFnv1aHash(toHash, separateUpperByte);

        public string RemoveInvalidCharacters(string filename)
        {
            var newFilename = filename;
            foreach (char c in Path.GetInvalidFileNameChars())
                newFilename = newFilename.Replace(c.ToString(), "");

            return newFilename;
        }

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
