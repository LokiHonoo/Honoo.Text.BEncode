using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;

namespace Honoo.Text.BEncode
{
    /// <summary>
    /// Torrent 文件的根节点。在 BEncode 字典类型的基础上增加了一些 Torrent 文件的相关方法。
    /// </summary>
    public class TorrentAnalysis : BEncodeDictionary
    {
        #region Properties

        private readonly bool _multiple;
        private readonly bool? _private;
        private readonly DateTime _start = DateTime.FromBinary(621355968000000000L);

        /// <summary>
        /// 获取一个值，指示此 Torrent 文件是否包含多个文件。
        /// </summary>
        public bool Multiple => _multiple;

        /// <summary>
        /// 获取一个值，指示此 Torrent 文件是否是私有种子。
        /// </summary>
        public bool? Private => _private;

        #endregion Properties

        #region Construction

        /// <summary>
        /// 初始化 TorrentAnalysis 类的新实例。
        /// </summary>
        /// <param name="content">指定从中读取的流。</param>
        /// <exception cref="Exception"></exception>
        public TorrentAnalysis(Stream content) : base(content)
        {
            BEncodeDictionary info = (BEncodeDictionary)this["info"];
            if (info.TryGetValue("private", out BEncodeInteger value))
            {
                _private = value.GetInt32Value() == 1;
            }
            if (info.ContainsKey("files"))
            {
                _multiple = true;
            }
        }

        #endregion Construction

        /// <summary>
        /// 获取主要 Tracker 服务器。默认使用 Encoding.UTF8 编码。
        /// </summary>
        /// <returns></returns>
        public string GetAnnounce()
        {
            return GetAnnounce(Encoding.UTF8);
        }

        /// <summary>
        /// 获取主要 Tracker 服务器。
        /// </summary>
        /// <param name="encoding">用于转换的字符编码。</param>
        /// <returns></returns>
        public string GetAnnounce(Encoding encoding)
        {
            if (TryGetValue("announce", out BEncodeString value))
            {
                return value.GetStringValue(encoding);
            }
            return null;
        }

        /// <summary>
        /// 获取备用 Tracker 服务器列表。默认使用 Encoding.UTF8 编码。
        /// </summary>
        /// <returns></returns>
        public List<List<string>> GetAnnounceList()
        {
            return GetAnnounceList(Encoding.UTF8);
        }

        /// <summary>
        /// 获取备用 Tracker 服务器列表。
        /// </summary>
        /// <param name="encoding">用于转换的字符编码。</param>
        /// <returns></returns>
        public List<List<string>> GetAnnounceList(Encoding encoding)
        {
            List<List<string>> result = new List<List<string>>();
            if (TryGetValue("announce-list", out BEncodeList list))
            {
                foreach (BEncodeList list2 in list.Cast<BEncodeList>())
                {
                    List<string> al = new List<string>();
                    foreach (BEncodeString entry in list2.Cast<BEncodeString>())
                    {
                        al.Add(entry.GetStringValue(encoding));
                    }
                    result.Add(al);
                }
            }
            return result;
        }

        /// <summary>
        /// 获取注释。默认使用 Encoding.UTF8 编码。
        /// </summary>
        /// <returns></returns>
        public string GetComment()
        {
            return GetComment(Encoding.UTF8);
        }

        /// <summary>
        /// 获取注释。
        /// </summary>
        /// <param name="encoding">用于转换的字符编码。</param>
        /// <returns></returns>
        public string GetComment(Encoding encoding)
        {
            if (TryGetValue("comment", out BEncodeString value))
            {
                return value.GetStringValue(encoding);
            }
            return null;
        }

        /// <summary>
        /// 获取创建者。默认使用 Encoding.UTF8 编码。
        /// </summary>
        /// <returns></returns>
        public string GetCreatedBy()
        {
            return GetCreatedBy(Encoding.UTF8);
        }

        /// <summary>
        /// 获取创建者。
        /// </summary>
        /// <param name="encoding">用于转换的字符编码。</param>
        /// <returns></returns>
        public string GetCreatedBy(Encoding encoding)
        {
            if (TryGetValue("created by", out BEncodeString value))
            {
                return value.GetStringValue(encoding);
            }
            return null;
        }

        /// <summary>
        /// 获取创建时间。
        /// </summary>
        /// <returns></returns>
        public DateTime? GetCreationDate()
        {
            if (TryGetValue("creation date", out BEncodeInteger value))
            {
                return _start.AddSeconds(value.GetInt64Value());
            }
            return null;
        }

        /// <summary>
        /// 获取编码标识。
        /// </summary>
        /// <returns></returns>
        public string GetEncoding()
        {
            if (TryGetValue("encoding", out BEncodeString value))
            {
                return value.GetStringValue();
            }
            return null;
        }

