using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Honoo.Text.BEncode
{
    /// <summary>
    /// 单文件格式的 "info" 元素，或多文件格式的 "file" 元素的映射。
    /// </summary>
    public class TorrentFileEntry
    {
        #region Members

        private readonly TorrentAnalysis _document;
        private readonly BEncodeDictionary _element;
        private readonly int _index;
        private readonly bool _multiple;

        /// <summary>
        /// 获取此文件的 "file" 元素映射。如果源实例是单文件种子，则获取 "info" 元素。
        /// 此元素是文档元素的映射，任何修改都会导致 "info" 元素内容改变。修改结果会反应在从中获取元素的源实例中。
        /// </summary>
        public BEncodeDictionary Element => _element;

        /// <summary>
        /// 获取此文件元素在 "files" 元素列表中的索引。如果源实例是单文件种子，值为 0。此参数仅在读取文档后有效，源实例修改可能导致此参数指向错误。
        /// </summary>
        public int Index => _index;

        /// <summary>
        /// 获取一个值，指示源实例是否是多文件格式。
        /// </summary>
        public bool Multiple => _multiple;

        #endregion Members

        #region Construction

        internal TorrentFileEntry(BEncodeDictionary element, int index, bool multiple, TorrentAnalysis document)
        {
            _multiple = multiple;
            _element = element;
            _index = index;
            _document = document;
        }

        #endregion Construction

        /// <summary>
        /// 获取文件的字节大小。
        /// </summary>
        /// <returns></returns>
        public long GetLength()
        {
            return ((BEncodeInteger)_element["length"]).GetInt64Value();
        }

        /// <summary>
        /// 获取路径的拆分集合。转换元素的值时默认使用 <see cref="BEncodeDocument.Encoding"/> 编码。
        /// </summary>
        /// <returns></returns>
        public string[] GetPath()
        {
            return GetPath(_document.Encoding);
        }

        /// <summary>
        /// 获取路径的拆分集合。
        /// </summary>
        /// <param name="valueEncoding">用于转换元素的值的字符编码。</param>
        /// <returns></returns>
        public string[] GetPath(Encoding valueEncoding)
        {
            if (_multiple)
            {
                List<string> paths = new List<string>();
                foreach (BEncodeString path in _element.GetValue<BEncodeList>("path").Cast<BEncodeString>())
                {
                    paths.Add(path.GetStringValue(valueEncoding));
                }
                return paths.ToArray();
            }
            else
            {
                string name = _element.GetValue<BEncodeString>("name").GetStringValue(valueEncoding);
                return new string[] { name };
            }
        }

        /// <summary>
        /// 设置路径的拆分集合。单文件格式将集合的最后一项设置为 "name" 元素。不可为 <see langword="null"/> 和 <see langword="Empty"/> 集合。
        /// 修改会导致 "info" 元素内容改变。修改结果会反应在从中获取元素的源实例中。转换元素的值时默认使用 <see cref="BEncodeDocument.Encoding"/> 编码。
        /// </summary>
        /// <param name="paths">路径的拆分集合。不可为 <see langword="null"/>。</param>
        /// <exception cref="Exception"/>
        public void SetPath(string[] paths)
        {
            SetPath(paths, _document.Encoding);
        }

        /// <summary>
        /// 设置路径的拆分集合。单文件格式将集合的最后一项设置为 "name" 元素。不可为 <see langword="null"/> 和 <see langword="Empty"/> 集合。
        /// 修改会导致 "info" 元素内容改变。修改结果会反应在从中获取元素的源实例中。
        /// </summary>
        /// <param name="paths">路径的拆分集合。不可为 <see langword="null"/>。</param>
        /// <param name="valueEncoding">用于转换元素的值的字符编码。不使用所属的 <see cref="BEncodeDocument"/> 实例的默认字符编码。</param>
        /// <exception cref="Exception"/>
        public void SetPath(string[] paths, Encoding valueEncoding)
        {
            if (paths is null)
            {
                throw new ArgumentNullException(nameof(paths));
            }
            if (_multiple)
            {
                BEncodeList path = _element.GetValue<BEncodeList>("path");
                path.Clear();
                foreach (string p in paths)
                {
                    path.Add(new BEncodeString(p, valueEncoding, _document));
                }
            }
            else
            {
                _document.SetName(paths[paths.Length - 1], valueEncoding);
            }
        }
    }
}