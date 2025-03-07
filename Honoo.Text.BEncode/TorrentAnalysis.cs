﻿using System;
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
    /// Torrent 文件的根元素。从 <see cref="BEncodeDictionary"/> 继承并增加 Torrent 文件的相关方法。
    /// </summary>
    public class TorrentAnalysis : BEncodeDictionary, IReadOnlyTorrentAnalysis
    {
        #region Properties

        private readonly DateTime _start = DateTime.FromBinary(621355968000000000L);
        private bool _multiple;

        /// <summary>
        /// 获取一个值，指示此 Torrent 文件是否是多文件格式。
        /// </summary>
        public bool Multiple => _multiple;

        #endregion Properties

        #region Construction

        /// <summary>
        /// 初始化 TorrentAnalysis 类的新实例。
        /// </summary>
        public TorrentAnalysis() : base()
        {
        }

        /// <summary>
        /// 初始化 TorrentAnalysis 类的新实例。
        /// </summary>
        /// <param name="content">指定从中读取的流。定位必须在编码标记 <see langword="d"/> 处。</param>
        /// <exception cref="Exception"/>
        public TorrentAnalysis(Stream content) : this(content, false)
        {
        }

        /// <summary>
        /// 初始化 TorrentAnalysis 类的新实例。
        /// </summary>
        /// <param name="content">指定从中读取的流。定位必须在编码标记 <see langword="d"/> 处。</param>
        /// <param name="readOnly">指定此 <see cref="TorrentAnalysis"/> 是只读实例。</param>
        /// <exception cref="Exception"/>
        public TorrentAnalysis(Stream content, bool readOnly) : base(content, readOnly)
        {
            if (base.TryGetValue("info", out BEncodeDictionary info))
            {
                if (info.ContainsKey("files"))
                {
                    _multiple = true;
                }
            }
        }

        #endregion Construction

        #region Announce

        /// <summary>
        /// 获取主要 Tracker 服务器。如果元素不存在，返回 <see langword="null"/>。转换元素的键时默认使用 <see cref="Encoding.UTF8"/> 编码。
        /// </summary>
        /// <returns></returns>
        public string GetAnnounce()
        {
            return GetAnnounce(Encoding.UTF8);
        }

        /// <summary>
        /// 获取主要 Tracker 服务器。如果元素不存在，返回 <see langword="null"/>。
        /// </summary>
        /// <param name="valueEncoding">用于转换元素的值的字符编码。</param>
        /// <returns></returns>
        /// <exception cref="Exception"/>
        public string GetAnnounce(Encoding valueEncoding)
        {
            if (base.TryGetValue("announce", out BEncodeString value))
            {
                return value.GetStringValue(valueEncoding);
            }
            return null;
        }

        /// <summary>
        /// 设置主要 Tracker 服务器。转换元素的值时默认使用 <see cref="Encoding.UTF8"/> 编码。设置 <see langword="null"/> 移除此元素。
        /// </summary>
        /// <param name="announce">Tracker 服务器地址。设置 <see langword="null"/> 移除此元素。</param>
        public void SetAnnounce(string announce)
        {
            SetAnnounce(announce, Encoding.UTF8);
        }

        /// <summary>
        /// 设置主要 Tracker 服务器。设置 <see langword="null"/> 移除此元素。
        /// </summary>
        /// <param name="announce">Tracker 服务器地址。设置 <see langword="null"/> 移除此元素。</param>
        /// <param name="valueEncoding">用于转换元素的值的字符编码。</param>
        /// <exception cref="Exception"/>
        public void SetAnnounce(string announce, Encoding valueEncoding)
        {
            if (announce == null)
            {
                base.Remove("announce");
            }
            else
            {
                base.AddOrUpdate("announce", new BEncodeString(announce, valueEncoding));
            }
        }

        #endregion Announce

        #region Announce-List

        /// <summary>
        /// 获取备用 Tracker 服务器列表。如果元素不存在，返回 <see langword="null"/>。转换元素的值时默认使用 <see cref="Encoding.UTF8"/> 编码。
        /// </summary>
        /// <returns></returns>
        public string[][] GetAnnounceList()
        {
            return GetAnnounceList(Encoding.UTF8);
        }

        /// <summary>
        /// 获取备用 Tracker 服务器列表。如果元素不存在，返回 <see langword="null"/>。
        /// </summary>
        /// <param name="valueEncoding">用于转换元素的值的字符编码。</param>
        /// <returns></returns>
        /// <exception cref="Exception"/>
        public string[][] GetAnnounceList(Encoding valueEncoding)
        {
            if (base.TryGetValue("announce-list", out BEncodeList announceList))
            {
                List<string[]> result = new List<string[]>();
                foreach (BEncodeList group in announceList.Cast<BEncodeList>())
                {
                    List<string> res = new List<string>();
                    foreach (BEncodeString entry in group.Cast<BEncodeString>())
                    {
                        res.Add(entry.GetStringValue(valueEncoding));
                    }
                    result.Add(res.ToArray());
                }
                return result.ToArray();
            }
            return null;
        }

        /// <summary>
        /// 设置备用 Tracker 服务器列表。转换元素的值时默认使用 <see cref="Encoding.UTF8"/> 编码。设置 <see langword="null"/> 移除此元素。
        /// </summary>
        /// <param name="announceList">Tracker 服务器列表。设置 <see langword="null"/> 移除此元素。</param>
        public void SetAnnounceList(string[][] announceList)
        {
            SetAnnounceList(announceList, Encoding.UTF8);
        }

        /// <summary>
        /// 设置备用 Tracker 服务器列表。设置 <see langword="null"/> 移除此元素。
        /// </summary>
        /// <param name="announceList">Tracker 服务器列表。设置 <see langword="null"/> 移除此元素。</param>
        /// <param name="valueEncoding">用于转换元素的值的字符编码。</param>
        /// <exception cref="Exception"/>
        public void SetAnnounceList(string[][] announceList, Encoding valueEncoding)
        {
            if (announceList == null)
            {
                base.Remove("announce-list");
            }
            else
            {
                BEncodeList list = base.AddOrUpdate("announce-list", new BEncodeList());
                foreach (string[] group in announceList)
                {
                    BEncodeList list2 = list.Add(new BEncodeList());
                    foreach (string announce in group)
                    {
                        list2.Add(new BEncodeString(announce, valueEncoding));
                    }
                }
            }
        }

        #endregion Announce-List

        #region CreatedBy

        /// <summary>
        /// 获取创建者名称。如果元素不存在，返回 <see langword="null"/>。转换元素的值时默认使用 <see cref="Encoding.UTF8"/> 编码。
        /// </summary>
        /// <returns></returns>
        public string GetCreatedBy()
        {
            return GetCreatedBy(Encoding.UTF8);
        }

        /// <summary>
        /// 获取创建者名称。如果元素不存在，返回 <see langword="null"/>。
        /// </summary>
        /// <param name="valueEncoding">用于转换元素的值的字符编码。</param>
        /// <returns></returns>
        /// <exception cref="Exception"/>
        public string GetCreatedBy(Encoding valueEncoding)
        {
            if (base.TryGetValue("created by", out BEncodeString value))
            {
                return value.GetStringValue(valueEncoding);
            }
            return null;
        }

        /// <summary>
        /// 设置创建者名称。转换元素的值时默认使用 <see cref="Encoding.UTF8"/> 编码。设置 <see langword="null"/> 移除此元素。
        /// </summary>
        /// <param name="createdBy">创建者名称。设置 <see langword="null"/> 移除此元素。</param>
        public void SetCreatedBy(string createdBy)
        {
            SetCreatedBy(createdBy, Encoding.UTF8);
        }

        /// <summary>
        /// 设置创建者名称。设置 <see langword="null"/> 移除此元素。
        /// </summary>
        /// <param name="createdBy">创建者名称。设置 <see langword="null"/> 移除此元素。</param>
        /// <param name="valueEncoding">用于转换元素的值的字符编码。</param>
        /// <exception cref="Exception"/>
        public void SetCreatedBy(string createdBy, Encoding valueEncoding)
        {
            if (createdBy == null)
            {
                base.Remove("created by");
            }
            else
            {
                base.AddOrUpdate("created by", new BEncodeString(createdBy, valueEncoding));
            }
        }

        #endregion CreatedBy

        #region CreationDate

        /// <summary>
        /// 获取创建时间。如果元素不存在，返回 <see langword="null"/>。
        /// </summary>
        /// <returns></returns>
        public DateTime? GetCreationDate()
        {
            if (base.TryGetValue("creation date", out BEncodeInteger value))
            {
                return _start.AddSeconds(value.GetInt64Value());
            }
            return null;
        }

        /// <summary>
        /// 设置创建时间。设置 <see langword="null"/> 移除此元素。
        /// </summary>
        /// <param name="creationDate">创建时间。设置 <see langword="null"/> 移除此元素。</param>
        public void SetCreationDate(DateTime? creationDate)
        {
            if (creationDate == null)
            {
                base.Remove("creation date");
            }
            else
            {
                base.AddOrUpdate("creation date", new BEncodeInteger((long)(creationDate.Value - _start).TotalSeconds));
            }
        }

        #endregion CreationDate

        #region Encoding

        /// <summary>
        /// 获取编码标识。如果元素不存在，返回 <see langword="null"/>。转换元素的值时默认使用 <see cref="Encoding.UTF8"/> 编码。
        /// </summary>
        /// <returns></returns>
        public string GetEncoding()
        {
            return GetEncoding(Encoding.UTF8);
        }

        /// <summary>
        /// 获取编码标识。如果元素不存在，返回 <see langword="null"/>。
        /// </summary>
        /// <param name="valueEncoding">用于转换元素的值的字符编码。</param>
        /// <returns></returns>
        /// <exception cref="Exception"/>
        public string GetEncoding(Encoding valueEncoding)
        {
            if (base.TryGetValue("encoding", out BEncodeString value))
            {
                return value.GetStringValue(valueEncoding);
            }
            return null;
        }

        /// <summary>
        /// 设置编码标识。转换元素的值时默认使用 <see cref="Encoding.UTF8"/> 编码。设置 <see langword="null"/> 移除此元素。
        /// </summary>
        /// <param name="flag">编码标识。设置 <see langword="null"/> 移除此元素。</param>
        public void SetEncoding(string flag)
        {
            SetEncoding(flag, Encoding.UTF8);
        }

        /// <summary>
        /// 设置编码标识。设置 <see langword="null"/> 移除此元素。
        /// </summary>
        /// <param name="flag">编码标识。设置 <see langword="null"/> 移除此元素。</param>
        /// <param name="valueEncoding">用于转换元素的值的字符编码。</param>
        /// <exception cref="Exception"/>
        public void SetEncoding(string flag, Encoding valueEncoding)
        {
            if (flag == null)
            {
                base.Remove("encoding");
            }
            else
            {
                base.AddOrUpdate("encoding", new BEncodeString(flag, valueEncoding));
            }
        }

        #endregion Encoding

        #region Comment

        /// <summary>
        /// 获取注释。如果元素不存在，返回 <see langword="null"/>。转换元素的值时默认使用 <see cref="Encoding.UTF8"/> 编码。
        /// </summary>
        /// <returns></returns>
        public string GetComment()
        {
            return GetComment(Encoding.UTF8);
        }

        /// <summary>
        /// 获取注释。如果元素不存在，返回 <see langword="null"/>。
        /// </summary>
        /// <param name="valueEncoding">用于转换元素的值的字符编码。</param>
        /// <returns></returns>
        /// <exception cref="Exception"/>
        public string GetComment(Encoding valueEncoding)
        {
            if (base.TryGetValue("comment", out BEncodeString value))
            {
                return value.GetStringValue(valueEncoding);
            }
            return null;
        }

        /// <summary>
        /// 设置注释。转换元素的值时默认使用 <see cref="Encoding.UTF8"/> 编码。设置 <see langword="null"/> 移除此元素。
        /// </summary>
        /// <param name="comment">注释。设置 <see langword="null"/> 移除此元素。</param>
        public void SetComment(string comment)
        {
            SetComment(comment, Encoding.UTF8);
        }

        /// <summary>
        /// 设置注释。设置 <see langword="null"/> 移除此元素。
        /// </summary>
        /// <param name="comment">注释。设置 <see langword="null"/> 移除此元素。</param>
        /// <param name="valueEncoding">用于转换元素的值的字符编码。</param>
        /// <exception cref="Exception"/>
        public void SetComment(string comment, Encoding valueEncoding)
        {
            if (comment == null)
            {
                base.Remove("comment");
            }
            else
            {
                base.AddOrUpdate("comment", new BEncodeString(comment, valueEncoding));
            }
        }

        #endregion Comment

        #region Nodes

        /// <summary>
        /// 获取 DHT 初始节点。如果元素不存在，返回 <see langword="null"/>。
        /// </summary>
        /// <returns></returns>
        public IPEndPoint[] GetNodes()
        {
            if (base.TryGetValue("nodes", out BEncodeList nodes))
            {
                List<IPEndPoint> result = new List<IPEndPoint>();
                foreach (BEncodeList node in nodes.Cast<BEncodeList>())
                {
                    IPAddress ip = IPAddress.Parse(((BEncodeString)node[0]).GetStringValue());
                    int port = ((BEncodeInteger)node[1]).GetInt32Value();
                    IPEndPoint ep = new IPEndPoint(ip, port);
                    result.Add(ep);
                }
                return result.ToArray();
            }
            return null;
        }

        /// <summary>
        /// 设置 DHT 初始节点。设置 <see langword="null"/> 移除此元素。
        /// </summary>
        /// <param name="eps">DHT 节点地址。设置 <see langword="null"/> 移除此元素。</param>
        public void SetNodes(IPEndPoint[] eps)
        {
            if (eps == null)
            {
                base.Remove("nodes");
            }
            else if (eps.Length > 0)
            {
                if (!base.TryGetValue("nodes", out BEncodeList nodes))
                {
                    nodes = base.Add("nodes", new BEncodeList());
                }
                foreach (IPEndPoint ep in eps)
                {
                    nodes.Add(new BEncodeList() { new BEncodeString(ep.Address.ToString()), new BEncodeInteger(ep.Port) });
                }
            }
        }

        #endregion Nodes

        #region Name

        /// <summary>
        /// 获取推荐文件名。如果元素不存在，返回 <see langword="null"/>。转换元素的值时默认使用 <see cref="Encoding.UTF8"/> 编码。
        /// </summary>
        /// <returns></returns>
        public string GetName()
        {
            return GetName(Encoding.UTF8);
        }

        /// <summary>
        /// 获取推荐文件名。如果元素不存在，返回 <see langword="null"/>。
        /// </summary>
        /// <param name="valueEncoding">用于转换元素的值的字符编码。</param>
        /// <returns></returns>
        /// <exception cref="Exception"/>
        public string GetName(Encoding valueEncoding)
        {
            if (base.TryGetValue("info", out BEncodeDictionary info))
            {
                if (info.TryGetValue("name", out BEncodeString name))
                {
                    return name.GetStringValue(valueEncoding);
                }
            }
            return null;
        }

        /// <summary>
        /// 设置推荐文件名。转换元素的值时默认使用 <see cref="Encoding.UTF8"/> 编码。设置 <see langword="null"/> 移除此元素。添加文件后会重置此元素。
        /// </summary>
        /// <param name="name">推荐文件名。设置 <see langword="null"/> 移除此元素。</param>
        public void SetName(string name)
        {
            SetName(name, Encoding.UTF8);
        }

        /// <summary>
        /// 设置推荐文件名。设置 <see langword="null"/> 移除此元素。添加文件后会重置此元素。
        /// </summary>
        /// <param name="name">推荐文件名。设置 <see langword="null"/> 移除此元素。</param>
        /// <param name="valueEncoding">用于转换元素的值的字符编码。</param>
        /// <exception cref="Exception"/>
        public void SetName(string name, Encoding valueEncoding)
        {
            if (name == null)
            {
                if (base.TryGetValue("info", out BEncodeDictionary info))
                {
                    info.Remove("name");
                }
            }
            else
            {
                if (!base.TryGetValue("info", out BEncodeDictionary info))
                {
                    info = base.Add("info", new BEncodeDictionary());
                }
                info.AddOrUpdate("name", new BEncodeString(name, valueEncoding));
            }
        }

        #endregion Name

        #region Private

        /// <summary>
        /// 获取私有标记。如果元素不存在，返回 <see langword="null"/>。
        /// </summary>
        /// <returns></returns>
        public bool? GetPrivate()
        {
            if (base.TryGetValue("info", out BEncodeDictionary info))
            {
                if (info.TryGetValue("private", out BEncodeInteger priv))
                {
                    return priv.GetInt32Value() == 1;
                }
            }
            return null;
        }

        /// <summary>
        /// 设置私有标记。设置 <see langword="null"/> 移除此元素。
        /// </summary>
        /// <param name="flag">私有标记。设置 <see langword="null"/> 移除此元素。</param>
        public void SetPrivate(bool? flag)
        {
            if (flag == null)
            {
                if (base.TryGetValue("info", out BEncodeDictionary info))
                {
                    info.Remove("private");
                }
            }
            else
            {
                if (!base.TryGetValue("info", out BEncodeDictionary info))
                {
                    info = base.Add("info", new BEncodeDictionary());
                }
                info.AddOrUpdate("private", new BEncodeInteger(flag.Value ? 1 : 0));
            }
        }

        #endregion Private

        #region PieceLength

        /// <summary>
        /// 获取分块大小。如果元素不存在，返回 <see langword="null"/>。
        /// </summary>
        /// <returns></returns>
        public long? GetPieceLength()
        {
            if (base.TryGetValue("info", out BEncodeDictionary info))
            {
                if (info.TryGetValue("piece length", out BEncodeInteger pieceLength))
                {
                    return pieceLength.GetInt64Value();
                }
            }
            return null;
        }

        /// <summary>
        /// 设置分块大小。设置 <see langword="null"/> 移除此元素。此设置与 "pieces" 元素值相关，更改后必须重新添加文件以生成新的 "pieces" 元素。
        /// </summary>
        /// <param name="length">分块大小。设置 <see langword="null"/> 移除此元素。</param>
        public void SetPieceLength(long? length)
        {
            if (length == null)
            {
                if (base.TryGetValue("info", out BEncodeDictionary info))
                {
                    info.Remove("piece length");
                }
            }
            else
            {
                if (!base.TryGetValue("info", out BEncodeDictionary info))
                {
                    info = base.Add("info", new BEncodeDictionary());
                }
                info.AddOrUpdate("piece length", new BEncodeInteger(length.Value));
            }
        }

        #endregion PieceLength

        #region Pieces

        /// <summary>
        /// 获取分块的集成特征码。如果元素不存在，返回 <see langword="null"/>。
        /// </summary>
        /// <returns></returns>
        public byte[] GetPieces()
        {
            if (base.TryGetValue("info", out BEncodeDictionary info))
            {
                if (info.TryGetValue("pieces", out BEncodeString pieces))
                {
                    return pieces.Value;
                }
            }
            return null;
        }

        #endregion Pieces

        #region Hash

        /// <summary>
        /// 获取特征码。如果元素不存在，返回 <see langword="null"/>。
        /// </summary>
        /// <returns></returns>
        public string GetHash()
        {
            if (base.TryGetValue("hash", out BEncodeString hash))
            {
                return hash.GetStringValue();
            }
            return null;
        }

        /// <summary>
        /// 设置实时计算的 BTIH 特征码。特征码是 "info" 元素的 SHA1 校验值。
        /// </summary>
        /// <exception cref="Exception"/>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Globalization", "CA1308:将字符串规范化为大写", Justification = "<挂起>")]
        public void SetHash()
        {
            if (base.TryGetValue("info", out BEncodeDictionary info))
            {
                byte[] checksum = ComputeHash(info);
                string hash = BitConverter.ToString(checksum, 0, checksum.Length).Replace("-", null).ToLowerInvariant();
                base.AddOrUpdate("hash", new BEncodeString(hash));
            }
            else
            {
                throw new CryptographicException("Has not \"info\", cannot compute hash values.");
            }
        }

        /// <summary>
        /// 设置自定义特征码。设置 <see langword="null"/> 移除此元素。
        /// </summary>
        /// <param name="hashHex">自定义特征码的 Hex 字符串。设置 <see langword="null"/> 移除此元素。</param>
        public void SetHash(string hashHex)
        {
            if (hashHex == null)
            {
                base.Remove("hash");
            }
            else
            {
                base.AddOrUpdate("hash", new BEncodeString(hashHex));
            }
        }

        #endregion Hash

        #region Files

        /// <summary>
        /// 获取所包含文件的信息。转换元素的值时默认使用 <see cref="Encoding.UTF8"/> 编码。
        /// </summary>
        /// <returns></returns>
        public IList<TorrentFileEntry> GetFiles()
        {
            return GetFiles(string.Empty, 0, long.MaxValue);
        }

        /// <summary>
        /// 获取所包含文件的信息。转换元素的值时默认使用 <see cref="Encoding.UTF8"/> 编码。
        /// </summary>
        /// <param name="searchPattern">文件名称检索条件（包括路径，路径使用 "/" 分隔符）。支持 * ? 通配符，不可使用正则表达式。</param>
        /// <param name="minSize">匹配最小文件大小。</param>
        /// <param name="maxSize">匹配最大文件大小。</param>
        /// <returns></returns>
        public IList<TorrentFileEntry> GetFiles(string searchPattern, long minSize, long maxSize)
        {
            return GetFiles(searchPattern, minSize, maxSize, Encoding.UTF8);
        }

        /// <summary>
        /// 获取所包含文件的信息。
        /// </summary>
        /// <param name="searchPattern">文件名称检索条件（包括路径，路径使用 "/" 分隔符）。支持 * ? 通配符，不可使用正则表达式。</param>
        /// <param name="minSize">匹配最小文件大小。</param>
        /// <param name="maxSize">匹配最大文件大小。</param>
        /// <param name="valueEncoding">用于转换元素的值的字符编码。</param>
        /// <returns></returns>
        /// <exception cref="Exception"/>
        public IList<TorrentFileEntry> GetFiles(string searchPattern, long minSize, long maxSize, Encoding valueEncoding)
        {
            if (!string.IsNullOrWhiteSpace(searchPattern))
            {
                searchPattern = ConvertPattern(searchPattern);
            }
            List<TorrentFileEntry> result = new List<TorrentFileEntry>();
            if (base.TryGetValue("info", out BEncodeDictionary info))
            {
                if (info.TryGetValue("files", out BEncodeList files))
                {
                    int index = -1;
                    foreach (BEncodeDictionary file in files.Cast<BEncodeDictionary>())
                    {
                        index++;
                        bool matched = true;
                        long len = ((BEncodeInteger)file["length"]).GetInt64Value();
                        if (len < minSize || len > maxSize)
                        {
                            matched = false;
                        }
                        if (matched && !string.IsNullOrWhiteSpace(searchPattern))
                        {
                            List<string> paths = new List<string>();
                            foreach (BEncodeString path in file.GetValue<BEncodeList>("path").Cast<BEncodeString>())
                            {
                                paths.Add(path.GetStringValue(valueEncoding));
                            }
                            string p = string.Join("/", paths);
                            Match match = Regex.Match(p, searchPattern);
                            matched = match.Success;
                        }
                        if (matched)
                        {
                            result.Add(new TorrentFileEntry(_multiple, file, index));
                        }
                    }
                }
                else if (info.TryGetValue("name", out BEncodeString name) && info.TryGetValue("length", out BEncodeInteger length))
                {
                    bool matched = true;
                    long len = length.GetInt64Value();
                    if (len < minSize || len > maxSize)
                    {
                        matched = false;
                    }
                    if (matched && !string.IsNullOrWhiteSpace(searchPattern))
                    {
                        string p = name.GetStringValue(valueEncoding);
                        Match match = Regex.Match(p, searchPattern);
                        matched = match.Success;
                    }
                    if (matched)
                    {
                        result.Add(new TorrentFileEntry(_multiple, info, 0));
                    }
                }
            }
            return result;
        }

        /// <summary>
        /// 设置单文件。添加文件后会重置 "name", "pieces" 等元素值。设置 <see langword="null"/> 移除所有文件相关元素，无论之前存在的是单文件格式还是多文件格式。
        /// 转换元素的值时默认使用 <see cref="Encoding.UTF8"/> 编码。
        /// </summary>
        /// <param name="fileName">本地文件。</param>
        public void SetFile(string fileName)
        {
            SetFile(fileName, Encoding.UTF8);
        }

        /// <summary>
        /// 设置单文件。添加文件后会重置 "name", "pieces" 等元素值。设置 <see langword="null"/> 移除所有文件相关元素，无论之前存在的是单文件格式还是多文件格式。
        /// </summary>
        /// <param name="fileName">本地文件。</param>
        /// <param name="valueEncoding">用于转换元素的值的字符编码。</param>
        /// <exception cref="Exception"/>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Security", "CA5350:不要使用弱加密算法", Justification = "<挂起>")]
        public void SetFile(string fileName, Encoding valueEncoding)
        {
            if (fileName == null)
            {
                if (base.TryGetValue("info", out BEncodeDictionary info))
                {
                    //info.Remove("name");
                    info.Remove("length");
                    info.Remove("pieces");
                    info.Remove("files");
                }
                _multiple = false;
            }
            else
            {
                _multiple = false;
                FileInfo fileInfo = new FileInfo(fileName);
                if (!base.TryGetValue("info", out BEncodeDictionary info))
                {
                    info = base.Add("info", new BEncodeDictionary());
                }
                info.Remove("files");
                info.AddOrUpdate("name", new BEncodeString(fileInfo.Name, valueEncoding));
                info.AddOrUpdate("length", new BEncodeInteger(fileInfo.Length));
                long pl = 16 * 1024 * 1024;
                if (info.TryGetValue("piece length", out BEncodeInteger pieceLength))
                {
                    pl = pieceLength.GetInt64Value();
                }
                else
                {
                    info.Add("piece length", new BEncodeInteger(pl));
                }
                using (SHA1 sha1 = SHA1.Create())
                {
                    List<byte> pieces = new List<byte>();
                    byte[] buffer = new byte[pl];
                    int index = 0;
                    ComputeHash(sha1, fileInfo, buffer, ref index, pieces);
                    if (index > 0)
                    {
                        pieces.AddRange(sha1.ComputeHash(buffer, 0, index));
                    }
                    info.AddOrUpdate("pieces", new BEncodeString(pieces.ToArray()));
                }
            }
        }

        /// <summary>
        /// 设置复数文件。添加文件后会重置 "name", "pieces" 等元素值。设置 <see langword="null"/> 移除所有文件相关元素，无论之前存在的是单文件格式还是多文件格式。
        /// 转换元素的值时默认使用 <see cref="Encoding.UTF8"/> 编码。
        /// </summary>
        /// <param name="folder">选择本地文件夹，添加其中所有文件。</param>
        /// <exception cref="Exception"/>
        public void SetFiles(string folder)
        {
            SetFiles(folder, Encoding.UTF8);
        }

        /// <summary>
        /// 设置复数文件。添加文件后会重置 "name", "pieces" 等元素值。设置 <see langword="null"/> 移除所有文件相关元素，无论之前存在的是单文件格式还是多文件格式。
        /// </summary>
        /// <param name="folder">选择本地文件夹，添加其中所有文件。</param>
        /// <param name="valueEncoding">用于转换元素的值的字符编码。</param>
        /// <exception cref="Exception"/>
        public void SetFiles(string folder, Encoding valueEncoding)
        {
            SetFiles(folder, valueEncoding, null, null);
        }

        /// <summary>
        /// 设置复数文件。添加文件后会重置 "name", "pieces" 等元素值。设置 <see langword="null"/> 移除所有文件相关元素，无论之前存在的是单文件格式还是多文件格式。
        /// </summary>
        /// <param name="folder">选择本地文件夹，添加其中所有文件。</param>
        /// <param name="valueEncoding">用于转换元素的值的字符编码。</param>
        /// <param name="callback">添加一个文件成功后执行。</param>
        /// <param name="userState">在回调中传递参数。</param>
        /// <exception cref="Exception"/>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Security", "CA5350:不要使用弱加密算法", Justification = "<挂起>")]
        public void SetFiles(string folder, Encoding valueEncoding, TorrentFileEntryAddedCallback callback, object userState)
        {
            if (folder == null)
            {
                if (base.TryGetValue("info", out BEncodeDictionary info))
                {
                    //info.Remove("name");
                    info.Remove("length");
                    info.Remove("pieces");
                    info.Remove("files");
                }
                _multiple = false;
            }
            else
            {
                DirectoryInfo dir = new DirectoryInfo(folder);
                List<FileInfo> fileList = new List<FileInfo>();
                SearchFile(dir, fileList);
                if (fileList.Count > 0)
                {
                    _multiple = true;
                    if (!base.TryGetValue("info", out BEncodeDictionary info))
                    {
                        info = base.Add("info", new BEncodeDictionary());
                    }
                    info.Remove("length");
                    info.AddOrUpdate("name", new BEncodeString(dir.Name, valueEncoding));
                    if (!info.TryGetValue("files", out BEncodeList files))
                    {
                        files = info.Add("files", new BEncodeList());
                    }
                    long pl = 16 * 1024 * 1024;
                    if (info.TryGetValue("piece length", out BEncodeInteger pieceLength))
                    {
                        pl = pieceLength.GetInt64Value();
                    }
                    else
                    {
                        info.Add("piece length", new BEncodeInteger(pl));
                    }
                    using (SHA1 sha1 = SHA1.Create())
                    {
                        List<byte> pieces = new List<byte>();
                        byte[] buffer = new byte[pl];
                        int index = 0;
                        int i = 0;
                        foreach (FileInfo fi in fileList)
                        {
                            BEncodeDictionary entry = files.Add(new BEncodeDictionary());
                            BEncodeList path = entry.Add("path", new BEncodeList());
                            string en = fi.FullName.Remove(0, dir.FullName.Length).Replace('\\', '/').Trim('/');
                            string[] paths = en.Split('/');
                            foreach (string p in paths)
                            {
                                path.Add(new BEncodeString(p, valueEncoding));
                            }
                            entry.Add("length", new BEncodeInteger(fi.Length));
                            ComputeHash(sha1, fi, buffer, ref index, pieces);
                            callback?.Invoke(entry, i, fileList.Count, userState);
                            i++;
                        }
                        if (index > 0)
                        {
                            pieces.AddRange(sha1.ComputeHash(buffer, 0, index));
                        }
                        info.AddOrUpdate("pieces", new BEncodeString(pieces.ToArray()));
                    }
                }
                else
                {
                    if (base.TryGetValue("info", out BEncodeDictionary info))
                    {
                        //info.Remove("name");
                        info.Remove("length");
                        info.Remove("pieces");
                        info.Remove("files");
                    }
                    _multiple = false;
                }
            }
        }

        private static string ConvertPattern(string pattern)
        {
            return pattern.Trim().Replace("\\", "/")
                .Replace("(", "\\(").Replace(")", "\\)").Replace("[", "\\[").Replace("{", "\\{")
                .Replace("^", "\\^").Replace("+", "\\+").Replace("|", "\\|").Replace("$", "\\$")
                .Replace(".", "\\.").Replace("*", ".*").Replace("?", ".?");
        }

        private static void SearchFile(DirectoryInfo dir, List<FileInfo> fileList)
        {
            DirectoryInfo[] dirList = dir.GetDirectories();
            if (dirList.Length > 0)
            {
                foreach (DirectoryInfo d in dirList)
                {
                    SearchFile(d, fileList);
                }
            }
            fileList.AddRange(dir.GetFiles());
        }

        #endregion Files

        #region Magnet

        /// <summary>
        /// 实时计算 BTIH 特征码标识的磁力链接。转换元素的值时默认使用 <see cref="Encoding.UTF8"/> 编码。
        /// </summary>
        /// <returns></returns>
        public string GetMagnet()
        {
            return GetMagnet(false, false, false, false);
        }

        /// <summary>
        /// 实时计算 BTIH 特征码标识的磁力链接。转换元素的值时默认使用 <see cref="Encoding.UTF8"/> 编码。
        /// </summary>
        /// <param name="dn">是否携带显示名称。</param>
        /// <param name="xl">是否携带文件长度数值。</param>
        /// <param name="tr">是否携带主要 Tracker 服务器。</param>
        /// <param name="trs">是否携带备用 Tracker 服务器列表。</param>
        /// <returns></returns>
        /// <exception cref="Exception"/>
        public string GetMagnet(bool dn, bool xl, bool tr, bool trs)
        {
            return GetMagnet(dn, xl, tr, trs, Encoding.UTF8);
        }

        /// <summary>
        /// 实时计算 BTIH 特征码标识的磁力链接。
        /// </summary>
        /// <param name="dn">是否携带显示名称。</param>
        /// <param name="xl">是否携带文件长度数值。</param>
        /// <param name="tr">是否携带主要 Tracker 服务器。</param>
        /// <param name="trs">是否携带备用 Tracker 服务器列表。</param>
        /// <param name="valueEncoding">用于转换元素的值的字符编码。</param>
        /// <returns></returns>
        /// <exception cref="Exception"/>
        public string GetMagnet(bool dn, bool xl, bool tr, bool trs, Encoding valueEncoding)
        {
            if (base.TryGetValue("info", out BEncodeDictionary info))
            {
                StringBuilder result = new StringBuilder();
                result.Append("magnet:?xt=urn:btih:");
                byte[] checksum = ComputeHash(info);
                result.Append(BitConverter.ToString(checksum, 0, checksum.Length).Replace("-", null));
                if (dn && info.TryGetValue("name", out BEncodeString name))
                {
                    result.Append("&dn=");
                    result.Append(Uri.EscapeDataString(name.GetStringValue(valueEncoding)));
                }
                if (xl)
                {
                    long len = 0;
                    if (info.TryGetValue("files", out BEncodeList files))
                    {
                        foreach (BEncodeDictionary file in files.Cast<BEncodeDictionary>())
                        {
                            if (file.TryGetValue("length", out BEncodeInteger length))
                            {
                                len += length.GetInt64Value();
                            }
                        }
                        result.Append("&xl=");
                        result.Append(len);
                    }
                    else if (info.TryGetValue("length", out BEncodeInteger length))
                    {
                        result.Append("&xl=");
                        result.Append(length.GetInt64Value());
                    }
                }
                if (tr && TryGetValue("announce", out BEncodeString announce))
                {
                    result.Append("&tr=");
                    result.Append(announce.GetStringValue(valueEncoding));
                }
                if (trs && TryGetValue("announce-list", out BEncodeList announceList))
                {
                    foreach (BEncodeList group in announceList.Cast<BEncodeList>())
                    {
                        foreach (BEncodeString entry in group.Cast<BEncodeString>())
                        {
                            result.Append("&tr=");
                            result.Append(Uri.EscapeDataString(entry.GetStringValue(valueEncoding)));
                        }
                    }
                }
                return result.ToString();
            }
            return string.Empty;
        }

        #endregion Magnet

        /// <summary>
        /// 获取此实例的只读接口。
        /// </summary>
        public new IReadOnlyTorrentAnalysis AsReadOnly()
        {
            return this;
        }

        /// <summary>
        /// 设置创建者名称和创建时间，同时实时计算 BTIH 特征码，保存到指定的流。
        /// </summary>
        /// <param name="stream">指定保存的目标流。</param>
        /// <param name="createdBy">创建者名称。设置 <see langword="null"/> 移除此元素。</param>
        /// <param name="creationDate">创建时间。设置 <see langword="null"/> 移除此元素。</param>
        /// <param name="renewHash">更新为实时计算的 BTIH 特征码。</param>
        /// <exception cref="Exception"/>
        public void Save(Stream stream, string createdBy, DateTime? creationDate, bool renewHash)
        {
            if (base.IsReadOnly)
            {
                throw new NotSupportedException("Collection is read-only.");
            }
            SetCreatedBy(createdBy);
            SetCreationDate(creationDate);
            if (renewHash)
            {
                SetHash();
            }
            base.Save(stream);
        }

        /// <summary>
        /// 设置创建者名称、创建时间和自定义特征码，保存到指定的流。
        /// </summary>
        /// <param name="stream">指定保存的目标流。</param>
        /// <param name="createdBy">创建者名称。设置 <see langword="null"/> 移除此元素。</param>
        /// <param name="creationDate">创建时间。设置 <see langword="null"/> 移除此元素。</param>
        /// <param name="hashHex">自定义特征码的 Hex 字符串。设置 <see langword="null"/> 移除此元素。</param>
        /// <exception cref="Exception"/>
        public void Save(Stream stream, string createdBy, DateTime? creationDate, string hashHex)
        {
            if (base.IsReadOnly)
            {
                throw new NotSupportedException("Collection is read-only.");
            }
            SetCreatedBy(createdBy);
            SetCreationDate(creationDate);
            SetHash(hashHex);
            base.Save(stream);
        }

        private static void ComputeHash(SHA1 sha1, FileInfo fileInfo, byte[] buffer, ref int index, List<byte> pieces)
        {
            using (FileStream stream = fileInfo.OpenRead())
            {
                while (true)
                {
                    int count = stream.Read(buffer, index, buffer.Length - index);
                    if (count > 0)
                    {
                        index += count;
                        if (index == buffer.Length)
                        {
                            pieces.AddRange(sha1.ComputeHash(buffer));
                            index = 0;
                            continue;
                        }
                    }
                    return;
                }
            }
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Security", "CA5350:不要使用弱加密算法", Justification = "<挂起>")]
        private static byte[] ComputeHash(BEncodeDictionary info)
        {
            using (MemoryStream stream = new MemoryStream())
            {
                info.Save(stream);
                stream.Seek(0, SeekOrigin.Begin);
                using (SHA1 hash = SHA1.Create())
                {
                    return hash.ComputeHash(stream);
                }
            }
        }
    }
}