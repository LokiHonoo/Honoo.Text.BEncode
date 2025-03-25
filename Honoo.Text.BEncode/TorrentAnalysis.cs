using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;

namespace Honoo.Text.BEncode
{
    /// <summary>
    /// Torrent 文件的根元素。从 <see cref="BEncodeDictionary"/> 继承并增加 Torrent 文件的相关方法。
    /// </summary>
    public class TorrentAnalysis : BEncodeDocument
    {
        #region Members

        private readonly DateTime _start = DateTime.FromBinary(621355968000000000L);
        private bool _multiple;

        /// <summary>
        /// 获取一个值，指示此 Torrent 文件是否是多文件格式。
        /// </summary>
        public bool Multiple => _multiple;

        #endregion Members

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
        public TorrentAnalysis(Stream content) : base(content)
        {
            if (base.Root.TryGetValue("info", out BEncodeDictionary info))
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
        /// 获取主要 Tracker 服务器。如果元素不存在，返回 <see langword="null"/>。转换元素的键时默认使用 <see cref="BEncodeDocument.Encoding"/> 编码。
        /// </summary>
        /// <returns></returns>
        public string GetAnnounce()
        {
            return GetAnnounce(base.Encoding);
        }

        /// <summary>
        /// 获取主要 Tracker 服务器。如果元素不存在，返回 <see langword="null"/>。
        /// </summary>
        /// <param name="valueEncoding">用于转换元素的值的字符编码。不使用所属的 <see cref="BEncodeDocument"/> 实例的默认字符编码。</param>
        /// <returns></returns>
        /// <exception cref="Exception"/>
        public string GetAnnounce(Encoding valueEncoding)
        {
            if (base.Root.TryGetValue("announce", out BEncodeString value))
            {
                return value.GetStringValue(valueEncoding);
            }
            return null;
        }

        /// <summary>
        /// 设置主要 Tracker 服务器。转换元素的值时默认使用 <see cref="BEncodeDocument.Encoding"/> 编码。设置 <see langword="null"/> 移除此元素。
        /// </summary>
        /// <param name="announce">Tracker 服务器地址。设置 <see langword="null"/> 移除此元素。</param>
        public void SetAnnounce(string announce)
        {
            SetAnnounce(announce, base.Encoding);
        }

        /// <summary>
        /// 设置主要 Tracker 服务器。设置 <see langword="null"/> 移除此元素。
        /// </summary>
        /// <param name="announce">Tracker 服务器地址。设置 <see langword="null"/> 移除此元素。</param>
        /// <param name="valueEncoding">用于转换元素的值的字符编码。不使用所属的 <see cref="BEncodeDocument"/> 实例的默认字符编码。</param>
        /// <exception cref="Exception"/>
        public void SetAnnounce(string announce, Encoding valueEncoding)
        {
            if (announce == null)
            {
                base.Root.Remove("announce");
            }
            else
            {
                base.Root.AddOrUpdate("announce", new BEncodeString(announce, valueEncoding, this));
            }
        }

        #endregion Announce

        #region Announce-List

        /// <summary>
        /// 获取备用 Tracker 服务器列表。如果元素不存在，返回 <see langword="null"/>。转换元素的值时默认使用 <see cref="BEncodeDocument.Encoding"/> 编码。
        /// </summary>
        /// <returns></returns>
        public string[][] GetAnnounceList()
        {
            return GetAnnounceList(base.Encoding);
        }

        /// <summary>
        /// 获取备用 Tracker 服务器列表。如果元素不存在，返回 <see langword="null"/>。
        /// </summary>
        /// <param name="valueEncoding">用于转换元素的值的字符编码。不使用所属的 <see cref="BEncodeDocument"/> 实例的默认字符编码。</param>
        /// <returns></returns>
        /// <exception cref="Exception"/>
        public string[][] GetAnnounceList(Encoding valueEncoding)
        {
            if (base.Root.TryGetValue("announce-list", out BEncodeList announceList))
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
        /// 设置备用 Tracker 服务器列表。转换元素的值时默认使用 <see cref="BEncodeDocument.Encoding"/> 编码。设置 <see langword="null"/> 移除此元素。
        /// </summary>
        /// <param name="announceList">Tracker 服务器列表。设置 <see langword="null"/> 移除此元素。</param>
        public void SetAnnounceList(string[][] announceList)
        {
            SetAnnounceList(announceList, base.Encoding);
        }

        /// <summary>
        /// 设置备用 Tracker 服务器列表。设置 <see langword="null"/> 移除此元素。
        /// </summary>
        /// <param name="announceList">Tracker 服务器列表。设置 <see langword="null"/> 移除此元素。</param>
        /// <param name="valueEncoding">用于转换元素的值的字符编码。不使用所属的 <see cref="BEncodeDocument"/> 实例的默认字符编码。</param>
        /// <exception cref="Exception"/>
        public void SetAnnounceList(string[][] announceList, Encoding valueEncoding)
        {
            if (announceList == null)
            {
                base.Root.Remove("announce-list");
            }
            else
            {
                BEncodeList list = base.Root.AddOrUpdate("announce-list", new BEncodeList(this));
                foreach (string[] group in announceList)
                {
                    BEncodeList list2 = list.Add(new BEncodeList(this));
                    foreach (string announce in group)
                    {
                        list2.Add(new BEncodeString(announce, valueEncoding, this));
                    }
                }
            }
        }

        #endregion Announce-List

        #region CreatedBy

        /// <summary>
        /// 获取创建者名称。如果元素不存在，返回 <see langword="null"/>。转换元素的值时默认使用 <see cref="BEncodeDocument.Encoding"/> 编码。
        /// </summary>
        /// <returns></returns>
        public string GetCreatedBy()
        {
            return GetCreatedBy(base.Encoding);
        }

        /// <summary>
        /// 获取创建者名称。如果元素不存在，返回 <see langword="null"/>。
        /// </summary>
        /// <param name="valueEncoding">用于转换元素的值的字符编码。不使用所属的 <see cref="BEncodeDocument"/> 实例的默认字符编码。</param>
        /// <returns></returns>
        /// <exception cref="Exception"/>
        public string GetCreatedBy(Encoding valueEncoding)
        {
            if (base.Root.TryGetValue("created by", out BEncodeString value))
            {
                return value.GetStringValue(valueEncoding);
            }
            return null;
        }

        /// <summary>
        /// 设置创建者名称。转换元素的值时默认使用 <see cref="BEncodeDocument.Encoding"/> 编码。设置 <see langword="null"/> 移除此元素。
        /// </summary>
        /// <param name="createdBy">创建者名称。设置 <see langword="null"/> 移除此元素。</param>
        public void SetCreatedBy(string createdBy)
        {
            SetCreatedBy(createdBy, base.Encoding);
        }

        /// <summary>
        /// 设置创建者名称。设置 <see langword="null"/> 移除此元素。
        /// </summary>
        /// <param name="createdBy">创建者名称。设置 <see langword="null"/> 移除此元素。</param>
        /// <param name="valueEncoding">用于转换元素的值的字符编码。不使用所属的 <see cref="BEncodeDocument"/> 实例的默认字符编码。</param>
        /// <exception cref="Exception"/>
        public void SetCreatedBy(string createdBy, Encoding valueEncoding)
        {
            if (createdBy == null)
            {
                base.Root.Remove("created by");
            }
            else
            {
                base.Root.AddOrUpdate("created by", new BEncodeString(createdBy, valueEncoding, this));
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
            if (base.Root.TryGetValue("creation date", out BEncodeInteger value))
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
                base.Root.Remove("creation date");
            }
            else
            {
                base.Root.AddOrUpdate("creation date", new BEncodeInteger((long)(creationDate.Value - _start).TotalSeconds, this));
            }
        }

        #endregion CreationDate

        #region Encoding

        /// <summary>
        /// 获取编码标识。如果元素不存在，返回 <see langword="null"/>。转换元素的值时默认使用 <see cref="BEncodeDocument.Encoding"/> 编码。
        /// </summary>
        /// <returns></returns>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Naming", "CA1721:属性名不应与 get 方法匹配", Justification = "<挂起>")]
        public string GetEncoding()
        {
            return GetEncoding(base.Encoding);
        }

        /// <summary>
        /// 获取编码标识。如果元素不存在，返回 <see langword="null"/>。
        /// </summary>
        /// <param name="valueEncoding">用于转换元素的值的字符编码。不使用所属的 <see cref="BEncodeDocument"/> 实例的默认字符编码。</param>
        /// <returns></returns>
        /// <exception cref="Exception"/>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Naming", "CA1721:属性名不应与 get 方法匹配", Justification = "<挂起>")]
        public string GetEncoding(Encoding valueEncoding)
        {
            if (base.Root.TryGetValue("encoding", out BEncodeString value))
            {
                return value.GetStringValue(valueEncoding);
            }
            return null;
        }

        /// <summary>
        /// 设置编码标识。转换元素的值时默认使用 <see cref="BEncodeDocument.Encoding"/> 编码。设置 <see langword="null"/> 移除此元素。
        /// </summary>
        /// <param name="flag">编码标识。设置 <see langword="null"/> 移除此元素。</param>
        public void SetEncoding(string flag)
        {
            SetEncoding(flag, base.Encoding);
        }

        /// <summary>
        /// 设置编码标识。设置 <see langword="null"/> 移除此元素。
        /// </summary>
        /// <param name="flag">编码标识。设置 <see langword="null"/> 移除此元素。</param>
        /// <param name="valueEncoding">用于转换元素的值的字符编码。不使用所属的 <see cref="BEncodeDocument"/> 实例的默认字符编码。</param>
        /// <exception cref="Exception"/>
        public void SetEncoding(string flag, Encoding valueEncoding)
        {
            if (flag == null)
            {
                base.Root.Remove("encoding");
            }
            else
            {
                base.Root.AddOrUpdate("encoding", new BEncodeString(flag, valueEncoding, this));
            }
        }

        #endregion Encoding

        #region Comment

        /// <summary>
        /// 获取注释。如果元素不存在，返回 <see langword="null"/>。转换元素的值时默认使用 <see cref="BEncodeDocument.Encoding"/> 编码。
        /// </summary>
        /// <returns></returns>
        public string GetComment()
        {
            return GetComment(base.Encoding);
        }

        /// <summary>
        /// 获取注释。如果元素不存在，返回 <see langword="null"/>。
        /// </summary>
        /// <param name="valueEncoding">用于转换元素的值的字符编码。不使用所属的 <see cref="BEncodeDocument"/> 实例的默认字符编码。</param>
        /// <returns></returns>
        /// <exception cref="Exception"/>
        public string GetComment(Encoding valueEncoding)
        {
            if (base.Root.TryGetValue("comment", out BEncodeString value))
            {
                return value.GetStringValue(valueEncoding);
            }
            return null;
        }

        /// <summary>
        /// 设置注释。转换元素的值时默认使用 <see cref="BEncodeDocument.Encoding"/> 编码。设置 <see langword="null"/> 移除此元素。
        /// </summary>
        /// <param name="comment">注释。设置 <see langword="null"/> 移除此元素。</param>
        public void SetComment(string comment)
        {
            SetComment(comment, base.Encoding);
        }

        /// <summary>
        /// 设置注释。设置 <see langword="null"/> 移除此元素。
        /// </summary>
        /// <param name="comment">注释。设置 <see langword="null"/> 移除此元素。</param>
        /// <param name="valueEncoding">用于转换元素的值的字符编码。不使用所属的 <see cref="BEncodeDocument"/> 实例的默认字符编码。</param>
        /// <exception cref="Exception"/>
        public void SetComment(string comment, Encoding valueEncoding)
        {
            if (comment == null)
            {
                base.Root.Remove("comment");
            }
            else
            {
                base.Root.AddOrUpdate("comment", new BEncodeString(comment, valueEncoding, this));
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
            if (base.Root.TryGetValue("nodes", out BEncodeList nodes))
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
                base.Root.Remove("nodes");
            }
            else if (eps.Length > 0)
            {
                if (!base.Root.TryGetValue("nodes", out BEncodeList nodes))
                {
                    nodes = base.Root.Add("nodes", new BEncodeList(this));
                }
                foreach (IPEndPoint ep in eps)
                {
                    nodes.Add(new BEncodeList(this) { new BEncodeString(ep.Address.ToString(), this), new BEncodeInteger(ep.Port, this) });
                }
            }
        }

        #endregion Nodes

        #region Name

        /// <summary>
        /// 获取推荐文件名。如果元素不存在，返回 <see langword="null"/>。转换元素的值时默认使用 <see cref="BEncodeDocument.Encoding"/> 编码。
        /// </summary>
        /// <returns></returns>
        public string GetName()
        {
            return GetName(base.Encoding);
        }

        /// <summary>
        /// 获取推荐文件名。如果元素不存在，返回 <see langword="null"/>。
        /// </summary>
        /// <param name="valueEncoding">用于转换元素的值的字符编码。不使用所属的 <see cref="BEncodeDocument"/> 实例的默认字符编码。</param>
        /// <returns></returns>
        /// <exception cref="Exception"/>
        public string GetName(Encoding valueEncoding)
        {
            if (base.Root.TryGetValue("info", out BEncodeDictionary info))
            {
                if (info.TryGetValue("name", out BEncodeString name))
                {
                    return name.GetStringValue(valueEncoding);
                }
            }
            return null;
        }

        /// <summary>
        /// 设置推荐文件名。转换元素的值时默认使用 <see cref="BEncodeDocument.Encoding"/> 编码。设置 <see langword="null"/> 移除此元素。添加文件后会重置此元素。
        /// </summary>
        /// <param name="name">推荐文件名。设置 <see langword="null"/> 移除此元素。</param>
        public void SetName(string name)
        {
            SetName(name, base.Encoding);
        }

        /// <summary>
        /// 设置推荐文件名。设置 <see langword="null"/> 移除此元素。添加文件后会重置此元素。
        /// </summary>
        /// <param name="name">推荐文件名。设置 <see langword="null"/> 移除此元素。</param>
        /// <param name="valueEncoding">用于转换元素的值的字符编码。不使用所属的 <see cref="BEncodeDocument"/> 实例的默认字符编码。</param>
        /// <exception cref="Exception"/>
        public void SetName(string name, Encoding valueEncoding)
        {
            if (name == null)
            {
                if (base.Root.TryGetValue("info", out BEncodeDictionary info))
                {
                    info.Remove("name");
                }
            }
            else
            {
                if (!base.Root.TryGetValue("info", out BEncodeDictionary info))
                {
                    info = base.Root.Add("info", new BEncodeDictionary(this));
                }
                info.AddOrUpdate("name", new BEncodeString(name, valueEncoding, this));
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
            if (base.Root.TryGetValue("info", out BEncodeDictionary info))
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
                if (base.Root.TryGetValue("info", out BEncodeDictionary info))
                {
                    info.Remove("private");
                }
            }
            else
            {
                if (!base.Root.TryGetValue("info", out BEncodeDictionary info))
                {
                    info = base.Root.Add("info", new BEncodeDictionary(this));
                }
                info.AddOrUpdate("private", new BEncodeInteger(flag.Value ? 1 : 0, this));
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
            if (base.Root.TryGetValue("info", out BEncodeDictionary info))
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
                if (base.Root.TryGetValue("info", out BEncodeDictionary info))
                {
                    info.Remove("piece length");
                }
            }
            else
            {
                if (!base.Root.TryGetValue("info", out BEncodeDictionary info))
                {
                    info = base.Root.Add("info", new BEncodeDictionary(this));
                }
                info.AddOrUpdate("piece length", new BEncodeInteger(length.Value, this));
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
            if (base.Root.TryGetValue("info", out BEncodeDictionary info))
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
            if (base.Root.TryGetValue("hash", out BEncodeString hash))
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
            if (base.Root.TryGetValue("info", out BEncodeDictionary info))
            {
                byte[] checksum = ComputeHash(info);
                string hash = BitConverter.ToString(checksum, 0, checksum.Length).Replace("-", null).ToLowerInvariant();
                base.Root.AddOrUpdate("hash", new BEncodeString(hash, this));
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
                base.Root.Remove("hash");
            }
            else
            {
                base.Root.AddOrUpdate("hash", new BEncodeString(hashHex, this));
            }
        }

        #endregion Hash

        #region GetFiles

        /// <summary>
        /// 获取所包含文件的信息。转换元素的值时默认使用 <see cref="BEncodeDocument.Encoding"/> 编码。
        /// </summary>
        /// <returns></returns>
        public IList<TorrentFileEntry> GetFiles()
        {
            return GetFiles(string.Empty, 0, long.MaxValue);
        }

        /// <summary>
        /// 获取所包含文件的信息。转换元素的值时默认使用 <see cref="BEncodeDocument.Encoding"/> 编码。
        /// </summary>
        /// <param name="searchPattern">文件名称检索条件（包括路径，路径使用 "/" 分隔符）。支持 * ? 通配符，不可使用正则表达式。</param>
        /// <param name="minSize">匹配最小文件大小。</param>
        /// <param name="maxSize">匹配最大文件大小。</param>
        /// <returns></returns>
        public IList<TorrentFileEntry> GetFiles(string searchPattern, long minSize, long maxSize)
        {
            return GetFiles(searchPattern, minSize, maxSize, base.Encoding);
        }

        /// <summary>
        /// 获取所包含文件的信息。
        /// </summary>
        /// <param name="searchPattern">文件名称检索条件（包括路径，路径使用 "/" 分隔符）。支持 * ? 通配符，不可使用正则表达式。</param>
        /// <param name="minSize">匹配最小文件大小。</param>
        /// <param name="maxSize">匹配最大文件大小。</param>
        /// <param name="valueEncoding">用于转换元素的值的字符编码。不使用所属的 <see cref="BEncodeDocument"/> 实例的默认字符编码。</param>
        /// <returns></returns>
        /// <exception cref="Exception"/>
        public IList<TorrentFileEntry> GetFiles(string searchPattern, long minSize, long maxSize, Encoding valueEncoding)
        {
            if (!string.IsNullOrWhiteSpace(searchPattern))
            {
                searchPattern = ConvertPattern(searchPattern);
            }
            List<TorrentFileEntry> result = new List<TorrentFileEntry>();
            if (base.Root.TryGetValue("info", out BEncodeDictionary info))
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
                            result.Add(new TorrentFileEntry(file, index, _multiple, this));
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
                        result.Add(new TorrentFileEntry(info, 0, _multiple, this));
                    }
                }
            }
            return result;
        }

