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
    - [1.2.1](#121)
    - [1.1.4](#114)
    - [1.1.3](#113)
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

BEncodeDocument doc = new BEncodeDocument();
doc.Root.AddOrUpdate("key1", doc.CreateInteger(996));
doc.Root.AddOrUpdate("key2", doc.CreateInteger(007));
BEncodeDictionary dict = doc.Root.AddOrUpdate("dict", doc.CreateDictionary());
BEncodeList list = dict.AddOrUpdate("key3", doc.CreateList());
list.Add(doc.CreateString("icu"));
//
doc.Root.TryGetValue("key1", out BEncodeInteger output);
//
//
//
TorrentAnalysis torrent = new TorrentAnalysis(stream);
torrent.SetName("Torrent Name");
//
torrent.GetName();

## GUIDE

### NuGet

<https://www.nuget.org/packages/Honoo.Text.BEncode/>

## USAGE

```c#

using Honoo.Text.BEncode;

```

### Basic

```c#

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

private static void CreateTorrent()
{
    string fileName = "test_create_" + Path.GetRandomFileName() + ".torrent";

    var torrent = new Honoo.Text.BEncode.TorrentAnalysis();
    torrent.SetAnnounce("http://tracker1.itzmx.com:8080/announce");
    torrent.SetAnnounceList([
        ["http://tracker2.itzmx.com:6961/announce", "http://tracker2.itzmx.com:6961/announce"],
        ["http://open.acgtracker.com:1096/announce", "http://open.acgtracker.com:1096/announce"]
    ]);
    torrent.SetComment("https://github.com/LokiHonoo/Honoo.Text.BEncode");
    torrent.SetNodes([new IPEndPoint(IPAddress.Parse("111.111.111.111"), 7777)]);
    // Set single file.
    //torrent.SetFile(new FileInfo("TestItems\\(pid-48674501)デウス.jpg"), 512 * 1024);
    // Set multiple file.
    torrent.SetFiles(new DirectoryInfo(AppDomain.CurrentDomain.BaseDirectory), 512 * 1024);

    using (var stream = new FileStream(fileName, FileMode.Create, FileAccess.Write))
    {
        torrent.Save(stream, "LokiHonoo", DateTime.UtcNow, true);
    }
  
    var magnet = torrent.GetMagnet(true, true, false, false);
    Console.WriteLine(magnet);
}

```

## CHANGELOG

### 1.2.1

**Removed* 删除了只读接口。

**Refactored* 种子添加文件的方法增加更多操作项。

**Refactored* BEncodeInteger、BEncodeString 更改为只读模式。

**Features* 新增 BEncodeDocument 类型作为根元素，元素需要从 BEncodeDocument.CreateDictionary() 创建。操作方式类似与 XmlDocument。

### 1.1.4

**Fix* 完善注释。

### 1.1.3

**Fix* 修复 BUG。

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
