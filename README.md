# Honoo.Text.BEncode

<!-- @import "[TOC]" {cmd="toc" depthFrom=1 depthTo=6 orderedList=false} -->

<!-- code_chunk_output -->

- [Honoo.Text.BEncode](#honootextbencode)
  - [INTRODUCTION](#introduction)
  - [CHANGELOG](#changelog)
    - [1.0.4](#104)
    - [1.0.3](#103)
    - [1.0.2](#102)
    - [1.0.1](#101)
    - [1.0.0](#100)
  - [USAGE](#usage)
    - [Github](#github)
    - [NuGet](#nuget)
    - [Namespace](#namespace)
    - [Example](#example)
  - [LICENSE](#license)

<!-- /code_chunk_output -->

## INTRODUCTION

读写 BEncode 编码格式，例如 torrent 文件。

## CHANGELOG

### 1.0.4

**Features* 新增部分方法的多个重载。

### 1.0.3

**Refactored* 涉及文本编码的字段以原始值存储，不再自动转换。在取出时由使用者指定文本编码。

**Features* 新增 BEncodeDictionary 的 AddOrUpdate() 方法的多个重载。

**Features* 新增 TorrentAnalysis 类，从 BEncodeDictionary 继承，提供一些 Torrent 文件的相关方法。

### 1.0.2

**Refactored* BEncodeDictionary 的 AddOrUpdate() 方法可直接返回添加的值，并且支持泛型。

**Refactored* BEncodeList 的 Add() AddRange() 方法可直接返回添加的值，并且支持泛型。

### 1.0.1

**Refactored* BEncodeList 公开所有 IList 的方法。

### 1.0.0

*初始版本。

## USAGE

### Github

<https://github.com/LokiHonoo/Honoo.Text.BEncode/>

### NuGet

<https://www.nuget.org/packages/Honoo.Text.BEncode/>

### Namespace

```c#

using Honoo.Text.BEncode;

```

### Example

```c#

private static void Main()
{
    BEncodeDictionary root = new BEncodeDictionary();
    while (true)
    {
        //
        // Write
        //
        var dict = root.AddOrUpdate("dict", new BEncodeDictionary());
        dict.AddOrUpdate("key3", new BEncodeString("key3sorted"));
        dict.AddOrUpdate("key2", new BEncodeString("key2sorted"));
        dict.AddOrUpdate("key4", new BEncodeString("key4sorted"));
        dict.AddOrUpdate("key2", new BEncodeString("key2reset"));
        var list = dict.AddOrUpdate("key1", new BEncodeList());
        list.Add(new BEncodeInteger(111));
        list.Add(new BEncodeInteger(222));
        list.Add(new BEncodeInteger(333));
        //
        // Read
        //
        root.TryGetValue("dict", out BEncodeDictionary dict1);
        dict1.TryGetValue("key2", out BEncodeString string1);
        Console.WriteLine(string1.GetStringValue());
        dict1.TryGetValue("key3", out string1);
        Console.WriteLine(string1.GetStringValue());
        dict1.TryGetValue("key4", out string1);
        Console.WriteLine(string1.GetStringValue());
        var list1 = (BEncodeList)dict1["key1"];
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

```

```c#

private static void Main()
{
    string fileName = "4.torrent";
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
        var magnet = torrent.GetMagnet();
        Console.WriteLine(magnet);
    }
    Console.WriteLine();
    Console.ReadKey(true);
}

```
## LICENSE

This project based on [MIT](LICENSE) license.

