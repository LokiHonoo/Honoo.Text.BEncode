﻿using System;
using System.IO;
using System.Text;

namespace Honoo.TorrentAnalysis
{
    internal class Program
    {
        #region Main

        private static void Main(string[] args)
        {
            string fileName = "[kisssub.org][VCB-Studio] 街角魔族 二丁目  Machikado Mazoku 2-Choume  まちカドまぞく 2丁目 [S2 Fin].torrent";
            using (var stream = new FileStream(fileName, FileMode.Open, FileAccess.Read))
            {
                var torrent = new Honoo.Text.BEncode.TorrentAnalysis(stream);
                Console.WriteLine(torrent.GetCreatedBy());
                Console.WriteLine(torrent.GetCreationDate());
                Console.WriteLine(torrent.GetAnnounce());
                Console.WriteLine(torrent.GetName());
                Console.WriteLine(torrent.GetPieceLength());
                Console.WriteLine(torrent.GetComment());
                var files = torrent.GetFiles(Encoding.UTF8, "", 0, long.MaxValue);
                files.Sort((x, y) => { return x.Length < y.Length ? 1 : -1; });
                foreach (var file in files)
                {
                    Console.WriteLine(file.Path[^1] + "     " + Honoo.Numeric.GetSize(file.Length, Numeric.Size1024.Auto, 2, out string unit) + unit);
                }
                var magnet = torrent.GetMagnet(Encoding.UTF8, true, true, true);
                Console.WriteLine(magnet);
            }
            Console.WriteLine();
            Console.ReadKey(true);
        }

        #endregion Main
    }
}