        /// <summary>
        /// 获取所包含文件的信息。默认使用 Encoding.UTF8 编码。
        /// </summary>
        /// <returns></returns>
        public List<TorrentFileInfo> GetFiles()
        {
            return GetFiles(Encoding.UTF8, string.Empty, 0, long.MaxValue);
        }

        /// <summary>
        /// 获取所包含文件的信息。
        /// </summary>
        /// <param name="encoding">用于转换的字符编码。</param>
        /// <returns></returns>
        public List<TorrentFileInfo> GetFiles(Encoding encoding)
        {
            return GetFiles(encoding, string.Empty, 0, long.MaxValue);
        }

        /// <summary>
        /// 获取所包含文件的信息。
        /// </summary>
        /// <param name="encoding">用于转换的字符编码。</param>
        /// <param name="searchPattern">检索条件。使用正则表达式。</param>
        /// <returns></returns>
        public List<TorrentFileInfo> GetFiles(Encoding encoding, string searchPattern)
        {
            return GetFiles(encoding, searchPattern, 0, long.MaxValue);
        }

        /// <summary>
        /// 获取所包含文件的信息。
        /// </summary>
        /// <param name="encoding">用于转换的字符编码。</param>
        /// <param name="searchPattern">检索条件。使用正则表达式。</param>
        /// <param name="minSize">匹配最小文件大小。</param>
        /// <param name="maxSize">匹配最大文件大小。</param>
        /// <returns></returns>
        public List<TorrentFileInfo> GetFiles(Encoding encoding, string searchPattern, long minSize, long maxSize)
        {
            List<TorrentFileInfo> result = new List<TorrentFileInfo>();
            BEncodeDictionary info = (BEncodeDictionary)this["info"];
            if (info.TryGetValue("files", out BEncodeList files))
            {
                foreach (BEncodeDictionary file in files.Cast<BEncodeDictionary>())
                {
                    bool matched = false;
                    List<string> paths = new List<string>();
                    long length = -1;
                    byte[] md5sum = null;
                    foreach (BEncodeString path in ((BEncodeList)file["path"]).Cast<BEncodeString>())
                    {
                        paths.Add(path.GetStringValue(encoding));
                    }
                    if (!string.IsNullOrWhiteSpace(searchPattern))
                    {
                        foreach (string path in paths)
                        {
                            Match match = Regex.Match(path, searchPattern);
                            matched |= match.Success;
                        }
                    }
                    else
                    {
                        matched = true;
                    }
                    if (matched)
                    {
                        length = ((BEncodeInteger)file["length"]).GetInt64Value();
                        if (length >= minSize && length <= maxSize)
                        {
                            if (info.TryGetValue("md5sum", out BEncodeString md5))
                            {
                                md5sum = md5.Value;
                            }
                        }
                        else
                        {
                            matched = false;
                        }
                    }
                    if (matched)
                    {
                        result.Add(new TorrentFileInfo(paths.ToArray(), length, md5sum));
                    }
                }
            }
            else
            {
                bool matched = true;
                string path = ((BEncodeString)info["name"]).GetStringValue(encoding);
                long length = -1;
                byte[] md5sum = null;
                if (!string.IsNullOrWhiteSpace(searchPattern))
                {
                    Match match = Regex.Match(path, searchPattern);
                    matched = match.Success;
                }
                if (matched)
                {
                    length = ((BEncodeInteger)info["length"]).GetInt64Value();
                    if (length >= minSize && length <= maxSize)
                    {
                        if (info.TryGetValue("md5sum", out BEncodeString md5))
                        {
                            md5sum = md5.Value;
                        }
                    }
                    else
                    {
                        matched = false;
                    }
                }
                if (matched)
                {
                    result.Add(new TorrentFileInfo(new string[] { path }, length, md5sum));
                }
            }
            return result;
        }

        /// <summary>
        /// 获取 BTIH 特征码。
        /// </summary>
        /// <returns></returns>
        public byte[] GetHash()
        {
            BEncodeDictionary info = (BEncodeDictionary)this["info"];
            using (MemoryStream stream = new MemoryStream())
            {
                info.Save(stream);
                using (SHA1 hash = new SHA1Managed())
                {
                    stream.Seek(0, SeekOrigin.Begin);
                    return hash.ComputeHash(stream);
                }
            }
        }

        /// <summary>
        /// 获取 BTIH 特征码标识的磁力链接。
        /// </summary>
        /// <returns></returns>
        public string GetMagnet()
        {
            return GetMagnet(Encoding.UTF8, false, false, false);
        }

