using Honoo.Text;
using System;
using System.Collections.Generic;
using System.IO;
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

            ExportFileNames(torrentFile, search, outputFile);
        }

        #endregion Main

        private static void ExportFileNames(string torrentFile, string search, string outputFile)
        {
            StringBuilder result = new StringBuilder();

            using (FileStream stream = new FileStream(torrentFile, FileMode.Open, FileAccess.Read))
            {
                BEncodeDictionary torrent = new BEncodeDictionary(stream);
                torrent.TryGetValue("info", out BEncodeDictionary info);
                if (info.TryGetValue("files", out BEncodeList files))
                {
                    foreach (BEncodeDictionary file in files)
                    {
                        List<string> splits = new List<string>();
                        var path = (BEncodeList)file["path"];
                        foreach (BEncodeSingle df in path)
                        {
                            splits.Add(df.GetStringValue());
                        }
                        var length = (BEncodeInteger)file["length"];
                        string comb = string.Join("\\", splits);
                        if (comb.Contains(search))
                        {
                            result.AppendLine(comb + "," + length.GetInt64Value());
                        }
                    }
                }
                else
                {
                    List<string> splits = new List<string>();
                    var path = (BEncodeList)info["path"];
                    foreach (BEncodeSingle df in path)
                    {
                        splits.Add(df.GetStringValue());
                    }
                    var length = (BEncodeInteger)info["length"];
                    string comb = string.Join("\\", splits);
                    if (comb.Contains(search, StringComparison.InvariantCultureIgnoreCase))
                    {
                        result.AppendLine(comb + "," + length.GetInt64Value());
                    }
                }
            }

            if (result.Length > 0)
            {
                File.WriteAllText(outputFile, result.ToString(), Encoding.UTF8);
                Console.WriteLine("Output file saved.");
            }
            else
            {
                Console.WriteLine("No search files.");
            }
            Console.WriteLine();
            Console.ReadKey(true);
        }
    }
}