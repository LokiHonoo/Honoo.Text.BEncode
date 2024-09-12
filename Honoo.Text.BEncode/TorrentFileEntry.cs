namespace Honoo.Text.BEncode
{
    /// <summary>
    /// BT 文件的 file 节点描述信息。
    /// </summary>
    public class TorrentFileEntry
    {
        private readonly BEncodeDictionary _customs;
        private readonly long _length;
        private readonly string[] _paths;

        /// <summary>
        /// 获取读取时设置的自定义元素。
        /// </summary>
        public BEncodeDictionary Customs => _customs;

        /// <summary>
        /// 获取文件的字节大小。
        /// </summary>
        public long Length => _length;

        /// <summary>
        /// 获取文件的路径。
        /// </summary>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Performance", "CA1819:属性不应返回数组", Justification = "<挂起>")]
        public string[] Paths => _paths;

        #region Construction

        internal TorrentFileEntry(string[] paths, long length, BEncodeDictionary customs)
        {
            _paths = paths;
            _length = length;
            _customs = customs;
        }

        #endregion Construction
    }
}