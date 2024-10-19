using System;
using System.Collections.Generic;
using System.Net;
using System.Text;

namespace Honoo.Text.BEncode
{
    /// <summary>
    /// TorrentAnalysis 类型只读接口。
    /// </summary>
    public interface IReadOnlyTorrentAnalysis : IReadOnlyBEncodeDictionary
    {
        /// <summary>
        /// 获取一个值，指示此 Torrent 文件是否是多文件格式。
        /// </summary>
        bool Multiple { get; }

        #region Announce

        /// <summary>
        /// 获取主要 Tracker 服务器。如果元素不存在，返回 <see langword="null"/>。转换元素的键时默认使用 <see cref="Encoding.UTF8"/> 编码。
        /// </summary>
        /// <returns></returns>
        string GetAnnounce();

        /// <summary>
        /// 获取主要 Tracker 服务器。如果元素不存在，返回 <see langword="null"/>。
        /// </summary>
        /// <param name="valueEncoding">用于转换元素的值的字符编码。</param>
        /// <returns></returns>
        /// <exception cref="Exception"/>
        string GetAnnounce(Encoding valueEncoding);

        #endregion Announce

        #region Announce-List

        /// <summary>
        /// 获取备用 Tracker 服务器列表。如果元素不存在，返回 <see langword="null"/>。转换元素的值时默认使用 <see cref="Encoding.UTF8"/> 编码。
        /// </summary>
        /// <returns></returns>
        string[][] GetAnnounceList();

        /// <summary>
        /// 获取备用 Tracker 服务器列表。如果元素不存在，返回 <see langword="null"/>。
        /// </summary>
        /// <param name="valueEncoding">用于转换元素的值的字符编码。</param>
        /// <returns></returns>
        /// <exception cref="Exception"/>
        string[][] GetAnnounceList(Encoding valueEncoding);

        #endregion Announce-List

        #region CreatedBy

        /// <summary>
        /// 获取创建者名称。如果元素不存在，返回 <see langword="null"/>。转换元素的值时默认使用 <see cref="Encoding.UTF8"/> 编码。
        /// </summary>
        /// <returns></returns>
        string GetCreatedBy();

        /// <summary>
        /// 获取创建者名称。如果元素不存在，返回 <see langword="null"/>。
        /// </summary>
        /// <param name="valueEncoding">用于转换元素的值的字符编码。</param>
        /// <returns></returns>
        /// <exception cref="Exception"/>
        string GetCreatedBy(Encoding valueEncoding);

        #endregion CreatedBy

        #region CreationDate

        /// <summary>
        /// 获取创建时间。如果元素不存在，返回 <see langword="null"/>。
        /// </summary>
        /// <returns></returns>
        DateTime? GetCreationDate();

        #endregion CreationDate

        #region Encoding

        /// <summary>
        /// 获取编码标识。如果元素不存在，返回 <see langword="null"/>。转换元素的值时默认使用 <see cref="Encoding.UTF8"/> 编码。
        /// </summary>
        /// <returns></returns>
        string GetEncoding();

        /// <summary>
        /// 获取编码标识。如果元素不存在，返回 <see langword="null"/>。
        /// </summary>
        /// <param name="valueEncoding">用于转换元素的值的字符编码。</param>
        /// <returns></returns>
        /// <exception cref="Exception"/>
        string GetEncoding(Encoding valueEncoding);

        #endregion Encoding

        #region Comment

        /// <summary>
        /// 获取注释。如果元素不存在，返回 <see langword="null"/>。转换元素的值时默认使用 <see cref="Encoding.UTF8"/> 编码。
        /// </summary>
        /// <returns></returns>
        string GetComment();

        /// <summary>
        /// 获取注释。如果元素不存在，返回 <see langword="null"/>。
        /// </summary>
        /// <param name="valueEncoding">用于转换元素的值的字符编码。</param>
        /// <returns></returns>
        /// <exception cref="Exception"/>
        string GetComment(Encoding valueEncoding);

        #endregion Comment

        #region Nodes

        /// <summary>
        /// 获取 DHT 初始节点。如果元素不存在，返回 <see langword="null"/>。
        /// </summary>
        /// <returns></returns>
        IPEndPoint[] GetNodes();

        #endregion Nodes

        #region Name

        /// <summary>
        /// 获取推荐文件名。如果元素不存在，返回 <see langword="null"/>。转换元素的值时默认使用 <see cref="Encoding.UTF8"/> 编码。
        /// </summary>
        /// <returns></returns>
        string GetName();

        /// <summary>
        /// 获取推荐文件名。如果元素不存在，返回 <see langword="null"/>。
        /// </summary>
        /// <param name="valueEncoding">用于转换元素的值的字符编码。</param>
        /// <returns></returns>
        /// <exception cref="Exception"/>
        string GetName(Encoding valueEncoding);

        #endregion Name

        #region Private

        /// <summary>
        /// 获取私有标记。如果元素不存在，返回 <see langword="null"/>。
        /// </summary>
        /// <returns></returns>
        bool? GetPrivate();

        #endregion Private

        #region PieceLength

        /// <summary>
        /// 获取分块大小。如果元素不存在，返回 <see langword="null"/>。
        /// </summary>
        /// <returns></returns>
        long? GetPieceLength();

        #endregion PieceLength

        #region Pieces

        /// <summary>
        /// 获取分块的集成特征码。如果元素不存在，返回 <see langword="null"/>。
        /// </summary>
        /// <returns></returns>
        byte[] GetPieces();

        #endregion Pieces

        #region Hash

        /// <summary>
        /// 获取特征码。如果元素不存在，返回 <see langword="null"/>。
        /// </summary>
        /// <returns></returns>
        string GetHash();

        #endregion Hash

        #region Files

        /// <summary>
        /// 获取所包含文件的信息。转换元素的值时默认使用 <see cref="Encoding.UTF8"/> 编码。
        /// </summary>
        /// <returns></returns>
        IList<TorrentFileEntry> GetFiles();

        /// <summary>
        /// 获取所包含文件的信息。转换元素的值时默认使用 <see cref="Encoding.UTF8"/> 编码。
        /// </summary>
        /// <param name="searchPattern">文件名称检索条件（包括路径，路径使用 "/" 分隔符）。支持 *.*，不可使用正则表达式。</param>
        /// <param name="minSize">匹配最小文件大小。</param>
        /// <param name="maxSize">匹配最大文件大小。</param>
        /// <returns></returns>
        IList<TorrentFileEntry> GetFiles(string searchPattern, long minSize, long maxSize);

        /// <summary>
        /// 获取所包含文件的信息。
        /// </summary>
        /// <param name="searchPattern">文件名称检索条件（包括路径，路径使用 "/" 分隔符）。支持 *.*，不可使用正则表达式。</param>
        /// <param name="minSize">匹配最小文件大小。</param>
        /// <param name="maxSize">匹配最大文件大小。</param>
        /// <param name="valueEncoding">用于转换元素的值的字符编码。</param>
        /// <returns></returns>
        /// <exception cref="Exception"/>
        IList<TorrentFileEntry> GetFiles(string searchPattern, long minSize, long maxSize, Encoding valueEncoding);

        #endregion Files

        #region Magnet

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

        #endregion Magnet
    }
}