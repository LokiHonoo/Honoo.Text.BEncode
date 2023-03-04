using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace Honoo.Text
{
    /// <summary>
    /// BEncode 列表类型。
    /// </summary>
    public sealed class BEncodeList : BEncodeValue, IEnumerable<BEncodeValue>
    {
        private readonly List<BEncodeValue> _elements = new List<BEncodeValue>();

        /// <summary>
        /// 获取元素集合中包含的元素数。
        /// </summary>
        public int Count => _elements.Count;

        /// <summary>
        /// 获取或设置指定索引的元素的值。
        /// </summary>
        /// <param name="index">元素的索引。</param>
        /// <returns></returns>
        public BEncodeValue this[int index]
        {
            get => index < _elements.Count ? _elements[index] : null;
            set => _elements[index] = value;
        }

        #region Construction

        /// <summary>
        /// 初始化 BEncodeList 类的新实例。
        /// </summary>
        public BEncodeList() : base(BEncodeValueKind.List)
        {
        }

        /// <summary>
        /// 初始化 BEncodeList 类的新实例。对子集中字典类型的键默认使用 Encoding.UTF8 编码。
        /// </summary>
        /// <param name="content">指定从中读取的流。</param>
        /// <exception cref="Exception"></exception>
        public BEncodeList(Stream content) : this(content, Encoding.UTF8)
        {
        }

        /// <summary>
        /// 初始化 BEncodeList 类的新实例。
        /// </summary>
        /// <param name="content">指定从中读取的流。</param>
        /// <param name="keyEncoding">对子集中字典类型的键解码使用的字符编码。</param>
        /// <exception cref="Exception"></exception>
        public BEncodeList(Stream content, Encoding keyEncoding) : base(BEncodeValueKind.List)
        {
            int kc = content.ReadByte();
            if (kc != 108)  // 'l'
            {
                throw new Exception($"The header char is not a list identification char 'l'. Stop at position: {content.Position}.");
            }
            kc = content.ReadByte();
            while (true)
            {
                if (kc == 101)  // 'e'
                {
                    break;
                }
                else
                {
                    switch (kc)
                    {
                        case 100: content.Seek(-1, SeekOrigin.Current); _elements.Add(new BEncodeDictionary(content, keyEncoding)); break;  // 'd'
                        case 108: content.Seek(-1, SeekOrigin.Current); _elements.Add(new BEncodeList(content, keyEncoding)); break;        // 'l'
                        case 105: content.Seek(-1, SeekOrigin.Current); _elements.Add(new BEncodeInteger(content)); break;                  // 'i'
                        case 48:                                                                                                            // '0'
                        case 49:                                                                                                            // '1'
                        case 50:                                                                                                            // '2'
                        case 51:                                                                                                            // '3'
                        case 52:                                                                                                            // '4'
                        case 53:                                                                                                            // '5'
                        case 54:                                                                                                            // '6'
                        case 55:                                                                                                            // '7'
                        case 56:                                                                                                            // '8'
                        case 57: content.Seek(-1, SeekOrigin.Current); _elements.Add(new BEncodeSingle(content)); break;                    // '9'
                        default: throw new Exception($"The incorrect identification char '{kc}'. Stop at position: {content.Position}.");
                    }
                    kc = content.ReadByte();
                }
            }
        }

        #endregion Construction

        /// <summary>
        /// 添加一个元素。
        /// </summary>
        /// <param name="value">元素的值。</param>
        /// <exception cref="Exception"/>
        public BEncodeValue Add(BEncodeValue value)
        {
            if (value == null)
            {
                throw new ArgumentNullException(nameof(value));
            }
            _elements.Add(value);
            return value;
        }

        /// <summary>
        /// 添加一个元素。
        /// </summary>
        /// <param name="value">元素的值。</param>
        /// <exception cref="Exception"/>
        public T Add<T>(T value) where T : BEncodeValue
        {
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
        /// <param name="values">元素的集合。</param>
        /// <exception cref="Exception"/>
        public IEnumerable<BEncodeValue> AddRange(IEnumerable<BEncodeValue> values)
        {
            if (values == null)
            {
                throw new ArgumentNullException(nameof(values));
            }
            _elements.AddRange(values);
            return values;
        }

        /// <summary>
        /// 添加元素集合。
        /// </summary>
        /// <param name="values">元素的集合。</param>
        /// <exception cref="Exception"/>
        public T AddRange<T>(T values) where T : IEnumerable<BEncodeValue>
        {
            if (values == null)
            {
                throw new ArgumentNullException(nameof(values));
            }
            _elements.AddRange(values);
            return values;
        }

        /// <summary>
        /// 从元素集合中移除所有元素。
        /// </summary>
        public void Clear()
        {
            _elements.Clear();
        }

        /// <summary>
        /// 确定指定元素是否在集合中。
        /// </summary>
        /// <param name="item">搜索的指定对象。</param>
        /// <returns></returns>
        public bool Contains(BEncodeValue item)
        {
            return _elements.Contains(item);
        }

        /// <summary>
        /// 从指定数组索引开始将值元素复制到到指定数组。
        /// </summary>
        /// <param name="array">要复制到的目标数组。</param>
        /// <param name="arrayIndex">目标数组中从零开始的索引，从此处开始复制。</param>
        public void CopyTo(BEncodeValue[] array, int arrayIndex)
        {
            _elements.CopyTo(array, arrayIndex);
        }

        /// <summary>
        /// 支持在泛型集合上进行简单迭代。
        /// </summary>
        /// <returns></returns>
        public IEnumerator<BEncodeValue> GetEnumerator()
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
        /// <param name="item">搜索的指定对象。</param>
        /// <returns></returns>
        public int IndexOf(BEncodeValue item)
        {
            return _elements.IndexOf(item);
        }

        /// <summary>
        /// 将元素插入指定索引处。
        /// </summary>
        /// <param name="index">指定索引。</param>
        /// <param name="item">要插入的元素。</param>
        public void Insert(int index, BEncodeValue item)
        {
            _elements.Insert(index, item);
        }

        /// <summary>
        /// 从元素集合中移除指定元素。
        /// </summary>
        /// <param name="value">要移除的元素。</param>
        /// <returns></returns>
        /// <exception cref="Exception"/>
        public bool Remove(BEncodeValue value)
        {
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
            _elements.RemoveAt(index);
        }

        /// <summary>
        /// 保存到指定的流。对子集中字典类型的键默认使用 Encoding.UTF8 编码。
        /// </summary>
        /// <param name="stream">指定保存的目标流。</param>
        /// <exception cref="Exception"/>
        public void Save(Stream stream)
        {
            Save(stream, Encoding.UTF8);
        }

        /// <summary>
        /// 保存到指定的流。
        /// </summary>
        /// <param name="stream">指定保存的目标流。</param>
        /// <param name="keyEncoding">对子集中字典类型的键编码使用的字符编码。</param>
        /// <exception cref="Exception"/>
        public void Save(Stream stream, Encoding keyEncoding)
        {
            SaveInternal(stream, keyEncoding);
        }

        internal override void SaveInternal(Stream stream, Encoding keyEncoding)
        {
            if (stream == null)
            {
                throw new ArgumentNullException(nameof(stream));
            }
            stream.WriteByte(108);  // 'l'
            var enumerator = _elements.GetEnumerator();
            while (enumerator.MoveNext())
            {
                enumerator.Current.SaveInternal(stream, keyEncoding);
            }
            stream.WriteByte(101);  // 'e'
        }
    }
}