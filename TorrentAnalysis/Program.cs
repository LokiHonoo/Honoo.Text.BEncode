﻿using Honoo.IO;
using System;
using System.IO;
using System.Linq;
using System.Net;

namespace TorrentAnalysis
{
    internal class Program
    {
        #region Main

        private static void Main()
        {
            string testItemsFolder = "..\\..\\..\\TestItems";

            ReadTorrent($"{testItemsFolder}\\[喵萌奶茶屋&LoliHouse] 葬送的芙莉莲  Sousou no Frieren [01-28 修正合集][WebRip 1080p HEVC-10bit AAC][简繁日内封字幕][Fin].torrent");
            Console.WriteLine("====================================================================================");
            Console.ReadKey(true);
            //
            //
            //
            CreateTorrent16MSingle("test_create_16M_s.torrent", $"{testItemsFolder}\\(pid-48674501)デウス.jpg");
            Console.WriteLine("====================================================================================");
            Console.ReadKey(true);
            //
            //
            //
            CreateTorrent16MMultiple("test_create_16M_m.torrent", $"{testItemsFolder}\\pics");
            Console.WriteLine("====================================================================================");
            Console.ReadKey(true);
            //
            //
            //
            CreateTorrent256KSingle("test_create_256K_s.torrent", $"{testItemsFolder}\\(pid-48674501)デウス.jpg");
            Console.WriteLine("====================================================================================");
            Console.ReadKey(true);
            //
            //
            //
            CreateTorrent256KMultiple("test_create_256K_m.torrent", $"{testItemsFolder}\\pics");
            Console.WriteLine("====================================================================================");
            Console.ReadKey(true);
            //
            //
            //
            Compare("16M Single", $"{testItemsFolder}\\test_bc_16M_s.torrent", "test_create_16M_s.torrent");
            Console.WriteLine("====================================================================================");
            Console.ReadKey(true);
            //
            //
            //
            Compare("16M Multiple", $"{testItemsFolder}\\test_bc_16M_m.torrent", "test_create_16M_m.torrent");
            Console.WriteLine("====================================================================================");
            Console.ReadKey(true);
            //
            //
            //
            Compare("256K Single", $"{testItemsFolder}\\test_bc_256K_s.torrent", "test_create_256K_s.torrent");
            Console.WriteLine("====================================================================================");
            Console.ReadKey(true);
            //
            //
            //
            Compare("256K Multiple", $"{testItemsFolder}\\test_bc_256K_m.torrent", "test_create_256K_m.torrent");
            Console.WriteLine("====================================================================================");
            Console.ReadKey(true);
        }

        #endregion Main

        private static void Compare(string info, string bc, string cre)
        {
            Console.WriteLine($"========== Compare {info} ==========");

            using (var stream = new FileStream(bc, FileMode.Open, FileAccess.Read))
            {
                var torrent = new Honoo.Text.BEncode.TorrentAnalysis(stream, true);
                Console.WriteLine("created by    :" + torrent.GetCreatedBy());
                Console.WriteLine("creation date :" + torrent.GetCreationDate());
                Console.WriteLine("announce      :" + torrent.GetAnnounce());
                Console.WriteLine("comment       :" + torrent.GetComment());
                Console.WriteLine("hash          :" + torrent.GetHash());
                Console.WriteLine("nodes         :" + torrent.GetNodes());
                Console.WriteLine("name          :" + torrent.GetName());
                Console.WriteLine("piece length  :" + torrent.GetPieceLength());
                Console.WriteLine("pieces        :" + BitConverter.ToString(torrent.GetPieces()).Replace("-", ""));
                var files = torrent.GetFiles("", 0, long.MaxValue, null);
                foreach (var file in files)
                {
                    Console.WriteLine(string.Join('\\', file.Paths).PadRight(80) + Numeric.GetSize(file.Length, Numeric.SizeKilo.Auto, 2, out string unit) + unit);
                }
            }
            Console.WriteLine();
            using (var stream = new FileStream(cre, FileMode.Open, FileAccess.Read))
            {
                var torrent = new Honoo.Text.BEncode.TorrentAnalysis(stream, true);
                Console.WriteLine("created by    :" + torrent.GetCreatedBy());
                Console.WriteLine("creation date :" + torrent.GetCreationDate());
                Console.WriteLine("announce      :" + torrent.GetAnnounce());
                Console.WriteLine("comment       :" + torrent.GetComment());
                Console.WriteLine("hash          :" + torrent.GetHash());
                Console.WriteLine("nodes         :" + torrent.GetNodes());
                Console.WriteLine("name          :" + torrent.GetName());
                Console.WriteLine("piece length  :" + torrent.GetPieceLength());
                Console.WriteLine("pieces        :" + BitConverter.ToString(torrent.GetPieces()).Replace("-", ""));
                var files = torrent.GetFiles("", 0, long.MaxValue, null);
                foreach (var file in files)
                {
                    Console.WriteLine(string.Join('\\', file.Paths).PadRight(80) + Numeric.GetSize(file.Length, Numeric.SizeKilo.Auto, 2, out string unit) + unit);
                }
            }
            Console.WriteLine();
        }