        #endregion GetFiles

        #region SetFile

        /// <summary>
        /// 设置单文件。添加文件后会重置 "length", "pieces" 等元素值。设置 <see langword="null"/> 移除所有文件相关元素，无论之前存在的是单文件格式还是多文件格式。
        /// 转换元素的值时默认使用 <see cref="BEncodeDocument.Encoding"/> 编码。
        /// </summary>
        /// <param name="file">本地文件。</param>
        /// <param name="pieceLength">设置分块大小。此设置与 "pieces" 元素值相关，更改后必须重新添加文件以生成新的 "pieces" 元素。</param>
        /// <exception cref="Exception"/>
        public void SetFile(FileInfo file, int pieceLength)
        {
            SetFile(file, pieceLength, base.Encoding);
        }

        /// <summary>
        /// 设置单文件。添加文件后会重置 "length", "pieces" 等元素值。设置 <see langword="null"/> 移除所有文件相关元素，无论之前存在的是单文件格式还是多文件格式。
        /// </summary>
        /// <param name="file">本地文件。</param>
        /// <param name="pieceLength">设置分块大小。此设置与 "pieces" 元素值相关，更改后必须重新添加文件以生成新的 "pieces" 元素。</param>
        /// <param name="valueEncoding">用于转换元素的值的字符编码。不使用所属的 <see cref="BEncodeDocument"/> 实例的默认字符编码。</param>
        /// <exception cref="Exception"/>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Security", "CA5350:不要使用弱加密算法", Justification = "<挂起>")]
        public void SetFile(FileInfo file, int pieceLength, Encoding valueEncoding)
        {
            if (file == null || !file.Exists)
            {
                if (base.Root.TryGetValue("info", out BEncodeDictionary info))
                {
                    info.Remove("length");
                    info.Remove("pieces");
                    info.Remove("files");
                }
                _multiple = false;
            }
            else
            {
                _multiple = false;
                if (!base.Root.TryGetValue("info", out BEncodeDictionary info))
                {
                    info = base.Root.Add("info", new BEncodeDictionary(this));
                }
                info.Remove("files");
                info.AddOrUpdate("name", new BEncodeString(file.Name, valueEncoding, this));
                info.AddOrUpdate("length", new BEncodeInteger(file.Length, this));
                info.AddOrUpdate("piece length", new BEncodeInteger(pieceLength, this));
                using (SHA1 sha1 = SHA1.Create())
                {
                    List<byte> pieces = new List<byte>();
                    byte[] buffer = new byte[pieceLength];
                    int index = 0;
                    ComputeHash(sha1, file, buffer, ref index, pieces);
                    if (index > 0)
                    {
                        pieces.AddRange(sha1.ComputeHash(buffer, 0, index));
                    }
                    info.AddOrUpdate("pieces", new BEncodeString(pieces.ToArray(), this));
                }
            }
        }

