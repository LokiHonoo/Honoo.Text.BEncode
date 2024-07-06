namespace Honoo.Text.BEncode
{
    /// <summary>
    /// BT 种子文件的文件描述信息。
    /// </summary>
    public class TorrentFileInfo
    {
        private readonly long _length;
        private readonly byte[] _md5Sum;
        private readonly string[] _path;

        /// <summary>
        /// 获取文件的字节大小。
        /// </summary>
        public long Length => _length;

        /// <summary>
        /// 获取文件的 MD5。
        /// </summary>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Performance", "CA1819:属性不应返回数组", Justification = "<挂起>")]
        public byte[] Md5Sum => _md5Sum;

        /// <summary>
        /// 获取文件的路径。
        /// </summary>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Performance", "CA1819:属性不应返回数组", Justification = "<挂起>")]
        public string[] Path => _path;

        #region Construction

        internal TorrentFileInfo(string[] path, long length, byte[] md5Sum)
        {
            _path = path;
            _length = length;
            _md5Sum = md5Sum;
        }

        #endregion Construction
    }
}