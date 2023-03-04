# Honoo.Text.BEncode

<!-- @import "[TOC]" {cmd="toc" depthFrom=1 depthTo=6 orderedList=false} -->

<!-- code_chunk_output -->

- [Honoo.Text.BEncode](#honootextbencode)
  - [INTRODUCTION](#introduction)
  - [CHANGELOG](#changelog)
    - [1.0.2](#102)
    - [1.0.1](#101)
    - [1.0.0](#100)
  - [USAGE](#usage)
    - [NuGet](#nuget)
    - [Namespace](#namespace)
    - [Example](#example)
  - [LICENSE](#license)

<!-- /code_chunk_output -->

## INTRODUCTION

读写 BEncode 编码格式，例如 torrent 文件。

## CHANGELOG

### 1.0.2

**Refactored* BEncodeDictionary 的 AddOrUpdate() 方法可直接返回添加的值，并且支持泛型。

**Refactored* BEncodeList 的 Add() AddRange() 方法可直接返回添加的值，并且支持泛型。

### 1.0.1

**Refactored* BEncodeList 公开所有 IList 的方法。

### 1.0.0

*初始版本。

## USAGE

### NuGet

<https://www.nuget.org/packages/Honoo.Text.BEncode/>

### Namespace

```c#

using Honoo.Text;

```

### Example

```c#

private static void Main()
{
    BEncodeDictionary root = new BEncodeDictionary();
    //
    // Write
    //
    var dict = root.AddOrUpdate("dict", new BEncodeDictionary());
    dict.AddOrUpdate("key3", new BEncodeSingle("key3sorted"));
    dict.AddOrUpdate("key2", new BEncodeSingle("key2sorted"));
    dict.AddOrUpdate("key4", new BEncodeSingle("key4sorted"));
    var list = dict.AddOrUpdate("key1", new BEncodeList());
    list.Add(new BEncodeInteger(111));
    list.Add(new BEncodeInteger(222));
    list.Add(new BEncodeInteger(333));
    //
    // Read
    //
    root.TryGetValue("dict", out BEncodeDictionary dict1);
    dict1.TryGetValue("key2", out BEncodeSingle single1);
    Console.WriteLine(single1.GetStringValue());
    dict1.TryGetValue("key3", out single1);
    Console.WriteLine(single1.GetStringValue());
    dict1.TryGetValue("key4", out single1);
    Console.WriteLine(single1.GetStringValue());
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
        BEncodeDictionary root1 = new BEncodeDictionary(stream);
    }

    Console.ReadKey(true);
}

```

## LICENSE

MIT 协议。