        /// <summary>
        /// 设置复数文件。添加文件后会重置 "length", "pieces" 等元素值。设置 <see langword="null"/> 移除所有文件相关元素，无论之前存在的是单文件格式还是多文件格式。
        /// 转换元素的值时默认使用 <see cref="BEncodeDocument.Encoding"/> 编码。
        /// </summary>
        /// <param name="file">本地文件。</param>
        /// <param name="pieceLength">设置分块大小。此设置与 "pieces" 元素值相关，更改后必须重新添加文件以生成新的 "pieces" 元素。</param>
        /// <param name="completed">任务结束后执行。</param>
        /// <param name="userState">在回调中传递参数。</param>
        /// <exception cref="Exception"/>
        public void SetFileAsync(FileInfo file, int pieceLength, TorrentSetFileCompletedCallback completed, object userState)
        {
            SetFileAsync(file, pieceLength, base.Encoding, completed, userState);
        }

        /// <summary>
        /// 设置复数文件。添加文件后会重置 "length", "pieces" 等元素值。设置 <see langword="null"/> 移除所有文件相关元素，无论之前存在的是单文件格式还是多文件格式。
        /// </summary>
        /// <param name="file">本地文件。</param>
        /// <param name="pieceLength">设置分块大小。此设置与 "pieces" 元素值相关，更改后必须重新添加文件以生成新的 "pieces" 元素。</param>
        /// <param name="valueEncoding">用于转换元素的值的字符编码。不使用所属的 <see cref="BEncodeDocument"/> 实例的默认字符编码。</param>
        /// <param name="completed">任务结束后执行。</param>
        /// <param name="userState">在回调中传递参数。</param>
        /// <exception cref="Exception"/>
        public void SetFileAsync(FileInfo file, int pieceLength, Encoding valueEncoding, TorrentSetFileCompletedCallback completed, object userState)
        {
            Task.Factory.StartNew(() =>
            {
                SetFile(file, pieceLength, valueEncoding);
                completed?.Invoke(new TorrentSetFileCompletedEventArgs(1, userState, false));
            }, CancellationToken.None, TaskCreationOptions.PreferFairness, TaskScheduler.Default);
        }

