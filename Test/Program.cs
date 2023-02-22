using Honoo.Text;
using System;
using System.Collections.Generic;
using System.IO;

namespace Test
{
    internal class Program
    {
        private static void Main()
        {
            using (FileStream stream = new FileStream("d:\\xxx.torrent", FileMode.Open, FileAccess.Read))
            {
                BEncodeDictionary torrent = new BEncodeDictionary(stream);
                //
                // TODO: Read
                //
                torrent.TryGetValue("announce", out BEncodeSingle announce);
                Console.WriteLine(announce.GetStringValue());

                torrent.TryGetValue("announce-list", out BEncodeList announceList);
                foreach (BEncodeList ll in announceList)
                {
                    var ll0 = (BEncodeSingle)ll[0];
                    Console.WriteLine(ll0.GetStringValue());
                }

                torrent.TryGetValue("encoding", out BEncodeSingle encoding);
                Console.WriteLine(encoding.GetStringValue());

                torrent.TryGetValue("website", out BEncodeSingle website);
                Console.WriteLine(website.GetStringValue());

                torrent.TryGetValue("info", out BEncodeDictionary info);

                info.TryGetValue("name", out BEncodeSingle name);
                Console.WriteLine(name.GetStringValue());

                info.TryGetValue("files", out BEncodeList files);
                foreach (BEncodeDictionary file in files.Values)
                {
                    List<string> comb = new List<string>();
                    var path = (BEncodeList)file["path"];
                    foreach (BEncodeSingle df in path.Values)
                    {
                        comb.Add(df.GetStringValue());
                    }
                    var length = (BEncodeInteger)file["length"];
                    Console.WriteLine(string.Join("\\", comb) + "    " + length.GetInt64Value());
                }
                //
                // TODO: Write
                //
                info.AddOrUpdate("name", new BEncodeSingle("new torrent floder"));
                //
                // TODO: Save
                //
                using (FileStream stream2 = new FileStream("d:\\xxx2.torrent", FileMode.Create, FileAccess.ReadWrite))
                {
                    torrent.Save(stream2);
                }
            }
            Console.ReadKey(true);
        }
    }
}