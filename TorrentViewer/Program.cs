using Honoo.Text.BEncode;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace TorrentViewer
{
    internal class Program
    {
        private static void Main(string[] args)
        {
            string m = string.Empty;
            while (true)
            {
                Console.Clear();
                Console.WriteLine("=======================================================================================");
                Console.WriteLine();
                Console.WriteLine("                        TorrentViewer   runtime " + Environment.Version);
                Console.WriteLine();
                if (m.Length > 0)
                {
                    Console.WriteLine(m);
                    m = string.Empty;
                }
                Console.WriteLine("=======================================================================================");
                //
                Console.WriteLine();
                Console.Write("拖放或输入 Torrent 文件:");
                while (true)
                {
                    string? line = Console.ReadLine();
                    if (line != null)
                    {
                        line = line.Trim('"');
                        if (File.Exists(line))
                        {
                            try
                            {
                                Console.Clear();
                                using (var stream = new FileStream(line, FileMode.Open, FileAccess.Read))
                                {
                                    var torrent = new TorrentAnalysis(stream, true);
                                    var sb = new StringBuilder();
                                    var writer = new JsonTextWriter(new StringWriter(sb))
                                    {
                                        Formatting = Formatting.Indented
                                    };
                                    Transform(torrent, writer);
                                    Console.WriteLine(sb.ToString());
                                }
                                Console.WriteLine();
                                Console.Write("Press any key to Main Menu...");
                                Console.Write("\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n");
                                Console.ReadKey(true);
                            }
                            catch (Exception ex)
                            {
                                m = ex.Message;
                            }
                        }
                    }
                    break;
                }
            }
        }

        private static void Transform(BEncodeValue bEncodeValue, JsonTextWriter writer)
        {
            switch (bEncodeValue)
            {
                case BEncodeDictionary dict:
                    writer.WriteStartObject();
                    foreach (KeyValuePair<BEncodeString, BEncodeValue> entry in dict)
                    {
                        string key = entry.Key.GetStringValue().Trim('\r').Trim('\n');
                        writer.WritePropertyName(key);
                        TransformDictionaryEntry(key, entry.Value, writer);
                    }
                    writer.WriteEndObject();
                    break;

                case BEncodeList list:
                    writer.WriteStartArray();
                    foreach (BEncodeValue value in list)
                    {
                        Transform(value, writer);
                    }
                    writer.WriteEndArray();
                    break;

                case BEncodeInteger integer:
                    writer.WriteValue(integer.Value.Trim('\r').Trim('\n'));
                    break;

                case BEncodeString str:
                    writer.WriteValue(str.GetStringValue().Trim('\r').Trim('\n'));
                    break;

                default: break;
            }
        }

        private static void TransformDictionaryEntry(string key, BEncodeValue value, JsonTextWriter writer)
        {
            if (key == "pieces")
            {
                writer.WriteValue("pieces bytes");
            }
            else
            {
                Transform(value, writer);
            }
        }
    }
}