        #endregion SetFile

        #region SetFiles

        /// <summary>
        /// 设置复数文件。添加文件后会重置 "length", "pieces" 等元素值。设置 <see langword="null"/> 移除所有文件相关元素，无论之前存在的是单文件格式还是多文件格式。
        /// 转换元素的值时默认使用 <see cref="BEncodeDocument.Encoding"/> 编码。
        /// </summary>
        /// <param name="folder">选择本地文件夹，添加其中所有文件。</param>
        /// <param name="pieceLength">设置分块大小。此设置与 "pieces" 元素值相关，更改后必须重新添加文件以生成新的 "pieces" 元素。</param>
        /// <exception cref="Exception"/>
        public void SetFiles(DirectoryInfo folder, int pieceLength)
        {
            SetFiles(folder, pieceLength, base.Encoding);
        }

        /// <summary>
        /// 设置复数文件。添加文件后会重置 "length", "pieces" 等元素值。设置 <see langword="null"/> 移除所有文件相关元素，无论之前存在的是单文件格式还是多文件格式。
        /// </summary>
        /// <param name="folder">选择本地文件夹，添加其中所有文件。</param>
        /// <param name="pieceLength">设置分块大小。此设置与 "pieces" 元素值相关，更改后必须重新添加文件以生成新的 "pieces" 元素。</param>
        /// <param name="valueEncoding">用于转换元素的值的字符编码。不使用所属的 <see cref="BEncodeDocument"/> 实例的默认字符编码。</param>
        /// <exception cref="Exception"/>
        public void SetFiles(DirectoryInfo folder, int pieceLength, Encoding valueEncoding)
        {
            SetFiles(folder, pieceLength, valueEncoding, null, null);
        }

