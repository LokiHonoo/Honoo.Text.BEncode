using Honoo.Text;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace Honoo.TorrentAnalysis
{
    internal class Program
    {
        #region Main

        private static void Main(string[] args)
        {
            string torrentFile = "aidmArchive.torrent";
            string search = "RPG";
            string outputFile = "search.csv";

            var dicts = ExportFileNames(torrentFile, search);
            if (dicts.Count > 0)
            {
                var csv = new StringBuilder();
                foreach (var en in dicts)
                {
                    csv.AppendLine(en.Value);
                }
                File.WriteAllText(outputFile, csv.ToString(), Encoding.UTF8);
                Console.WriteLine("Output file saved.");
            }
            else
            {
                Console.WriteLine("Nothing search.");
            }
            Console.WriteLine();
            Console.ReadKey(true);
        }

        #endregion Main

        private static Dictionary<string, string> ExportFileNames(string torrentFile, string search)
        {
            var result = new Dictionary<string, string>();
            using (var stream = new FileStream(torrentFile, FileMode.Open, FileAccess.Read))
            {
                var torrent = new BEncodeDictionary(stream);
                torrent.TryGetValue("info", out BEncodeDictionary info);
                info.TryGetValue("files", out BEncodeList files);
                foreach (BEncodeDictionary file in files.Cast<BEncodeDictionary>())
                {
                    var pathEntities = (BEncodeList)file["path"];
                    var key = ((BEncodeSingle)pathEntities[0]).GetStringValue();
                    var fn = ((BEncodeSingle)pathEntities[^1]).GetStringValue();
                    if (fn.Contains(search))
                    {
                        if (!result.TryGetValue(key, out _))
                        {
                            result.Add(key, key + "," + fn );
                        }
                    }
                }
                foreach (BEncodeDictionary file in files.Cast<BEncodeDictionary>())
                {
                    var pathEntities = (BEncodeList)file["path"];
                    var key = ((BEncodeSingle)pathEntities[0]).GetStringValue();
                    if (result.TryGetValue(key, out string value))
                    {
                        var length = (BEncodeInteger)file["length"];
                        var len = length.GetInt64Value();
                        result[key] = value + "," + len + "," + Honoo.Integer.GetSize(len, Integer.SizeRadix2.GiB, 2, out _) + " GiB";
                    }
                }
            }
            return result;
        }
    }
}