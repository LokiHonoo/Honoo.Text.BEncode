using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace Honoo.Text.BEncode
{
    /// <summary>
    /// BEncode 字典类型。
    /// </summary>
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Naming", "CA1711:标识符应采用正确的后缀", Justification = "<挂起>")]
    public class BEncodeDictionary : BEncodeValue, IEnumerable<KeyValuePair<BEncodeString, BEncodeValue>>
    {
        #region Class

        /// <summary>
        /// 代表此元素集合的键的集合。
        /// </summary>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Design", "CA1034:嵌套类型应不可见", Justification = "<挂起>")]
        public sealed class KeyCollection : IEnumerable<BEncodeString>
        {
            #region Properties

            private readonly SortedDictionary<BEncodeString, BEncodeValue> _elements;

            /// <summary>
            /// 获取元素集合的键的元素数。
            /// </summary>
            public int Count => _elements.Count;

            #endregion Properties

            internal KeyCollection(SortedDictionary<BEncodeString, BEncodeValue> elements)
            {
                _elements = elements;
            }

            /// <summary>
            /// 从指定数组索引开始将键元素复制到到指定数组。
            /// </summary>
            /// <param name="array">目标数组。</param>
            /// <param name="arrayIndex">目标数组中从零开始的索引，从此处开始复制。</param>
            public void CopyTo(BEncodeString[] array, int arrayIndex)
            {
                _elements.Keys.CopyTo(array, arrayIndex);
            }

            /// <summary>
            /// 从指定数组索引开始将键元素的文本值复制到到指定数组。默认使用 Encoding.UTF8 编码。
            /// </summary>
            /// <param name="array">目标数组。</param>
            /// <param name="arrayIndex">目标数组中从零开始的索引，从此处开始复制。</param>
            public void CopyTo(string[] array, int arrayIndex)
            {
                CopyTo(array, arrayIndex, Encoding.UTF8);
            }

            /// <summary>
            /// 从指定数组索引开始将键元素的文本值复制到到指定数组。
            /// </summary>
            /// <param name="array">目标数组。</param>
            /// <param name="arrayIndex">目标数组中从零开始的索引，从此处开始复制。</param>
            /// <param name="encoding">用于转换的字符编码。</param>

            public void CopyTo(string[] array, int arrayIndex, Encoding encoding)
            {
                if (array == null)
                {
                    throw new ArgumentNullException(nameof(array));
                }
                foreach (var key in _elements.Keys)
                {
                    array[arrayIndex] = key.GetStringValue(encoding);
                    arrayIndex++;
                }
            }

            /// <summary>
            /// 支持在泛型集合上进行简单迭代。
            /// </summary>
            /// <returns></returns>
            public IEnumerator<BEncodeString> GetEnumerator()
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
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Design", "CA1034:嵌套类型应不可见", Justification = "<挂起>")]
        public sealed class ValueCollection : IEnumerable<BEncodeValue>
        {
            #region Properties

            private readonly SortedDictionary<BEncodeString, BEncodeValue> _elements;

            /// <summary>
            /// 获取元素集合的值的元素数。
            /// </summary>
            public int Count => _elements.Count;

            #endregion Properties

            internal ValueCollection(SortedDictionary<BEncodeString, BEncodeValue> elements)
            {
                _elements = elements;
            }

            /// <summary>
            /// 从指定数组索引开始将值元素复制到到指定数组。
            /// </summary>
            /// <param name="array">目标数组。</param>
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

        private readonly SortedDictionary<BEncodeString, BEncodeValue> _elements = new SortedDictionary<BEncodeString, BEncodeValue>();
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
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Design", "CA1043:将整型或字符串参数用于索引器", Justification = "<挂起>")]
        public BEncodeValue this[BEncodeString key]
        {
            get => _elements.TryGetValue(key, out BEncodeValue value) ? value : null;
            set { AddOrUpdate(key, value); }
        }

        /// <summary>
        /// 获取或设置具有指定键的元素的值。比较元素的键时默认使用 Encoding.UTF8 编码。直接赋值等同于 AddOrUpdate 方法。
        /// </summary>
        /// <param name="key">元素的键。</param>
        /// <returns></returns>
        /// <exception cref="Exception"/>
        public BEncodeValue this[string key]
        {
            get
            {
                var k = new BEncodeString(key, Encoding.UTF8);
                return _elements.TryGetValue(k, out BEncodeValue value) ? value : null;
            }
            set
            {
                AddOrUpdate(key, value);
            }
        }

        /// <summary>
        /// 获取或设置具有指定键的元素的值。直接赋值等同于 AddOrUpdate 方法。
        /// </summary>
        /// <param name="key">元素的键。</param>
        /// <param name="keyEncoding">比较元素的键时使用的字符编码。</param>
        /// <returns></returns>
        /// <exception cref="Exception"/>
        public BEncodeValue this[string key, Encoding keyEncoding]
        {
            get
            {
                var k = new BEncodeString(key, keyEncoding);
                return _elements.TryGetValue(k, out BEncodeValue value) ? value : null;
            }
            set
            {
                AddOrUpdate(key, value);
            }
        }

        #endregion Properties

        #region Construction

        /// <summary>
        /// 初始化 BEncodeDictionary 类的新实例。
        /// </summary>
        public BEncodeDictionary() : base(BEncodeValueKind.BEncodeDictionary)
        {
            _keyExhibits = new KeyCollection(_elements);
            _valueExhibits = new ValueCollection(_elements);
        }

        /// <summary>
        /// 初始化 BEncodeDictionary 类的新实例。
        /// </summary>
        /// <param name="content">指定从中读取的流。</param>
        /// <exception cref="Exception"></exception>
        public BEncodeDictionary(Stream content) : base(BEncodeValueKind.BEncodeDictionary)
        {
            if (content == null)
            {
                throw new ArgumentNullException(nameof(content));
            }
            int kc = content.ReadByte();
            if (kc != 100)  // 'd'
            {
                throw new ArgumentException($"The header char is not a dictionary identification char 'd'. Stop at position: {content.Position}.");
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
                    var key = new BEncodeString(content);
                    kc = content.ReadByte();
                    switch (kc)
                    {
                        case 100: content.Seek(-1, SeekOrigin.Current); _elements.Add(key, new BEncodeDictionary(content)); break;               // 'd'
                        case 108: content.Seek(-1, SeekOrigin.Current); _elements.Add(key, new BEncodeList(content)); break;                     // 'l'
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
                        case 57: content.Seek(-1, SeekOrigin.Current); _elements.Add(key, new BEncodeString(content)); break;                    // '9'
                        default: throw new ArgumentException($"The incorrect identification char '{kc}'. Stop at position: {content.Position}.");
                    }
                    kc = content.ReadByte();
                }
            }
            _keyExhibits = new KeyCollection(_elements);
            _valueExhibits = new ValueCollection(_elements);
        }

        #endregion Construction

        #region Add

        /// <summary>
        /// 添加一个元素。
        /// </summary>
        /// <param name="key">元素的键。</param>
        /// <param name="value">元素的值。</param>
        /// <exception cref="Exception"/>
        public T Add<T>(BEncodeString key, T value) where T : BEncodeValue
        {
            if (key == null)
            {
                throw new ArgumentNullException(nameof(key));
            }
            if (value == null)
            {
                throw new ArgumentNullException(nameof(value));
            }
            _elements.Add(key, value);
            return value;
        }

        /// <summary>
        /// 添加一个元素。
        /// </summary>
        /// <param name="key">元素的键。</param>
        /// <param name="value">元素的值。</param>
        /// <exception cref="Exception"/>
        public T Add<T>(string key, T value) where T : BEncodeValue
        {
            if (string.IsNullOrWhiteSpace(key))
            {
                throw new ArgumentNullException(nameof(key));
            }
            var k = new BEncodeString(key, Encoding.UTF8);
            return Add(k, value);
        }

        /// <summary>
        /// 添加一个元素。
        /// </summary>
        /// <param name="key">元素的键。</param>
        /// <param name="keyEncoding">比较元素的键时使用的字符编码。</param>
        /// <param name="value">元素的值。</param>
        /// <exception cref="Exception"/>
        public T Add<T>(string key, Encoding keyEncoding, T value) where T : BEncodeValue
        {
            if (string.IsNullOrWhiteSpace(key))
            {
                throw new ArgumentNullException(nameof(key));
            }
            var k = new BEncodeString(key, keyEncoding);
            return Add(k, value);
        }

        #endregion Add

        #region AddOrUpdate

        /// <summary>
        /// 添加或更新一个元素。
        /// </summary>
        /// <param name="key">元素的键。</param>
        /// <param name="value">元素的值。</param>
        /// <exception cref="Exception"/>
        public T AddOrUpdate<T>(BEncodeString key, T value) where T : BEncodeValue
        {
            if (key == null)
            {
                throw new ArgumentNullException(nameof(key));
            }
            if (value == null)
            {
                throw new ArgumentNullException(nameof(value));
            }
            Remove(key);
            Add(key, value);
            return value;
        }

        /// <summary>
        /// 添加或更新一个元素。比较元素的键时默认使用 Encoding.UTF8 编码。
        /// </summary>
        /// <param name="key">元素的键。</param>
        /// <param name="value">元素的值。</param>
        /// <returns></returns>
        /// <exception cref="Exception"/>
        public T AddOrUpdate<T>(string key, T value) where T : BEncodeValue
        {
            if (string.IsNullOrWhiteSpace(key))
            {
                throw new ArgumentNullException(nameof(key));
            }
            var k = new BEncodeString(key, Encoding.UTF8);
            return AddOrUpdate(k, value);
        }

        /// <summary>
        /// 添加或更新一个元素。
        /// </summary>
        /// <param name="key">元素的键。</param>
        /// <param name="keyEncoding">比较元素的键时使用的字符编码。</param>
        /// <param name="value">元素的值。</param>
        /// <returns></returns>
        /// <exception cref="Exception"/>
        public T AddOrUpdate<T>(string key, Encoding keyEncoding, T value) where T : BEncodeValue
        {
            if (string.IsNullOrWhiteSpace(key))
            {
                throw new ArgumentNullException(nameof(key));
            }
            var k = new BEncodeString(key, keyEncoding);
            return AddOrUpdate(k, value);
        }

        #endregion AddOrUpdate

        #region TryGetValue

        /// <summary>
        /// 获取与指定键关联的元素的值。
        /// </summary>
        /// <param name="key">元素的键。</param>
        /// <param name="value">元素的值。</param>
        /// <returns></returns>
        /// <exception cref="Exception"/>
        public bool TryGetValue(BEncodeString key, out BEncodeDictionary value)
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
        /// 获取与指定键关联的元素的值。比较元素的键时默认使用 Encoding.UTF8 编码。
        /// </summary>
        /// <param name="key">元素的键。</param>
        /// <param name="value">元素的值。</param>
        /// <returns></returns>
        /// <exception cref="Exception"/>
        public bool TryGetValue(string key, out BEncodeDictionary value)
        {
            var k = new BEncodeString(key, Encoding.UTF8);
            return TryGetValue(k, out value);
        }

        /// <summary>
        /// 获取与指定键关联的元素的值。
        /// </summary>
        /// <param name="key">元素的键。</param>
        /// <param name="keyEncoding">比较元素的键时使用的字符编码。</param>
        /// <param name="value">元素的值。</param>
        /// <returns></returns>
        /// <exception cref="Exception"/>
        public bool TryGetValue(string key, Encoding keyEncoding, out BEncodeDictionary value)
        {
            var k = new BEncodeString(key, keyEncoding);
            return TryGetValue(k, out value);
        }

        /// <summary>
        /// 获取与指定键关联的元素的值。
        /// </summary>
        /// <param name="key">元素的键。</param>
        /// <param name="value">元素的值。</param>
        /// <returns></returns>
        /// <exception cref="Exception"/>
        public bool TryGetValue(BEncodeString key, out BEncodeList value)
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
        /// 获取与指定键关联的元素的值。比较元素的键时默认使用 Encoding.UTF8 编码。
        /// </summary>
        /// <param name="key">元素的键。</param>
        /// <param name="value">元素的值。</param>
        /// <returns></returns>
        /// <exception cref="Exception"/>
        public bool TryGetValue(string key, out BEncodeList value)
        {
            var k = new BEncodeString(key, Encoding.UTF8);
            return TryGetValue(k, out value);
        }

        /// <summary>
        /// 获取与指定键关联的元素的值。
        /// </summary>
        /// <param name="key">元素的键。</param>
        /// <param name="keyEncoding">比较元素的键时使用的字符编码。</param>
        /// <param name="value">元素的值。</param>
        /// <returns></returns>
        /// <exception cref="Exception"/>
        public bool TryGetValue(string key, Encoding keyEncoding, out BEncodeList value)
        {
            var k = new BEncodeString(key, keyEncoding);
            return TryGetValue(k, out value);
        }

        /// <summary>
        /// 获取与指定键关联的元素的值。
        /// </summary>
        /// <param name="key">元素的键。</param>
        /// <param name="value">元素的值。</param>
        /// <returns></returns>
        /// <exception cref="Exception"/>
        public bool TryGetValue(BEncodeString key, out BEncodeInteger value)
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
        /// 获取与指定键关联的元素的值。比较元素的键时默认使用 Encoding.UTF8 编码。
        /// </summary>
        /// <param name="key">元素的键。</param>
        /// <param name="value">元素的值。</param>
        /// <returns></returns>
        /// <exception cref="Exception"/>
        public bool TryGetValue(string key, out BEncodeInteger value)
        {
            var k = new BEncodeString(key, Encoding.UTF8);
            return TryGetValue(k, out value);
        }

        /// <summary>
        /// 获取与指定键关联的元素的值。
        /// </summary>
        /// <param name="key">元素的键。</param>
        /// <param name="keyEncoding">比较元素的键时使用的字符编码。</param>
        /// <param name="value">元素的值。</param>
        /// <returns></returns>
        /// <exception cref="Exception"/>
        public bool TryGetValue(string key, Encoding keyEncoding, out BEncodeInteger value)
        {
            var k = new BEncodeString(key, keyEncoding);
            return TryGetValue(k, out value);
        }

        /// <summary>
        /// 获取与指定键关联的元素的值。
        /// </summary>
        /// <param name="key">元素的键。</param>
        /// <param name="value">元素的值。</param>
        /// <returns></returns>
        /// <exception cref="Exception"/>
        public bool TryGetValue(BEncodeString key, out BEncodeString value)
        {
            if (_elements.TryGetValue(key, out BEncodeValue val))
            {
                value = (BEncodeString)val;
                return true;
            }
            else
            {
                value = null;
                return false;
            }
        }

        /// <summary>
        /// 获取与指定键关联的元素的值。比较元素的键时默认使用 Encoding.UTF8 编码。
        /// </summary>
        /// <param name="key">元素的键。</param>
        /// <param name="value">元素的值。</param>
        /// <returns></returns>
        /// <exception cref="Exception"/>
        public bool TryGetValue(string key, out BEncodeString value)
        {
            var k = new BEncodeString(key, Encoding.UTF8);
            return TryGetValue(k, out value);
        }

        /// <summary>
        /// 获取与指定键关联的元素的值。
        /// </summary>
        /// <param name="key">元素的键。</param>
        /// <param name="keyEncoding">比较元素的键时使用的字符编码。</param>
        /// <param name="value">元素的值。</param>
        /// <returns></returns>
        /// <exception cref="Exception"/>
        public bool TryGetValue(string key, Encoding keyEncoding, out BEncodeString value)
        {
            var k = new BEncodeString(key, keyEncoding);
            return TryGetValue(k, out value);
        }

        /// <summary>
        /// 获取与指定键关联的元素的值。
        /// </summary>
        /// <param name="key">元素的键。</param>
        /// <param name="value">元素的值。</param>
        /// <returns></returns>
        /// <exception cref="Exception"/>
        public bool TryGetValue(BEncodeString key, out BEncodeValue value)
        {
            return _elements.TryGetValue(key, out value);
        }

        /// <summary>
        /// 获取与指定键关联的元素的值。比较元素的键时默认使用 Encoding.UTF8 编码。
        /// </summary>
        /// <param name="key">元素的键。</param>
        /// <param name="value">元素的值。</param>
        /// <returns></returns>
        /// <exception cref="Exception"/>
        public bool TryGetValue(string key, out BEncodeValue value)
        {
            var k = new BEncodeString(key, Encoding.UTF8);
            return TryGetValue(k, out value);
        }

        /// <summary>
        /// 获取与指定键关联的元素的值。
        /// </summary>
        /// <param name="key">元素的键。</param>
        /// <param name="keyEncoding">比较元素的键时使用的字符编码。</param>
        /// <param name="value">元素的值。</param>
        /// <returns></returns>
        /// <exception cref="Exception"/>
        public bool TryGetValue(string key, Encoding keyEncoding, out BEncodeValue value)
        {
            var k = new BEncodeString(key, keyEncoding);
            return TryGetValue(k, out value);
        }

        #endregion TryGetValue

        #region GetValue

        /// <summary>
        /// 获取与指定键关联的元素的值。
        /// </summary>
        /// <param name="key">元素的键。</param>
        /// <returns></returns>
        /// <exception cref="Exception"/>
        public BEncodeValue GetValue(BEncodeString key)
        {
            return TryGetValue(key, out BEncodeValue value) ? value : null;
        }

        /// <summary>
        /// 获取与指定键关联的元素的值。
        /// </summary>
        /// <param name="key">元素的键。</param>
        /// <returns></returns>
        /// <exception cref="Exception"/>
        public BEncodeValue GetValue(string key)
        {
            return TryGetValue(key, out BEncodeValue value) ? value : null;
        }

        /// <summary>
        /// 获取与指定键关联的元素的值。
        /// </summary>
        /// <param name="key">元素的键。</param>
        /// <param name="keyEncoding">比较元素的键时使用的字符编码。</param>
        /// <returns></returns>
        /// <exception cref="Exception"/>
        public BEncodeValue GetValue(string key, Encoding keyEncoding)
        {
            return TryGetValue(key, keyEncoding, out BEncodeValue value) ? value : null;
        }

        #endregion GetValue

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
        public bool ContainsKey(BEncodeString key)
        {
            return _elements.ContainsKey(key);
        }

        /// <summary>
        /// 确定元素集合是否包含带有指定键的元素。比较元素的键时默认使用 Encoding.UTF8 编码。
        /// </summary>
        /// <param name="key">元素的键。</param>
        /// <returns></returns>
        /// <exception cref="Exception"/>
        public bool ContainsKey(string key)
        {
            var k = new BEncodeString(key, Encoding.UTF8);
            return _elements.ContainsKey(k);
        }

        /// <summary>
        /// 确定元素集合是否包含带有指定键的元素。
        /// </summary>
        /// <param name="key">元素的键。</param>
        /// <param name="keyEncoding">比较元素的键时使用的字符编码。</param>
        /// <returns></returns>
        /// <exception cref="Exception"/>
        public bool ContainsKey(string key, Encoding keyEncoding)
        {
            var k = new BEncodeString(key, keyEncoding);
            return _elements.ContainsKey(k);
        }

        /// <summary>
        /// 支持在泛型集合上进行简单迭代。
        /// </summary>
        /// <returns></returns>
        public IEnumerator<KeyValuePair<BEncodeString, BEncodeValue>> GetEnumerator()
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
        public bool Remove(BEncodeString key)
        {
            return _elements.Remove(key);
        }

        /// <summary>
        /// 从元素集合中移除带有指定键的元素。比较元素的键时默认使用 Encoding.UTF8 编码。
        /// <para/>如果该元素成功移除，返回 true。如果没有找到指定键，则仍返回 false。
        /// </summary>
        /// <param name="key">元素的键。</param>
        /// <returns></returns>
        /// <exception cref="Exception"/>
        public bool Remove(string key)
        {
            var k = new BEncodeString(key, Encoding.UTF8);
            return _elements.Remove(k);
        }

        /// <summary>
        /// 从元素集合中移除带有指定键的元素。
        /// <para/>如果该元素成功移除，返回 true。如果没有找到指定键，则仍返回 false。
        /// </summary>
        /// <param name="key">元素的键。</param>
        /// <param name="keyEncoding">比较元素的键时使用的字符编码。</param>
        /// <returns></returns>
        /// <exception cref="Exception"/>
        public bool Remove(string key, Encoding keyEncoding)
        {
            var k = new BEncodeString(key, keyEncoding);
            return _elements.Remove(k);
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
            stream.WriteByte(100);  // 'd'
            var enumerator = _elements.GetEnumerator();
            while (enumerator.MoveNext())
            {
                enumerator.Current.Key.Save(stream);
                enumerator.Current.Value.Save(stream);
            }
            stream.WriteByte(101);  // 'e'
        }
    }
}