        /// <summary>
        /// 设置复数文件。添加文件后会重置 "length", "pieces" 等元素值。设置 <see langword="null"/> 移除所有文件相关元素，无论之前存在的是单文件格式还是多文件格式。
        /// 转换元素的值时默认使用 <see cref="BEncodeDocument.Encoding"/> 编码。
        /// </summary>
        /// <param name="folder">选择本地文件夹，添加其中所有文件。</param>
        /// <param name="pieceLength">设置分块大小。此设置与 "pieces" 元素值相关，更改后必须重新添加文件以生成新的 "pieces" 元素。</param>
        /// <param name="fileEntryAdded">添加一个文件后执行。</param>
        /// <param name="userState">在回调中传递参数。</param>
        /// <exception cref="Exception"/>
        public void SetFiles(DirectoryInfo folder, int pieceLength, TorrentFileEntryAddedCallback fileEntryAdded, object userState)
        {
            SetFiles(folder, pieceLength, base.Encoding, fileEntryAdded, userState);
        }

        /// <summary>
        /// 设置复数文件。添加文件后会重置 "length", "pieces" 等元素值。设置 <see langword="null"/> 移除所有文件相关元素，无论之前存在的是单文件格式还是多文件格式。
        /// </summary>
        /// <param name="folder">选择本地文件夹，添加其中所有文件。</param>
        /// <param name="pieceLength">设置分块大小。此设置与 "pieces" 元素值相关，更改后必须重新添加文件以生成新的 "pieces" 元素。</param>
        /// <param name="valueEncoding">用于转换元素的值的字符编码。不使用所属的 <see cref="BEncodeDocument"/> 实例的默认字符编码。</param>
        /// <param name="fileEntryAdded">添加一个文件后执行。</param>
        /// <param name="userState">在回调中传递参数。</param>
        /// <returns></returns>
        /// <exception cref="Exception"/>
        public int SetFiles(DirectoryInfo folder, int pieceLength, Encoding valueEncoding, TorrentFileEntryAddedCallback fileEntryAdded, object userState)
        {
            return SetFiles(folder, pieceLength, valueEncoding, fileEntryAdded, userState, out _);
        }

