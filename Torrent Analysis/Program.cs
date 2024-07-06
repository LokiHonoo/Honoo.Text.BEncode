using Honoo.IO;
using System;
using System.IO;
using System.Text;

namespace Honoo.TorrentAnalysis
{
    internal class Program
    {
        #region Main

        private static void Main()
        {
            string fileName = "[kisssub.org][VCB-Studio] 街角魔族 二丁目  Machikado Mazoku 2-Choume  まちカドまぞく 2丁目 [S2 Fin].torrent";
            using (var stream = new FileStream(fileName, FileMode.Open, FileAccess.Read))
            {
                var torrent = new Honoo.Text.BEncode.TorrentAnalysis(stream);
                string encodingString = torrent.GetEncoding();
                Encoding encoding = Encoding.UTF8;
                if (!string.IsNullOrEmpty(encodingString))
                {
                    try
                    {
                        encoding = Encoding.GetEncoding(encodingString);
                    }
                    catch
                    {
                        try
                        {
                            encoding = Encoding.GetEncoding(int.Parse(encodingString));
                        }
                        catch { }
                    }
                }
                Console.WriteLine(torrent.GetCreatedBy());
                Console.WriteLine(torrent.GetCreationDate());
                Console.WriteLine(torrent.GetAnnounce());
                Console.WriteLine(torrent.GetName());
                Console.WriteLine(torrent.GetPieceLength());
                Console.WriteLine(torrent.GetComment());

                var files = torrent.SearchFiles(encoding, "", 0, long.MaxValue);
                files.Sort((x, y) => { return x.Length < y.Length ? 1 : -1; });
                foreach (var file in files)
                {
                    Console.WriteLine(file.Path[file.Path.Length - 1] + "     " + Numeric.GetSize(file.Length, Numeric.SizeKilo.Auto, 2, out string unit) + unit);
                }
                var magnet = torrent.ComputeMagnet();
                Console.WriteLine(magnet);
            }
            Console.WriteLine();
            Console.ReadKey(true);
        }

        #endregion Main
    }
}