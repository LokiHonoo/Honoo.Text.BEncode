using Honoo.Text.BEncode;
using System;
using System.IO;
using System.Text;

namespace Test
{
    internal class Program
    {
        private static void Main()
        {
            BEncodeDictionary root = new BEncodeDictionary();
            while (true)
            {
                //
                // Write
                //
                var dict = root.AddOrUpdate("dict", new BEncodeDictionary());
                dict.AddOrUpdate("key3", new BEncodeString("key3"));
                dict.AddOrUpdate("key2", new BEncodeString("key2"));
                dict.AddOrUpdate("key4", new BEncodeString("key4"));
                dict.AddOrUpdate("key2", new BEncodeString("key2reset"));
                var list = dict.AddOrUpdate("key1", new BEncodeList());
                list.Add(new BEncodeInteger(333));
                list.Add(new BEncodeInteger(111));
                list.Add(new BEncodeInteger(222));
                //
                // Read
                //
                dict = root.GetValue<BEncodeDictionary>("dict");
                dict.TryGetValue("key2", out BEncodeString string1);
                Console.WriteLine(string1.GetStringValue());
                dict.TryGetValue("key3", out string1);
                Console.WriteLine(string1.GetStringValue());
                dict.TryGetValue("key4", out string1);
                Console.WriteLine(string1.GetStringValue());
                var list1 = (BEncodeList)dict["key1"];
                Console.WriteLine(((BEncodeInteger)list1[0]).Value);
                Console.WriteLine(((BEncodeInteger)list1[1]).GetInt32Value());
                Console.WriteLine(((BEncodeInteger)list1[2]).GetInt64Value());
                //
                // Save
                //
                using (MemoryStream stream = new MemoryStream())
                {
                    root.Save(stream);
                    string content = Encoding.UTF8.GetString(stream.ToArray());
                    Console.WriteLine(content);
                    stream.Seek(0, SeekOrigin.Begin);
                    root = new BEncodeDictionary(stream);
                }
                Console.ReadKey(true);
            }
        }
    }
}