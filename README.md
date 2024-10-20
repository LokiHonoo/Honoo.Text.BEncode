# Honoo.Text.BEncode

<!-- @import "[TOC]" {cmd="toc" depthFrom=1 depthTo=6 orderedList=false} -->

<!-- code_chunk_output -->

- [Honoo.Text.BEncode](#honootextbencode)
  - [INTRODUCTION](#introduction)
  - [GUIDE](#guide)
    - [NuGet](#nuget)
  - [USAGE](#usage)
    - [Basic](#basic)
    - [Torrent](#torrent)
  - [CHANGELOG](#changelog)
    - [1.1.1](#111)
    - [1.0.9](#109)
    - [1.0.8](#108)
    - [1.0.4](#104)
    - [1.0.3](#103)
    - [1.0.2](#102)
    - [1.0.1](#101)
    - [1.0.0](#100)
  - [LICENSE](#license)

<!-- /code_chunk_output -->

## INTRODUCTION

读写 BEncode 编码格式，例如 torrent 文件。

## GUIDE

### NuGet

<https://www.nuget.org/packages/Honoo.Text.BEncode/>

## USAGE

```c#

using Honoo.Text.BEncode;

```

### Basic

```c#

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
            dict.AddOrUpdate("key2", new BEncodeString("key2update"));
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

```

### Torrent

```c#

private static void ReadTorrent()
{
    string fileName = "葬送的芙莉莲 Sousou no Frieren [WebRip 1080p HEVC-10bit AAC][Fin].torrent";
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

        var files = torrent.GetFiles("*.*", 0, long.MaxValue);
        foreach (var file in files.OrderByDescending(entry => entry.GetLength()))
        {
            Console.WriteLine(file.GetPaths()[^1] + "    " + Numerics.GetSize(file.GetLength(), SizeKilo.Auto, 2, out string unit) + unit);
        }
        Console.WriteLine();

        var magnet = torrent.GetMagnet(true, true, true, false);
        Console.WriteLine(magnet);
    }
}

```

```c#

private static void CreateTorrent256K()
{
    string fileName = "test_create_" + Path.GetRandomFileName() + ".torrent";

    var torrent = new Honoo.Text.BEncode.TorrentAnalysis();
    torrent.SetAnnounce("http://tracker1.itzmx.com:8080/announce");
    torrent.SetAnnounceList([
        ["http://tracker2.itzmx.com:6961/announce", "http://tracker2.itzmx.com:6961/announce"],
        ["http://open.acgtracker.com:1096/announce", "http://open.acgtracker.com:1096/announce"]
    ]);
    torrent.SetComment("https://github.com/LokiHonoo/Honoo.Text.BEncode");
    torrent.SetEncoding("UTF-8");
    torrent.SetNodes([new IPEndPoint(IPAddress.Parse("111.111.111.111"), 7777)]);
    torrent.SetPieceLength(256 * 1024);
    // Set single file.
    //torrent.SetFile("TestItems\\(pid-48674501)デウス.jpg");
    // Set multiple file.
    torrent.SetFiles(AppDomain.CurrentDomain.BaseDirectory); // folder

    using (var stream = new FileStream(fileName, FileMode.Create, FileAccess.Write))
    {
        torrent.Save(stream, "LokiHonoo", DateTime.UtcNow, true);
    }
  
    var magnet = torrent.GetMagnet(true, true, false, false);
    Console.WriteLine(magnet);
}

```

## CHANGELOG

### 1.1.1

**Features* 优化。

### 1.0.9

**Features* TorrentFileEntry 增加了快速修改文件名的方法。同时公开了元素节点以供自由编辑。

### 1.0.8

**Features* TorrentAnalysis 类新增一些 Torrent 文件标准节点的编辑方法。

### 1.0.4

**Features* 新增部分方法的多个重载。

### 1.0.3

**Refactored* 涉及文本编码的字段以原始值存储，不再自动转换。在取出时由使用者指定文本编码。

**Features* 新增 BEncodeDictionary 的 AddOrUpdate() 方法的多个重载。

**Features* 新增 TorrentAnalysis 类，从 BEncodeDictionary 继承，提供一些 Torrent 文件标准节点的读取方法。

### 1.0.2

**Refactored* BEncodeDictionary 的 AddOrUpdate() 方法返回添加的值，并且支持泛型。

**Refactored* BEncodeList 的 Add() AddRange() 方法返回添加的值，并且支持泛型。

### 1.0.1

**Refactored* BEncodeList 公开所有 IList 的方法。

### 1.0.0

*初始版本。

## LICENSE

[MIT](LICENSE) license.
