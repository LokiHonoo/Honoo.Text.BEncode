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
            string m = "输入文件路径，有空格的路径名称需要加前后双引号。";
            while (true)
            {
                Console.Clear();
                Console.WriteLine("=======================================================================================");
                Console.WriteLine();
                Console.WriteLine("                        TorrentViewer   runtime " + Environment.Version);
                Console.WriteLine();
                Console.WriteLine("=======================================================================================");
                Console.WriteLine();
                if (m.Length > 0)
                {
                    Console.WriteLine(m);
                    m = string.Empty;
                }
                Console.WriteLine();
                Console.WriteLine();
                Console.Write("拖放或输入 Torrent 文件:");

                string? line = Console.ReadLine();
                if (line != null)
                {
                    line = line.Trim('"');
                    if (File.Exists(line))
                    {
                        try
                        {
                            Console.Clear();
                            Console.WriteLine("\x1b[3J");
                            using (var stream = new FileStream(line, FileMode.Open, FileAccess.Read))
                            {
                                var torrent = new TorrentDocument(stream);
                                var sb = new StringBuilder();
                                var writer = new JsonTextWriter(new StringWriter(sb))
                                {
                                    Formatting = Formatting.Indented
                                };
                                Transform(torrent.Root, writer);
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
                    else
                    {
                        m = "文件不存在。";
                    }
                }
                else
                {
                    m = "输入文件路径，有空格的路径名称需要加前后双引号。";
                }
            }
        }

        private static void Transform(BEncodeElement bEncodeValue, JsonTextWriter writer)
        {
            switch (bEncodeValue)
            {
                case BEncodeDictionary dict:
                    writer.WriteStartObject();
                    foreach (KeyValuePair<BEncodeString, BEncodeElement> entry in dict)
                    {
                        string key = entry.Key.GetStringValue().Trim('\r').Trim('\n');
                        writer.WritePropertyName(key);
                        TransformDictionaryEntry(key, entry.Value, writer);
                    }
                    writer.WriteEndObject();
                    break;

                case BEncodeList list:
                    writer.WriteStartArray();
                    foreach (BEncodeElement value in list)
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

        private static void TransformDictionaryEntry(string key, BEncodeElement value, JsonTextWriter writer)
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