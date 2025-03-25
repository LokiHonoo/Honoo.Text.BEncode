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
            BEncodeDocument doc = new BEncodeDocument();
            BEncodeDocument doc2 = new BEncodeDocument();
            while (true)
            {
                //
                // Write
                //
                doc.Root.AddOrUpdate("key1", doc.CreateInteger(996));
                doc.Root.AddOrUpdate("key2", doc.CreateInteger(007));
                BEncodeDictionary dict = doc.Root.AddOrUpdate("dict", doc.CreateDictionary());
                BEncodeList list = dict.AddOrUpdate("key3", doc.CreateList());
                list.Add(doc.CreateString("icu"));
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
                Console.WriteLine(doc.Root.GetValue<BEncodeInteger>("key1").GetInt32Value());
                Console.WriteLine(doc.Root.GetValue<BEncodeInteger>("key2").GetInt32Value());
                doc.Root.GetValue<BEncodeDictionary>("dict").TryGetValue("key3", out list);
                Console.WriteLine(((BEncodeString)list[0]).GetStringValue());
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