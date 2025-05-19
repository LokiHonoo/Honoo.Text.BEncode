# Honoo.Text.BEncode

<!-- @import "[TOC]" {cmd="toc" depthFrom=1 depthTo=6 orderedList=false} -->

<!-- code_chunk_output -->

- [Honoo.Text.BEncode](#honootextbencode)
  - [INTRODUCTION](#introduction)
  - [GUIDE](#guide)
    - [GitHub](#github)
  - [LICENSE](#license)

<!-- /code_chunk_output -->

## INTRODUCTION

BEncode Analysis. Edit BEncode content (e.g. BT torrent file).

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
TorrentDocument torrent = new TorrentDocument(stream);  
torrent.SetName("Torrent Name");  
//  
torrent.GetName();  

## GUIDE

### GitHub

<https://github.com/LokiHonoo/Honoo.Text.BEncode/>

## LICENSE

[MIT](LICENSE) license.
