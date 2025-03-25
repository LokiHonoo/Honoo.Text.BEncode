using System;

namespace Honoo.Text.BEncode
{
    /// <summary>
    /// 种子添加一个文件后的事件参数。
    /// </summary>
    public sealed class TorrentFileEntryAddedEventArgs : EventArgs
    {
        #region Members

        private readonly TorrentFileEntry _fileEntry;
        private readonly int _index;
        private readonly int _total;
        private bool _cancel;
        private object _userState;

        /// <summary>
        /// 取消任务。
        /// </summary>
        public bool Cancel { get => _cancel; set => _cancel = value; }

        /// <summary>
        /// 添加的文件元素。
        /// </summary>
        public TorrentFileEntry FileEntry => _fileEntry;

        /// <summary>
        /// 添加的文件元素在文件列表中从 0 开始的索引。
        /// </summary>
        public int Index => _index;

        /// <summary>
        /// 文件列表中的文件总数。
        /// </summary>
        public int Total => _total;

        /// <summary>
        /// 传递的用户参数。
        /// </summary>
        public object UserState { get => _userState; set => _userState = value; }

        #endregion Members

        /// <summary>
        /// 创建 TorrentFileEntryAddedEventArgs 类的新实例。
        /// </summary>
        /// <param name="fileEntry">添加的文件元素。</param>
        /// <param name="index">添加的文件元素在文件列表中从 0 开始的索引。</param>
        /// <param name="total">文件列表中的文件总数。</param>
        /// <param name="userState">传递的用户参数。</param>
        /// <param name="cancel">取消任务。</param>
        internal TorrentFileEntryAddedEventArgs(TorrentFileEntry fileEntry, int index, int total, object userState, bool cancel)
        {
            _fileEntry = fileEntry;
            _index = index;
            _total = total;
            _userState = userState;
            _cancel = cancel;
        }
    }

    /// <summary>
    /// 种子添加文件任务结束事件参数。
    /// </summary>
    public sealed class TorrentSetFileCompletedEventArgs : EventArgs
    {
        #region Members

        private readonly bool _isCancelled;
        private readonly int _total;
        private readonly object _userState;

        /// <summary>
        /// 指示此任务是完成或取消。
        /// </summary>
        public bool IsCancelled => _isCancelled;

        /// <summary>
        /// 文件列表中的文件总数。
        /// </summary>
        public int Total => _total;

        /// <summary>
        /// 传递的用户参数。
        /// </summary>
        public object UserState => _userState;

        #endregion Members

        /// <summary>
        /// 创建 TorrentSetFileCompletedEventArgs 类的新实例。
        /// </summary>
        /// <param name="total">文件列表中的文件总数。</param>
        /// <param name="userState">传递的用户参数。</param>
        /// <param name="isCancelled">指示此任务是完成或取消。</param>
        internal TorrentSetFileCompletedEventArgs(int total, object userState, bool isCancelled)
        {
            _total = total;
            _userState = userState;
            _isCancelled = isCancelled;
        }
    }
}