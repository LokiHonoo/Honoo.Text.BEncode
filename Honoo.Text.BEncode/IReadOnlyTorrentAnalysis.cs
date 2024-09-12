using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Text;

namespace Honoo.Text.BEncode
{
    /// <summary>
    /// TorrentAnalysis 类型只读接口。
    /// </summary>
    public interface IReadOnlyTorrentAnalysis
    {
        /// <summary>
        /// 获取一个值，指示此 Torrent 文件是否是多文件格式。
        /// </summary>
        bool Multiple { get; }

        /// <summary>
        /// 获取主要 Tracker 服务器。如果节点不存在，返回 <see langword="null"/>。转换元素的键时默认使用 <see cref="Encoding.UTF8"/> 编码。
        /// </summary>
        /// <returns></returns>
        string GetAnnounce();

        /// <summary>
        /// 获取主要 Tracker 服务器。如果节点不存在，返回 <see langword="null"/>。
        /// </summary>
        /// <param name="valueEncoding">用于转换元素的值的字符编码。</param>
        /// <returns></returns>
        /// <exception cref="Exception"/>
        string GetAnnounce(Encoding valueEncoding);

        /// <summary>
        /// 获取备用 Tracker 服务器列表。如果节点不存在，返回 <see langword="null"/>。转换元素的值时默认使用 <see cref="Encoding.UTF8"/> 编码。
        /// </summary>
        /// <returns></returns>
        string[][] GetAnnounceList();

        /// <summary>
        /// 获取备用 Tracker 服务器列表。如果节点不存在，返回 <see langword="null"/>。
        /// </summary>
        /// <param name="valueEncoding">用于转换元素的值的字符编码。</param>
        /// <returns></returns>
        /// <exception cref="Exception"/>
        string[][] GetAnnounceList(Encoding valueEncoding);

        /// <summary>
        /// 获取注释。如果节点不存在，返回 <see langword="null"/>。转换元素的值时默认使用 <see cref="Encoding.UTF8"/> 编码。
        /// </summary>
        /// <returns></returns>
        string GetComment();

        /// <summary>
        /// 获取注释。如果节点不存在，返回 <see langword="null"/>。
        /// </summary>
        /// <param name="valueEncoding">用于转换元素的值的字符编码。</param>
        /// <returns></returns>
        /// <exception cref="Exception"/>
        string GetComment(Encoding valueEncoding);

        /// <summary>
        /// 获取创建者名称。如果节点不存在，返回 <see langword="null"/>。转换元素的值时默认使用 <see cref="Encoding.UTF8"/> 编码。
        /// </summary>
        /// <returns></returns>
        string GetCreatedBy();

        /// <summary>
        /// 获取创建者名称。如果节点不存在，返回 <see langword="null"/>。
        /// </summary>
        /// <param name="valueEncoding">用于转换元素的值的字符编码。</param>
        /// <returns></returns>
        /// <exception cref="Exception"/>
        string GetCreatedBy(Encoding valueEncoding);

        /// <summary>
        /// 获取创建时间。如果节点不存在，返回 <see langword="null"/>。
        /// </summary>
        /// <returns></returns>
        DateTime? GetCreationDate();

        /// <summary>
        /// 获取编码标识。如果节点不存在，返回 <see langword="null"/>。转换元素的值时默认使用 <see cref="Encoding.UTF8"/> 编码。
        /// </summary>
        /// <returns></returns>
        string GetEncoding();

        /// <summary>
        /// 获取编码标识。如果节点不存在，返回 <see langword="null"/>。
        /// </summary>
        /// <param name="valueEncoding">用于转换元素的值的字符编码。</param>
        /// <returns></returns>
        /// <exception cref="Exception"/>
        string GetEncoding(Encoding valueEncoding);

        /// <summary>
        /// 获取所包含文件的信息。转换元素的值时默认使用 <see cref="Encoding.UTF8"/> 编码。
        /// </summary>
        /// <returns></returns>
        ICollection<TorrentFileEntry> GetFiles();

        /// <summary>
        /// 获取所包含文件的信息。转换元素的值时默认使用 <see cref="Encoding.UTF8"/> 编码。
        /// </summary>
        /// <param name="searchPattern">文件名称检索条件（包括路径，路径使用 '/' 分隔符）。支持*.*，不可使用正则表达式。</param>
        /// <param name="minSize">匹配最小文件大小。</param>
        /// <param name="maxSize">匹配最大文件大小。</param>
        /// <returns></returns>
        ICollection<TorrentFileEntry> GetFiles(string searchPattern, long minSize, long maxSize);

        /// <summary>
        /// 获取所包含文件的信息。
        /// </summary>
        /// <param name="searchPattern">文件名称检索条件（包括路径，路径使用 '/' 分隔符）。支持*.*，不可使用正则表达式。</param>
        /// <param name="minSize">匹配最小文件大小。</param>
        /// <param name="maxSize">匹配最大文件大小。</param>
        /// <param name="customKeys">尝试读取指定键的元素。</param>
        /// <returns></returns>
        ICollection<TorrentFileEntry> GetFiles(string searchPattern, long minSize, long maxSize, HashSet<BEncodeString> customKeys);