        private static void CreateTorrent16MMultiple(string fileName, string dst)
        {
            Console.WriteLine("========== Create 16M Multiple ==========");

            var torrent = new Honoo.Text.BEncode.TorrentAnalysis();
            torrent.SetAnnounce("http://tracker1.itzmx.com:8080/announce");
            torrent.SetAnnounceList([
                ["http://tracker2.itzmx.com:6961/announce", "http://tracker2.itzmx.com:6961/announce"],
                ["http://open.acgtracker.com:1096/announce", "http://open.acgtracker.com:1096/announce"]
            ]);
            //torrent.SetCreatedBy("LokiHonoo");
            torrent.SetComment("https://github.com/LokiHonoo/Honoo.Text.BEncode");
            torrent.SetEncoding("UTF-8");
            torrent.SetNodes([new IPEndPoint(IPAddress.Parse("111.111.111.111"), 7777)]);
            torrent.SetFiles(dst);

            using (var stream = new FileStream(fileName, FileMode.Create, FileAccess.Write))
            {
                torrent.Save(stream);
            }
            Console.WriteLine("created by    :" + torrent.GetCreatedBy());
            Console.WriteLine("creation date :" + torrent.GetCreationDate());
            Console.WriteLine("announce      :" + torrent.GetAnnounce());
            Console.WriteLine("comment       :" + torrent.GetComment());
            Console.WriteLine("hash          :" + torrent.GetHash());
            Console.WriteLine("nodes         :" + torrent.GetNodes());
            Console.WriteLine("name          :" + torrent.GetName());
            Console.WriteLine("piece length  :" + torrent.GetPieceLength());

            Console.WriteLine();
            var magnet = torrent.GetMagnet(true, true, false, false);
            Console.WriteLine(magnet);
            Console.WriteLine();
        }

        private static void CreateTorrent16MSingle(string fileName, string dst)
        {
            Console.WriteLine("========== Create 16M Single ==========");

            var torrent = new Honoo.Text.BEncode.TorrentAnalysis();
            torrent.SetAnnounce("http://tracker1.itzmx.com:8080/announce");
            torrent.SetAnnounceList([
                ["http://tracker2.itzmx.com:6961/announce", "http://tracker2.itzmx.com:6961/announce"],
                ["http://open.acgtracker.com:1096/announce", "http://open.acgtracker.com:1096/announce"]
            ]);
            torrent.SetCreatedBy("LokiHonoo");
            torrent.SetComment("https://github.com/LokiHonoo/Honoo.Text.BEncode");
            torrent.SetEncoding("UTF-8");
            torrent.SetNodes([new IPEndPoint(IPAddress.Parse("111.111.111.111"), 7777)]);
            torrent.SetFile(dst);

            using (var stream = new FileStream(fileName, FileMode.Create, FileAccess.Write))
            {
                torrent.Save(stream);
            }
            Console.WriteLine("created by    :" + torrent.GetCreatedBy());
            Console.WriteLine("creation date :" + torrent.GetCreationDate());
            Console.WriteLine("announce      :" + torrent.GetAnnounce());
            Console.WriteLine("comment       :" + torrent.GetComment());
            Console.WriteLine("hash          :" + torrent.GetHash());
            Console.WriteLine("nodes         :" + torrent.GetNodes());
            Console.WriteLine("name          :" + torrent.GetName());
            Console.WriteLine("piece length  :" + torrent.GetPieceLength());

            Console.WriteLine();
            var magnet = torrent.GetMagnet(true, true, false, false);
            Console.WriteLine(magnet);
            Console.WriteLine();
        }

