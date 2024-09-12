namespace Honoo.Text.BEncode
{
    /// <summary>
    /// 添加一个文件成功后执行。
    /// </summary>
    /// <param name="fileEntry">添加的文件元素。</param>
    /// <param name="index">添加的文件元素在任务列表中从 0 开始的索引。</param>
    /// <param name="total">任务列表中的任务总数。</param>
    public delegate void TorrentFileEntryAddedCallback(BEncodeDictionary fileEntry, int index, int total);
}