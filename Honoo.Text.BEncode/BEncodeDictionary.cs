using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace Honoo.Text
{
    /// <summary>
    /// BEncode 字典类型。
    /// </summary>
    public sealed class BEncodeDictionary : BEncodeValue, IEnumerable<KeyValuePair<string, BEncodeValue>>
    {
        #region Class

        /// <summary>
        /// 代表此元素集合的键的集合。
        /// </summary>
        public sealed class KeyCollection : IEnumerable<string>, IEnumerable
        {
            #region Properties

            private readonly Dictionary<string, BEncodeValue> _elements;

            /// <summary>
            /// 获取元素集合的键的元素数。
            /// </summary>
            public int Count => _elements.Count;

            #endregion Properties

            internal KeyCollection(Dictionary<string, BEncodeValue> elements)
            {
                _elements = elements;
            }

            /// <summary>
            /// 从指定数组索引开始将键元素复制到到指定数组。
            /// </summary>
            /// <param name="array"></param>
            /// <param name="arrayIndex">目标数组中从零开始的索引，从此处开始复制。</param>
            public void CopyTo(string[] array, int arrayIndex)
            {
                _elements.Keys.CopyTo(array, arrayIndex);
            }

            /// <summary>
            /// 支持在泛型集合上进行简单迭代。
            /// </summary>
            /// <returns></returns>
            public IEnumerator<string> GetEnumerator()
            {
                return _elements.Keys.GetEnumerator();
            }

            IEnumerator IEnumerable.GetEnumerator()
            {
                return _elements.Keys.GetEnumerator();
            }
        }

        /// <summary>
        /// 代表此元素集合的值的集合。
        /// </summary>
        public sealed class ValueCollection : IEnumerable<BEncodeValue>
        {
            #region Properties

            private readonly Dictionary<string, BEncodeValue> _elements;

            /// <summary>
            /// 获取元素集合的值的元素数。
            /// </summary>
            public int Count => _elements.Count;

            #endregion Properties

            internal ValueCollection(Dictionary<string, BEncodeValue> elements)
            {
                _elements = elements;
            }

            /// <summary>
            /// 从指定数组索引开始将值元素复制到到指定数组。
            /// </summary>
            /// <param name="array"></param>
            /// <param name="arrayIndex">目标数组中从零开始的索引，从此处开始复制。</param>
            public void CopyTo(BEncodeValue[] array, int arrayIndex)
            {
                _elements.Values.CopyTo(array, arrayIndex);
            }

            /// <summary>
            /// 支持在泛型集合上进行简单迭代。
            /// </summary>
            /// <returns></returns>
            public IEnumerator<BEncodeValue> GetEnumerator()
            {
                return _elements.Values.GetEnumerator();
            }

            IEnumerator IEnumerable.GetEnumerator()
            {
                return _elements.Values.GetEnumerator();
            }
        }

        #endregion Class

        #region Properties

        private readonly Dictionary<string, BEncodeValue> _elements = new Dictionary<string, BEncodeValue>();
        private readonly KeyCollection _keyExhibits;
        private readonly ValueCollection _valueExhibits;

        /// <summary>
        /// 获取元素集合中包含的元素数。
        /// </summary>
        public int Count => _elements.Count;

        /// <summary>
        /// 获取元素集合的键的集合。
        /// </summary>
        public KeyCollection Keys => _keyExhibits;

        /// <summary>
        /// 获取元素集合的值的集合。
        /// </summary>
        public ValueCollection Values => _valueExhibits;

        /// <summary>
        /// 获取或设置具有指定键的元素的值。直接赋值等同于 AddOrUpdate 方法。
        /// </summary>
        /// <param name="key">元素的键。</param>
        /// <returns></returns>
        /// <exception cref="Exception"/>
        public BEncodeValue this[string key]
        {
            get => _elements.TryGetValue(key, out BEncodeValue value) ? value : null;
            set { AddOrUpdate(key, value); }
        }

        #endregion Properties

        #region Construction

        /// <summary>
        /// 初始化 BEncodeDictionary 类的新实例。
        /// </summary>
        public BEncodeDictionary() : base(BEncodeValueKind.Dictionary)
        {
            _keyExhibits = new KeyCollection(_elements);
            _valueExhibits = new ValueCollection(_elements);
        }

        /// <summary>
        /// 初始化 BEncodeDictionary 类的新实例。对自身和子集中字典类型的键默认使用 Encoding.UTF8 编码。
        /// </summary>
        /// <param name="content">指定从中读取的流。</param>
        /// <exception cref="Exception"></exception>
        public BEncodeDictionary(Stream content) : this(content, Encoding.UTF8)
        {
        }

        /// <summary>
        /// 初始化 BEncodeDictionary 类的新实例。
        /// </summary>
        /// <param name="content">指定从中读取的流。</param>
        /// <param name="keyEncoding">对自身和子集中字典类型的键解码使用的字符编码。</param>
        /// <exception cref="Exception"></exception>
        public BEncodeDictionary(Stream content, Encoding keyEncoding) : base(BEncodeValueKind.Dictionary)
        {
            int kc = content.ReadByte();
            if (kc != 100)  // 'd'
            {
                throw new Exception($"The header char is not a dictionary identification char 'd'. Stop at position: {content.Position}.");
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
                    content.Seek(-1, SeekOrigin.Current);
                    var single = new BEncodeSingle(content);
                    string key = single.GetStringValue(keyEncoding);
                    kc = content.ReadByte();
                    switch (kc)
                    {
                        case 100: content.Seek(-1, SeekOrigin.Current); _elements.Add(key, new BEncodeDictionary(content, keyEncoding)); break;  // 'd'
                        case 108: content.Seek(-1, SeekOrigin.Current); _elements.Add(key, new BEncodeList(content, keyEncoding)); break;        // 'l'
                        case 105: content.Seek(-1, SeekOrigin.Current); _elements.Add(key, new BEncodeInteger(content)); break;                  // 'i'
                        case 48:                                                                                                                 // '0'
                        case 49:                                                                                                                 // '1'
                        case 50:                                                                                                                 // '2'
                        case 51:                                                                                                                 // '3'
                        case 52:                                                                                                                 // '4'
                        case 53:                                                                                                                 // '5'
                        case 54:                                                                                                                 // '6'
                        case 55:                                                                                                                 // '7'
                        case 56:                                                                                                                 // '8'
                        case 57: content.Seek(-1, SeekOrigin.Current); _elements.Add(key, new BEncodeSingle(content)); break;                    // '9'
                        default: throw new Exception($"The incorrect identification char '{kc}'. Stop at position: {content.Position}.");
                    }
                    kc = content.ReadByte();
                }
            }
            _keyExhibits = new KeyCollection(_elements);
            _valueExhibits = new ValueCollection(_elements);
        }

        #endregion Construction

        /// <summary>
        /// 添加或更新一个元素。
        /// </summary>
        /// <param name="key">元素的键。</param>
        /// <param name="value">元素的值。</param>
        /// <exception cref="Exception"/>
        public void AddOrUpdate(string key, BEncodeValue value)
        {
            if (string.IsNullOrWhiteSpace(key))
            {
                throw new ArgumentNullException(nameof(key));
            }
            if (value == null)
            {
                _elements.Remove(key);
            }
            else
            {
                if (_elements.TryGetValue(key, out _))
                {
                    _elements[key] = value;
                }
                else
                {
                    _elements.Add(key, value);
                }
            }
        }

        /// <summary>
        /// 从元素集合中移除所有元素。
        /// </summary>
        public void Clear()
        {
            _elements.Clear();
        }

        /// <summary>
        /// 确定元素集合是否包含带有指定键的元素。
        /// </summary>
        /// <param name="key">元素的键。</param>
        /// <returns></returns>
        /// <exception cref="Exception"/>
        public bool ContainsKey(string key)
        {
            return _elements.ContainsKey(key);
        }

        /// <summary>
        /// 支持在泛型集合上进行简单迭代。
        /// </summary>
        /// <returns></returns>
        public IEnumerator<KeyValuePair<string, BEncodeValue>> GetEnumerator()
        {
            return _elements.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return _elements.GetEnumerator();
        }

        /// <summary>
        /// 从元素集合中移除带有指定键的元素。
        /// <para/>如果该元素成功移除，返回 true。如果没有找到指定键，则仍返回 false。
        /// </summary>
        /// <param name="key">元素的键。</param>
        /// <returns></returns>
        /// <exception cref="Exception"/>
        public bool Remove(string key)
        {
            return _elements.Remove(key);
        }

        /// <summary>
        /// 保存到指定的流。对自身和子集中字典类型的键编码默认使用 Encoding.UTF8 编码。
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
        /// <param name="keyEncoding">对自身和子集中字典类型的键编码使用的字符编码。</param>
        /// <exception cref="Exception"/>
        public void Save(Stream stream, Encoding keyEncoding)
        {
            SaveInternal(stream, keyEncoding);
        }

        /// <summary>
        /// 获取与指定键关联的元素的值。
        /// </summary>
        /// <param name="key">元素的键。</param>
        /// <param name="value">元素的值。</param>
        /// <returns></returns>
        /// <exception cref="Exception"/>
        public bool TryGetValue(string key, out BEncodeDictionary value)
        {
            if (_elements.TryGetValue(key, out BEncodeValue val))
            {
                value = (BEncodeDictionary)val;
                return true;
            }
            else
            {
                value = null;
                return false;
            }
        }

        /// <summary>
        /// 获取与指定键关联的元素的值。
        /// </summary>
        /// <param name="key">元素的键。</param>
        /// <param name="value">元素的值。</param>
        /// <returns></returns>
        /// <exception cref="Exception"/>
        public bool TryGetValue(string key, out BEncodeList value)
        {
            if (_elements.TryGetValue(key, out BEncodeValue val))
            {
                value = (BEncodeList)val;
                return true;
            }
            else
            {
                value = null;
                return false;
            }
        }

        /// <summary>
        /// 获取与指定键关联的元素的值。
        /// </summary>
        /// <param name="key">元素的键。</param>
        /// <param name="value">元素的值。</param>
        /// <returns></returns>
        /// <exception cref="Exception"/>
        public bool TryGetValue(string key, out BEncodeInteger value)
        {
            if (_elements.TryGetValue(key, out BEncodeValue val))
            {
                value = (BEncodeInteger)val;
                return true;
            }
            else
            {
                value = null;
                return false;
            }
        }

        /// <summary>
        /// 获取与指定键关联的元素的值。
        /// </summary>
        /// <param name="key">元素的键。</param>
        /// <param name="value">元素的值。</param>
        /// <returns></returns>
        /// <exception cref="Exception"/>
        public bool TryGetValue(string key, out BEncodeSingle value)
        {
            if (_elements.TryGetValue(key, out BEncodeValue val))
            {
                value = (BEncodeSingle)val;
                return true;
            }
            else
            {
                value = null;
                return false;
            }
        }

        /// <summary>
        /// 获取与指定键关联的元素的值。
        /// </summary>
        /// <param name="key">元素的键。</param>
        /// <param name="value">元素的值。</param>
        /// <returns></returns>
        /// <exception cref="Exception"/>
        public bool TryGetValue(string key, out BEncodeValue value)
        {
            return _elements.TryGetValue(key, out value);
        }

        internal override void SaveInternal(Stream stream, Encoding keyEncoding)
        {
            if (stream == null)
            {
                throw new ArgumentNullException(nameof(stream));
            }
            stream.WriteByte(100);  // 'd'
            List<string> keys = new List<string>(_elements.Keys);
            keys.Sort();
            foreach (string key in keys)
            {
                BEncodeSingle keyEncode = new BEncodeSingle(key, keyEncoding);
                keyEncode.SaveInternal(stream, null);
                _elements[key].SaveInternal(stream, keyEncoding);
            }
            //var enumerator = _elements.GetEnumerator();
            //while (enumerator.MoveNext())
            //{
            //    var key = new BEncodeSingle(enumerator.Current.Key, keyEncoding);
            //    key.SaveInternal(stream, keyEncoding); // keyEncoding 忽略
            //    enumerator.Current.Value.SaveInternal(stream, keyEncoding);
            //}
            stream.WriteByte(101);  // 'e'
        }
    }
}