using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;

namespace Honoo.Text.BEncode
{
    /// <summary>
    /// BEncode 列表类型。
    /// </summary>
    public class BEncodeList : BEncodeElement, IEnumerable<BEncodeElement>, IReadOnlyBEncodeList
    {
        private readonly List<BEncodeElement> _elements = new List<BEncodeElement>();
        private bool _isReadOnly;

        /// <summary>
        /// 获取元素集合中包含的元素数。
        /// </summary>
        public int Count => _elements.Count;

        /// <summary>
        /// 获取一个值，该值指示此 <see cref="BEncodeList"/> 是否为只读。
        /// </summary>
        public bool IsReadOnly => _isReadOnly;

        /// <summary>
        /// 获取或设置指定索引的元素的值。
        /// </summary>
        /// <param name="index">元素的索引。</param>
        /// <returns></returns>
        /// <exception cref="Exception"/>
        public BEncodeElement this[int index]
        {
            get => _elements[index];
            set
            {
                RemoveAt(index);
                Insert(index, value);
            }
        }

        #region Construction

        /// <summary>
        /// 初始化 BEncodeList 类的新实例。
        /// </summary>
        public BEncodeList() : base(BEncodeElementKind.BEncodeList)
        {
        }

        /// <summary>
        /// 初始化 BEncodeList 类的新实例。
        /// </summary>
        /// <param name="content">指定从中读取的流。定位必须在编码标记 <see langword="l"/> 处。</param>
        /// <exception cref="Exception"/>
        public BEncodeList(Stream content) : this(content, false)
        {
        }

        /// <summary>
        /// 初始化 BEncodeList 类的新实例。
        /// </summary>
        /// <param name="content">指定从中读取的流。定位必须在编码标记 <see langword="l"/> 处。</param>
        /// <param name="readOnly">指定此 <see cref="BEncodeList"/> 及子元素是只读的。</param>
        /// <exception cref="Exception"/>
        public BEncodeList(Stream content, bool readOnly) : base(BEncodeElementKind.BEncodeList)
        {
            if (content == null)
            {
                throw new ArgumentNullException(nameof(content));
            }
            int kc = content.ReadByte();
            if (kc != 108)  // "l"
            {
                throw new ArgumentException($"The header char is not a list identification char \"l\". Stop at position: {content.Position}.");
            }
            kc = content.ReadByte();
            while (true)
            {
                if (kc == 101)  // "e"
                {
                    break;
                }
                else
                {
                    switch (kc)
                    {
                        case 100: content.Seek(-1, SeekOrigin.Current); _elements.Add(new BEncodeDictionary(content, readOnly)); break;// "d"
                        case 108: content.Seek(-1, SeekOrigin.Current); _elements.Add(new BEncodeList(content, readOnly)); break;      // "l"
                        case 105: content.Seek(-1, SeekOrigin.Current); _elements.Add(new BEncodeInteger(content, readOnly)); break;   // "i"
                        case 48:                                                                                                       // "0"
                        case 49:                                                                                                       // "1"
                        case 50:                                                                                                       // "2"
                        case 51:                                                                                                       // "3"
                        case 52:                                                                                                       // "4"
                        case 53:                                                                                                       // "5"
                        case 54:                                                                                                       // "6"
                        case 55:                                                                                                       // "7"
                        case 56:                                                                                                       // "8"
                        case 57: content.Seek(-1, SeekOrigin.Current); _elements.Add(new BEncodeString(content, readOnly)); break;     // "9"
                        default: throw new ArgumentException($"The incorrect identification char \"{kc}\". Stop at position: {content.Position}.");
                    }
                    kc = content.ReadByte();
                }
            }
            _isReadOnly = readOnly;
        }

        #endregion Construction

        /// <summary>
        /// 添加一个元素。
        /// </summary>
        /// <typeparam name="T">指定元素类型。</typeparam>
        /// <param name="value">元素的值。</param>
        /// <exception cref="Exception"/>
        public T Add<T>(T value) where T : BEncodeElement
        {
            if (_isReadOnly)
            {
                throw new NotSupportedException("Collection is read-only.");
            }
            if (value == null)
            {
                throw new ArgumentNullException(nameof(value));
            }
            _elements.Add(value);
            return value;
        }