        private static void CreateTorrent256KMultiple(string fileName, string dst)
        {
            Console.WriteLine("========== Create 256K Multiple ==========");

            var torrent = new Honoo.Text.BEncode.TorrentAnalysis();
            torrent.SetAnnounce("http://tracker1.itzmx.com:8080/announce");
            torrent.SetAnnounceList([
                ["http://tracker2.itzmx.com:6961/announce", "http://tracker2.itzmx.com:6961/announce"],
                ["http://open.acgtracker.com:1096/announce", "http://open.acgtracker.com:1096/announce"]
            ]);
            //torrent.SetCreatedBy("LokiHonoo");
            torrent.SetComment("https://github.com/LokiHonoo/Honoo.Text.BEncode");
            torrent.SetEncoding("UTF-8");
            torrent.SetNodes([new IPEndPoint(IPAddress.Parse("111.111.111.111"), 7777)]);
            torrent.SetPieceLength(256 * 1024);
            torrent.SetFiles(dst);

            using (var stream = new FileStream(fileName, FileMode.Create, FileAccess.Write))
            {
                torrent.Save(stream);
            }
            Console.WriteLine("created by    :" + torrent.GetCreatedBy());
            Console.WriteLine("creation date :" + torrent.GetCreationDate());
            Console.WriteLine("announce      :" + torrent.GetAnnounce());
            Console.WriteLine("comment       :" + torrent.GetComment());
            Console.WriteLine("hash          :" + torrent.GetHash());
            Console.WriteLine("nodes         :" + torrent.GetNodes());
            Console.WriteLine("name          :" + torrent.GetName());
            Console.WriteLine("piece length  :" + torrent.GetPieceLength());

            Console.WriteLine();
            var magnet = torrent.GetMagnet(true, true, false, false);
            Console.WriteLine(magnet);
            Console.WriteLine();
        }

        private static void CreateTorrent256KSingle(string fileName, string dst)
        {
            Console.WriteLine("========== Create 256K Single ==========");

            var torrent = new Honoo.Text.BEncode.TorrentAnalysis();
            torrent.SetAnnounce("http://tracker1.itzmx.com:8080/announce");
            torrent.SetAnnounceList([
                ["http://tracker2.itzmx.com:6961/announce", "http://tracker2.itzmx.com:6961/announce"],
                ["http://open.acgtracker.com:1096/announce", "http://open.acgtracker.com:1096/announce"]
            ]);
            torrent.SetCreatedBy("LokiHonoo");
            torrent.SetComment("https://github.com/LokiHonoo/Honoo.Text.BEncode");
            torrent.SetEncoding("UTF-8");
            torrent.SetNodes([new IPEndPoint(IPAddress.Parse("111.111.111.111"), 7777)]);
            torrent.SetPieceLength(256 * 1024);
            torrent.SetFile(dst);

            using (var stream = new FileStream(fileName, FileMode.Create, FileAccess.Write))
            {
                torrent.Save(stream);
            }
            Console.WriteLine("created by    :" + torrent.GetCreatedBy());
            Console.WriteLine("creation date :" + torrent.GetCreationDate());
            Console.WriteLine("announce      :" + torrent.GetAnnounce());
            Console.WriteLine("comment       :" + torrent.GetComment());
            Console.WriteLine("hash          :" + torrent.GetHash());
            Console.WriteLine("nodes         :" + torrent.GetNodes());
            Console.WriteLine("name          :" + torrent.GetName());
            Console.WriteLine("piece length  :" + torrent.GetPieceLength());

            Console.WriteLine();
            var magnet = torrent.GetMagnet(true, true, false, false);
            Console.WriteLine(magnet);
            Console.WriteLine();
        }

        private static void ReadTorrent(string fileName)
        {
            Console.WriteLine("========== Read ==========");

            using (var stream = new FileStream(fileName, FileMode.Open, FileAccess.Read))
            {
                var torrent = new Honoo.Text.BEncode.TorrentAnalysis(stream, true);
                Console.WriteLine("created by    :" + torrent.GetCreatedBy());
                Console.WriteLine("creation date :" + torrent.GetCreationDate());
                Console.WriteLine("announce      :" + torrent.GetAnnounce());
                Console.WriteLine("comment       :" + torrent.GetComment());
                Console.WriteLine("hash          :" + torrent.GetHash());
                Console.WriteLine("nodes         :" + torrent.GetNodes());
                Console.WriteLine("name          :" + torrent.GetName());
                Console.WriteLine("piece length  :" + torrent.GetPieceLength());

                var files = torrent.GetFiles("Frieren - 17 [WebRip", 0, long.MaxValue, null);
                Console.WriteLine($"Search - \"Frieren - 17 [WebRip\" - count:{files.Count}");
                files = torrent.GetFiles("10bit AAC ASSx2].mkv", 0, long.MaxValue, null);
                Console.WriteLine($"Search - \"10bit AAC ASSx2].mkv\" - count:{files.Count}");
                Console.WriteLine("Search - \"*.*\"");
                files = torrent.GetFiles("*.*", 0, long.MaxValue, null);
                foreach (var file in files.OrderByDescending(entry => entry.Length))
                {
                    Console.WriteLine(file.Paths[^1] + "    " + Numeric.GetSize(file.Length, Numeric.SizeKilo.Auto, 2, out string unit) + unit);
                }
                Console.WriteLine();

                var magnet = torrent.GetMagnet(true, true, true, false);
                Console.WriteLine(magnet);
            }
            Console.WriteLine();
        }
    }
}