        /// <summary>
        /// 获取所包含文件的信息。
        /// </summary>
        /// <param name="searchPattern">文件名称检索条件（包括路径，路径使用 '/' 分隔符）。支持*.*，不可使用正则表达式。</param>
        /// <param name="minSize">匹配最小文件大小。</param>
        /// <param name="maxSize">匹配最大文件大小。</param>
        /// <param name="customKeys">尝试读取指定键的元素。</param>
        /// <param name="valueEncoding">用于转换元素的值的字符编码。</param>
        /// <returns></returns>
        /// <exception cref="Exception"/>
        ICollection<TorrentFileEntry> GetFiles(string searchPattern, long minSize, long maxSize, HashSet<BEncodeString> customKeys, Encoding valueEncoding);

        /// <summary>
        /// 获取特征码。如果节点不存在，返回 <see langword="null"/>。
        /// </summary>
        /// <returns></returns>
        string GetHash();

        /// <summary>
        /// 实时计算 BTIH 特征码标识的磁力链接。转换元素的值时默认使用 <see cref="Encoding.UTF8"/> 编码。
        /// </summary>
        /// <returns></returns>
        string GetMagnet();

        /// <summary>
        /// 实时计算 BTIH 特征码标识的磁力链接。转换元素的值时默认使用 <see cref="Encoding.UTF8"/> 编码。
        /// </summary>
        /// <param name="dn">是否携带显示名称。</param>
        /// <param name="xl">是否携带文件长度数值。</param>
        /// <param name="tr">是否携带主要 Tracker 服务器。</param>
        /// <param name="trs">是否携带备用 Tracker 服务器列表。</param>
        /// <returns></returns>
        /// <exception cref="Exception"/>
        string GetMagnet(bool dn, bool xl, bool tr, bool trs);

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
        string GetMagnet(bool dn, bool xl, bool tr, bool trs, Encoding valueEncoding);

        /// <summary>
        /// 获取推荐文件名。如果节点不存在，返回 <see langword="null"/>。转换元素的值时默认使用 <see cref="Encoding.UTF8"/> 编码。
        /// </summary>
        /// <returns></returns>
        string GetName();

        /// <summary>
        /// 获取推荐文件名。如果节点不存在，返回 <see langword="null"/>。
        /// </summary>
        /// <param name="valueEncoding">用于转换元素的值的字符编码。</param>
        /// <returns></returns>
        /// <exception cref="Exception"/>
        string GetName(Encoding valueEncoding);

        /// <summary>
        /// 获取 DHT 初始节点。如果节点不存在，返回 <see langword="null"/>。
        /// </summary>
        /// <returns></returns>
        IPEndPoint[] GetNodes();

        /// <summary>
        /// 获取分块大小。如果节点不存在，返回 <see langword="null"/>。
        /// </summary>
        /// <returns></returns>
        long? GetPieceLength();

        /// <summary>
        /// 获取分块的集成特征码。如果节点不存在，返回 <see langword="null"/>。
        /// </summary>
        /// <returns></returns>
        byte[] GetPieces();

        /// <summary>
        /// 获取私有标记。如果节点不存在，返回 <see langword="null"/>。
        /// </summary>
        /// <returns></returns>
        bool? GetPrivate();

        /// <summary>
        /// 获取发布者名称。如果节点不存在，返回 <see langword="null"/>。转换元素的值时默认使用 <see cref="Encoding.UTF8"/> 编码。
        /// </summary>
        /// <returns></returns>
        string GetPublisher();

        /// <summary>
        /// 获取发布者名称。如果节点不存在，返回 <see langword="null"/>。
        /// </summary>
        /// <param name="valueEncoding">用于转换元素的值的字符编码。</param>
        /// <returns></returns>
        /// <exception cref="Exception"/>
        string GetPublisher(Encoding valueEncoding);

        /// <summary>
        /// 获取发布者 Url。如果节点不存在，返回 <see langword="null"/>。转换元素的值时默认使用 <see cref="Encoding.UTF8"/> 编码。
        /// </summary>
        /// <returns></returns>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Design", "CA1055:类 URI 返回值不应是字符串", Justification = "<挂起>")]
        string GetPublisherUrl();

        /// <summary>
        /// 获取发布者 Url。如果节点不存在，返回 <see langword="null"/>。
        /// </summary>
        /// <param name="valueEncoding">用于转换元素的值的字符编码。</param>
        /// <returns></returns>
        /// <exception cref="Exception"/>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Design", "CA1055:类 URI 返回值不应是字符串", Justification = "<挂起>")]
        string GetPublisherUrl(Encoding valueEncoding);

        /// <summary>
        /// 保存到指定的流。如果 <see cref="TorrentAnalysis"/> 不是只读的，添加或更新必要元素。
        /// </summary>
        /// <param name="stream">指定保存的目标流。</param>
        /// <exception cref="Exception"/>
        void Save(Stream stream);
    }
}