        /// <summary>
        /// 获取 BTIH 特征码标识的磁力链接。
        /// </summary>
        /// <param name="encoding">用于转换的字符编码。</param>
        /// <param name="dn">是否携带显示名称。</param>
        /// <param name="tr">是否携带主要 Tracker 服务器。</param>
        /// <param name="trs">是否携带备用 Tracker 服务器列表。</param>
        /// <returns></returns>
        public string GetMagnet(Encoding encoding, bool dn, bool tr, bool trs)
        {
            StringBuilder result = new StringBuilder();
            result.Append("magnet:?");
            BEncodeDictionary info = (BEncodeDictionary)this["info"];
            using (MemoryStream stream = new MemoryStream())
            {
                info.Save(stream);
                using (SHA1 hash = new SHA1Managed())
                {
                    stream.Seek(0, SeekOrigin.Begin);
                    byte[] checksum = hash.ComputeHash(stream);
                    result.Append("xt=urn:btih:");
                    result.Append(BitConverter.ToString(checksum, 0, checksum.Length).Replace("-", null));
                }
            }
            if (dn)
            {
                string name = ((BEncodeString)info["name"]).GetStringValue(encoding);
                string encoded = Uri.EscapeDataString(name);
                result.Append("&dn=");
                result.Append(encoded);
            }
            if (tr && TryGetValue("announce", out BEncodeString announce))
            {
                result.Append("&tr=");
                result.Append(announce.GetStringValue(encoding));
            }
            if (trs && TryGetValue("announce-list", out BEncodeList list))
            {
                foreach (BEncodeList list2 in list.Cast<BEncodeList>())
                {
                    foreach (BEncodeString entry in list2.Cast<BEncodeString>())
                    {
                        //result.Append("&tr=");
                        //string a = entry.GetStringValue(encoding);
                        //string encoded = Uri.EscapeDataString(a);
                        //result.Append(encoded);
                        result.Append("&tr=");
                        result.Append(entry.GetStringValue(encoding));
                    }
                }
            }
            return result.ToString();
        }

        /// <summary>
        /// 获取 BT 种子文件的文件名。默认使用 Encoding.UTF8 编码。
        /// </summary>
        /// <returns></returns>
        public string GetName()
        {
            return GetName(Encoding.UTF8);
        }

        /// <summary>
        /// 获取 BT 种子文件的文件名。
        /// </summary>
        /// <param name="encoding">用于转换的字符编码。</param>
        /// <returns></returns>
        public string GetName(Encoding encoding)
        {
            BEncodeDictionary info = (BEncodeDictionary)this["info"];
            return ((BEncodeString)info["name"]).GetStringValue(encoding);
        }

        /// <summary>
        /// 获取 DHT 初始节点。
        /// </summary>
        /// <returns></returns>
        public List<IPEndPoint> GetNodes()
        {
            List<IPEndPoint> result = new List<IPEndPoint>();
            if (TryGetValue("nodes", out BEncodeList list))
            {
                foreach (BEncodeList list2 in list.Cast<BEncodeList>())
                {
                    IPAddress ip = IPAddress.Parse(((BEncodeString)list2[0]).GetStringValue());
                    int port = ((BEncodeInteger)list2[1]).GetInt32Value();
                    IPEndPoint ep = new IPEndPoint(ip, port);
                    result.Add(ep);
                }
            }
            return result;
        }

        /// <summary>
        /// 获取分块大小。
        /// </summary>
        /// <returns></returns>
        public long GetPieceLength()
        {
            BEncodeDictionary info = (BEncodeDictionary)this["info"];
            return ((BEncodeInteger)info["piece length"]).GetInt64Value();
        }

        /// <summary>
        /// 获取分块的集成 Hash。
        /// </summary>
        ///         /// <returns></returns>
        public byte[] GetPieces()
        {
            BEncodeDictionary info = (BEncodeDictionary)this["info"];
            return ((BEncodeString)info["pieces"]).Value;
        }

        /// <summary>
        /// 获取发布者。默认使用 Encoding.UTF8 编码。
        /// </summary>
        /// <returns></returns>
        public string GetPublisher()
        {
            return GetPublisher(Encoding.UTF8);
        }

        /// <summary>
        /// 获取发布者。
        /// </summary>
        /// <param name="encoding">用于转换的字符编码。</param>
        /// <returns></returns>
        public string GetPublisher(Encoding encoding)
        {
            BEncodeDictionary info = (BEncodeDictionary)this["info"];
            if (info.TryGetValue("publisher", out BEncodeString value))
            {
                return value.GetStringValue(encoding);
            }
            return null;
        }

        /// <summary>
        /// 获取发布者 Url。默认使用 Encoding.UTF8 编码。
        /// </summary>
        /// <returns></returns>
        public string GetPublisherUrl()
        {
            return GetPublisherUrl(Encoding.UTF8);
        }

        /// <summary>
        /// 获取发布者 Url。
        /// </summary>
        /// <param name="encoding">用于转换的字符编码。</param>
        /// <returns></returns>
        public string GetPublisherUrl(Encoding encoding)
        {
            BEncodeDictionary info = (BEncodeDictionary)this["info"];
            if (info.TryGetValue("publisher-url", out BEncodeString value))
            {
                return value.GetStringValue(encoding);
            }
            return null;
        }
    }
}