        /// <summary>
        /// 添加元素集合。
        /// </summary>
        /// <typeparam name="T">指定元素类型。</typeparam>
        /// <param name="values">元素的集合。</param>
        /// <exception cref="Exception"/>
        public IEnumerable<BEncodeElement> AddRange<T>(IEnumerable<BEncodeElement> values) where T : BEncodeElement
        {
            if (_isReadOnly)
            {
                throw new NotSupportedException("Collection is read-only.");
            }
            if (values == null)
            {
                throw new ArgumentNullException(nameof(values));
            }
            _elements.AddRange(values);
            return values;
        }

        /// <summary>
        /// 获取此实例的只读接口。
        /// </summary>
        public IReadOnlyBEncodeList AsReadOnly()
        {
            return this;
        }

        /// <summary>
        /// 从元素集合中移除所有元素。
        /// </summary>
        /// <exception cref="Exception"/>
        public void Clear()
        {
            if (_isReadOnly)
            {
                throw new NotSupportedException("Collection is read-only.");
            }
            _elements.Clear();
        }

        /// <summary>
        /// 确定指定元素是否在集合中。
        /// </summary>
        /// <typeparam name="T">指定元素类型。</typeparam>
        /// <param name="value">搜索的指定对象。</param>
        /// <returns></returns>
        public bool Contains<T>(T value) where T : BEncodeElement
        {
            return _elements.Contains(value);
        }

        /// <summary>
        /// 从指定数组索引开始将值元素复制到到指定数组。
        /// </summary>
        /// <typeparam name="T">指定元素类型。</typeparam>
        /// <param name="array">要复制到的目标数组。</param>
        /// <param name="arrayIndex">目标数组中从零开始的索引，从此处开始复制。</param>
        public void CopyTo<T>(T[] array, int arrayIndex) where T : BEncodeElement
        {
            _elements.CopyTo(array, arrayIndex);
        }

        /// <summary>
        /// 支持在泛型集合上进行简单迭代。
        /// </summary>
        /// <returns></returns>
        public IEnumerator<BEncodeElement> GetEnumerator()
        {
            return _elements.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return _elements.GetEnumerator();
        }

        /// <summary>
        /// 搜索指定对象，并返回第一个匹配项从零开始的索引。
        /// </summary>
        /// <typeparam name="T">指定元素类型。</typeparam>
        /// <param name="value">搜索的指定对象。</param>
        /// <returns></returns>
        public int IndexOf<T>(T value) where T : BEncodeElement
        {
            return _elements.IndexOf(value);
        }

        /// <summary>
        /// 将元素插入指定索引处。
        /// </summary>
        /// <typeparam name="T">指定元素类型。</typeparam>
        /// <param name="index">指定索引。</param>
        /// <param name="value">要插入的元素。</param>
        /// <exception cref="Exception"/>
        public void Insert<T>(int index, T value) where T : BEncodeElement
        {
            if (_isReadOnly)
            {
                throw new NotSupportedException("Collection is read-only.");
            }
            _elements.Insert(index, value);
        }

        /// <summary>
        /// 从元素集合中移除指定元素。
        /// </summary>
        /// <typeparam name="T">指定元素类型。</typeparam>
        /// <param name="value">要移除的元素。</param>
        /// <returns></returns>
        /// <exception cref="Exception"/>
        public bool Remove<T>(T value) where T : BEncodeElement
        {
            if (_isReadOnly)
            {
                throw new NotSupportedException("Collection is read-only.");
            }
            return _elements.Remove(value);
        }

        /// <summary>
        /// 从元素集合中移除指定索引处的元素。
        /// </summary>
        /// <param name="index">要移除的元素的索引。</param>
        /// <returns></returns>
        /// <exception cref="Exception"/>
        public void RemoveAt(int index)
        {
            if (_isReadOnly)
            {
                throw new NotSupportedException("Collection is read-only.");
            }
            _elements.RemoveAt(index);
        }

        /// <summary>
        /// 保存到指定的流。
        /// </summary>
        /// <param name="stream">指定保存的目标流。</param>
        /// <exception cref="Exception"/>
        public override void Save(Stream stream)
        {
            if (stream == null)
            {
                throw new ArgumentNullException(nameof(stream));
            }
            stream.WriteByte(108);  // "l"
            var enumerator = _elements.GetEnumerator();
            while (enumerator.MoveNext())
            {
                enumerator.Current.Save(stream);
            }
            stream.WriteByte(101);  // "e"
        }

        internal override void ChangeReadOnly(bool isReadOnly)
        {
            var enumerator = _elements.GetEnumerator();
            while (enumerator.MoveNext())
            {
                enumerator.Current.ChangeReadOnly(isReadOnly);
            }
            _isReadOnly = isReadOnly;
        }
    }
}