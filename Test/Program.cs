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
            var doc = new BEncodeDocument();
            var doc2 = new BEncodeDocument();
            while (true)
            {
                //
                // Write
                //
                var dict = doc.Root.AddOrUpdate("dict", doc.CreateDictionary());
                dict.AddOrUpdate("key3", doc.CreateString("key3"));
                dict.AddOrUpdate("key2", doc.CreateString("key2"));
                dict.AddOrUpdate("key4", doc.CreateString("key4"));
                dict.AddOrUpdate("key2", doc.CreateString("key2update"));
                var list = doc.Root.AddOrUpdate("key1", doc.CreateList());
                list.Add(doc.CreateInteger(333));
                list.Add(doc.CreateInteger(111));
                list.Add(doc.CreateInteger(222));
                //
                // Allways error.
                //
                try
                {
                    doc.Root.Add("error", doc2.CreateString("error"));
                }
                catch
                {
                    Console.WriteLine("Document cross reference.");
                }
                //
                // Read
                //
                dict = doc.Root.GetValue<BEncodeDictionary>("dict");
                dict.TryGetValue("key2", out BEncodeString string1);
                Console.WriteLine(string1.GetStringValue());
                dict.TryGetValue("key3", out string1);
                Console.WriteLine(string1.GetStringValue());
                dict.TryGetValue("key4", out string1);
                Console.WriteLine(string1.GetStringValue());
                var list1 = (BEncodeList)doc.Root["key1"];
                Console.WriteLine(((BEncodeInteger)list1[0]).Value);
                Console.WriteLine(((BEncodeInteger)list1[1]).GetInt32Value());
                Console.WriteLine(((BEncodeInteger)list1[2]).GetInt64Value());
                //
                // Copy
                //
                doc = new BEncodeDocument(doc.Root);
                doc = new BEncodeDocument(doc);
                //
                // Save
                //
                using (MemoryStream stream = new MemoryStream())
                {
                    doc.Save(stream);
                    string content = Encoding.UTF8.GetString(stream.ToArray());
                    Console.WriteLine(content);
                    stream.Seek(0, SeekOrigin.Begin);
                    doc = new BEncodeDocument(stream);
                }
                //
                Console.ReadKey(true);
            }
        }
    }
}