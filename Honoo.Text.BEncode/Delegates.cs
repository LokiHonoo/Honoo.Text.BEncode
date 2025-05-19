namespace Honoo.Text.BEncode
{
    /// <summary>
    /// 种子添加一个文件后执行。
    /// </summary>
    /// <param name="sender">引发此事件的 Torrent 文档。</param>
    /// <param name="e">种子添加一个文件后的事件参数。</param>
    public delegate void TorrentFileEntryAddedCallback(TorrentDocument sender, TorrentFileEntryAddedEventArgs e);

    /// <summary>
    /// 种子添加文件任务结束后执行。
    /// </summary>
    /// <param name="sender">引发此事件的 Torrent 文档。</param>
    /// <param name="e">种子添加文件任务结束事件参数。</param>
    public delegate void TorrentSetFileCompletedCallback(TorrentDocument sender, TorrentSetFileCompletedEventArgs e);
}