        /// <summary>
        /// 设置复数文件。添加文件后会重置 "length", "pieces" 等元素值。设置 <see langword="null"/> 移除所有文件相关元素，无论之前存在的是单文件格式还是多文件格式。
        /// 转换元素的值时默认使用 <see cref="BEncodeDocument.Encoding"/> 编码。
        /// </summary>
        /// <param name="folder">选择本地文件夹，添加其中所有文件。</param>
        /// <param name="pieceLength">设置分块大小。此设置与 "pieces" 元素值相关，更改后必须重新添加文件以生成新的 "pieces" 元素。</param>
        /// <param name="fileEntryAdded">添加一个文件后执行。</param>
        /// <param name="completed">任务结束后执行。</param>
        /// <param name="userState">在回调中传递参数。</param>
        /// <exception cref="Exception"/>
        public void SetFilesAsync(DirectoryInfo folder, int pieceLength, TorrentFileEntryAddedCallback fileEntryAdded, TorrentSetFileCompletedCallback completed, object userState)
        {
            SetFilesAsync(folder, pieceLength, base.Encoding, fileEntryAdded, completed, userState);
        }

        /// <summary>
        /// 设置复数文件。添加文件后会重置 "length", "pieces" 等元素值。设置 <see langword="null"/> 移除所有文件相关元素，无论之前存在的是单文件格式还是多文件格式。
        /// </summary>
        /// <param name="folder">选择本地文件夹，添加其中所有文件。</param>
        /// <param name="pieceLength">设置分块大小。此设置与 "pieces" 元素值相关，更改后必须重新添加文件以生成新的 "pieces" 元素。</param>
        /// <param name="valueEncoding">用于转换元素的值的字符编码。不使用所属的 <see cref="BEncodeDocument"/> 实例的默认字符编码。</param>
        /// <param name="fileEntryAdded">添加一个文件后执行。</param>
        /// <param name="completed">任务结束后执行。</param>
        /// <param name="userState">在回调中传递参数。</param>
        /// <exception cref="Exception"/>
        public void SetFilesAsync(DirectoryInfo folder, int pieceLength, Encoding valueEncoding, TorrentFileEntryAddedCallback fileEntryAdded, TorrentSetFileCompletedCallback completed, object userState)
        {
            Task.Factory.StartNew(() =>
            {
                int total = SetFiles(folder, pieceLength, valueEncoding, fileEntryAdded, userState, out bool isCancelled);
                completed?.Invoke(new TorrentSetFileCompletedEventArgs(total, userState, isCancelled));
            }, CancellationToken.None, TaskCreationOptions.PreferFairness, TaskScheduler.Default);
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Security", "CA5350:不要使用弱加密算法", Justification = "<挂起>")]
        private int SetFiles(DirectoryInfo folder, int pieceLength, Encoding valueEncoding, TorrentFileEntryAddedCallback fileEntryAdded, object userState, out bool isCancelled)
        {
            if (folder == null || !folder.Exists)
            {
                if (base.Root.TryGetValue("info", out BEncodeDictionary info))
                {
                    info.Remove("length");
                    info.Remove("pieces");
                    info.Remove("files");
                }
                _multiple = false;
                isCancelled = false;
                return 0;
            }
            else
            {
                var files = new List<FileInfo>();
                SearchFiles(folder, files);
                if (files.Count == 0)
                {
                    if (base.Root.TryGetValue("info", out BEncodeDictionary info))
                    {
                        info.Remove("length");
                        info.Remove("pieces");
                        info.Remove("files");
                    }
                    _multiple = false;
                    isCancelled = false;
                    return 0;
                }
                else
                {
                    _multiple = true;
                    if (!base.Root.TryGetValue("info", out BEncodeDictionary info))
                    {
                        info = base.Root.Add("info", new BEncodeDictionary(this));
                    }
                    info.Remove("length");
                    info.Remove("pieces");
                    info.AddOrUpdate("name", new BEncodeString(folder.Name, valueEncoding, this));
                    info.GetOrAdd("piece length", new BEncodeInteger(pieceLength, this));
                    var fileEntries = info.AddOrUpdate("files", new BEncodeList(this));
                    using (SHA1 sha1 = SHA1.Create())
                    {
                        List<byte> pieces = new List<byte>();
                        byte[] buffer = new byte[pieceLength];
                        int hashIndex = 0;
                        for (int i = 0; i < files.Count; i++)
                        {
                            FileInfo file = files[i];
                            BEncodeDictionary entry = fileEntries.Add(new BEncodeDictionary(this));
                            BEncodeList path = entry.Add("path", new BEncodeList(this));
                            string en = file.FullName.Remove(0, folder.FullName.Length).Replace('\\', '/').Trim('/');
                            string[] paths = en.Split('/');
                            foreach (string p in paths)
                            {
                                path.Add(new BEncodeString(p, valueEncoding, this));
                            }
                            entry.Add("length", new BEncodeInteger(file.Length, this));
                            ComputeHash(sha1, file, buffer, ref hashIndex, pieces);
                            if (fileEntryAdded != null)
                            {
                                var e = new TorrentFileEntryAddedEventArgs(new TorrentFileEntry(entry, i, _multiple, this), i, files.Count, userState, false);
                                fileEntryAdded.Invoke(e);
                                if (e.Cancel)
                                {
                                    info.Remove("length");
                                    info.Remove("pieces");
                                    info.Remove("files");
                                    _multiple = false;
                                    isCancelled = true;
                                    return 0;
                                }
                            }
                        }
                        if (hashIndex > 0)
                        {
                            pieces.AddRange(sha1.ComputeHash(buffer, 0, hashIndex));
                        }
                        info.AddOrUpdate("pieces", new BEncodeString(pieces.ToArray(), this));
                    }
                    isCancelled = false;
                    return files.Count;
                }
            }
        }

        #endregion SetFiles

        #region Magnet

        /// <summary>
        /// 实时计算 BTIH 特征码标识的磁力链接。转换元素的值时默认使用 <see cref="BEncodeDocument.Encoding"/> 编码。
        /// </summary>
        /// <returns></returns>
        public string GetMagnet()
        {
            return GetMagnet(false, false, false, false);
        }

        /// <summary>
        /// 实时计算 BTIH 特征码标识的磁力链接。转换元素的值时默认使用 <see cref="BEncodeDocument.Encoding"/> 编码。
        /// </summary>
        /// <param name="dn">是否携带显示名称。</param>
        /// <param name="xl">是否携带文件长度数值。</param>
        /// <param name="tr">是否携带主要 Tracker 服务器。</param>
        /// <param name="trs">是否携带备用 Tracker 服务器列表。</param>
        /// <returns></returns>
        /// <exception cref="Exception"/>
        public string GetMagnet(bool dn, bool xl, bool tr, bool trs)
        {
            return GetMagnet(dn, xl, tr, trs, base.Encoding);
        }

        /// <summary>
        /// 实时计算 BTIH 特征码标识的磁力链接。
        /// </summary>
        /// <param name="dn">是否携带显示名称。</param>
        /// <param name="xl">是否携带文件长度数值。</param>
        /// <param name="tr">是否携带主要 Tracker 服务器。</param>
        /// <param name="trs">是否携带备用 Tracker 服务器列表。</param>
        /// <param name="valueEncoding">用于转换元素的值的字符编码。不使用所属的 <see cref="BEncodeDocument"/> 实例的默认字符编码。</param>
        /// <returns></returns>
        /// <exception cref="Exception"/>
        public string GetMagnet(bool dn, bool xl, bool tr, bool trs, Encoding valueEncoding)
        {
            if (base.Root.TryGetValue("info", out BEncodeDictionary info))
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
                if (tr && base.Root.TryGetValue("announce", out BEncodeString announce))
                {
                    result.Append("&tr=");
                    result.Append(announce.GetStringValue(valueEncoding));
                }
                if (trs && base.Root.TryGetValue("announce-list", out BEncodeList announceList))
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

        #region Save

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
            SetCreatedBy(createdBy);
            SetCreationDate(creationDate);
            SetHash(hashHex);
            base.Save(stream);
        }

        #endregion Save

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

        private static string ConvertPattern(string pattern)
        {
            return pattern.Trim().Replace("\\", "/")
                .Replace("(", "\\(").Replace(")", "\\)").Replace("[", "\\[").Replace("{", "\\{")
                .Replace("^", "\\^").Replace("+", "\\+").Replace("|", "\\|").Replace("$", "\\$")
                .Replace(".", "\\.").Replace("*", ".*").Replace("?", ".?");
        }

        private static void SearchFiles(DirectoryInfo folder, List<FileInfo> files)
        {
            var folders = folder.GetDirectories();
            if (folders.Length > 0)
            {
                foreach (DirectoryInfo f in folders)
                {
                    SearchFiles(f, files);
                }
            }
            var searched = folder.GetFiles();
            if (searched.Length > 0)
            {
                foreach (FileInfo f in searched)
                {
                    files.Add(f);
                }
            }
        }
    }
}