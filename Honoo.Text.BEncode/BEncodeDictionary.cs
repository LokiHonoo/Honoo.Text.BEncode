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
    public class BEncodeDictionary : BEncodeElement, IEnumerable<KeyValuePair<BEncodeString, BEncodeElement>>, IReadOnlyBEncodeDictionary
    {
        #region Class

        /// <summary>
        /// 代表此元素集合的键的集合。
        /// </summary>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Design", "CA1034:嵌套类型应不可见", Justification = "<挂起>")]
        public sealed class KeyCollection : IEnumerable<BEncodeString>
        {
            #region Properties

            private readonly Dictionary<BEncodeString, BEncodeElement> _elements;

            /// <summary>
            /// 获取元素集合的键的元素数。
            /// </summary>
            public int Count => _elements.Count;

            #endregion Properties

            internal KeyCollection(Dictionary<BEncodeString, BEncodeElement> elements)
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
            /// 从指定数组索引开始将键元素的文本值复制到到指定数组。转换元素的键时默认使用 <see cref="Encoding.UTF8"/> 编码。
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
            /// <param name="keyEncoding">用于转换元素的键的字符编码。</param>
            /// <exception cref="Exception"/>
            public void CopyTo(string[] array, int arrayIndex, Encoding keyEncoding)
            {
                if (array == null)
                {
                    throw new ArgumentNullException(nameof(array));
                }
                foreach (var key in _elements.Keys)
                {
                    array[arrayIndex] = key.GetStringValue(keyEncoding);
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
        public sealed class ValueCollection : IEnumerable<BEncodeElement>
        {
            #region Properties

            private readonly Dictionary<BEncodeString, BEncodeElement> _elements;

            /// <summary>
            /// 获取元素集合的值的元素数。
            /// </summary>
            public int Count => _elements.Count;

            #endregion Properties

            internal ValueCollection(Dictionary<BEncodeString, BEncodeElement> elements)
            {
                _elements = elements;
            }

            /// <summary>
            /// 从指定数组索引开始将值元素复制到到指定数组。
            /// </summary>
            /// <typeparam name="T">指定元素类型。</typeparam>
            /// <param name="array">目标数组。</param>
            /// <param name="arrayIndex">目标数组中从零开始的索引，从此处开始复制。</param>
            public void CopyTo<T>(T[] array, int arrayIndex) where T : BEncodeElement
            {
                _elements.Values.CopyTo(array, arrayIndex);
            }

            /// <summary>
            /// 支持在泛型集合上进行简单迭代。
            /// </summary>
            /// <returns></returns>
            public IEnumerator<BEncodeElement> GetEnumerator()
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

        private readonly Dictionary<BEncodeString, BEncodeElement> _elements = new Dictionary<BEncodeString, BEncodeElement>();
        private readonly KeyCollection _keyExhibits;
        private readonly ValueCollection _valueExhibits;
        private bool _isReadOnly;

        /// <summary>
        /// 获取元素集合中包含的元素数。
        /// </summary>
        public int Count => _elements.Count;

        /// <summary>
        /// 获取一个值，该值指示此 <see cref="BEncodeDictionary"/> 是否为只读。
        /// </summary>
        public bool IsReadOnly => _isReadOnly;

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
        public BEncodeElement this[BEncodeString key]
        {
            get { return GetValue<BEncodeElement>(key); }
            set { AddOrUpdate(key, value); }
        }

        /// <summary>
        /// 获取或设置具有指定键的元素的值。转换元素的键时默认使用 <see cref="Encoding.UTF8"/> 编码。直接赋值等同于 AddOrUpdate 方法。
        /// </summary>
        /// <param name="key">元素的键。</param>
        /// <returns></returns>
        /// <exception cref="Exception"/>
        public BEncodeElement this[string key]
        {
            get { return GetValue<BEncodeElement>(new BEncodeString(key, Encoding.UTF8)); }
            set { AddOrUpdate(key, value); }
        }

        /// <summary>
        /// 获取或设置具有指定键的元素的值。直接赋值等同于 AddOrUpdate 方法。
        /// </summary>
        /// <param name="key">元素的键。</param>
        /// <param name="keyEncoding">用于转换元素的键的字符编码。</param>
        /// <returns></returns>
        /// <exception cref="Exception"/>
        public BEncodeElement this[string key, Encoding keyEncoding]
        {
            get { return GetValue<BEncodeElement>(new BEncodeString(key, keyEncoding)); }
            set { AddOrUpdate(key, value); }
        }

        #endregion Properties

        #region Construction

        /// <summary>
        /// 初始化 BEncodeDictionary 类的新实例。
        /// </summary>
        public BEncodeDictionary() : base(BEncodeElementKind.BEncodeDictionary)
        {
            _keyExhibits = new KeyCollection(_elements);
            _valueExhibits = new ValueCollection(_elements);
        }

        /// <summary>
        /// 初始化 BEncodeDictionary 类的新实例。
        /// </summary>
        /// <param name="content">指定从中读取的流。定位必须在编码标记 <see langword="d"/> 处。</param>
        /// <exception cref="Exception"/>
        public BEncodeDictionary(Stream content) : this(content, false)
        {
        }

        /// <summary>
        /// 初始化 BEncodeDictionary 类的新实例。
        /// </summary>
        /// <param name="content">指定从中读取的流。定位必须在编码标记 <see langword="d"/> 处。</param>
        /// <param name="readOnly">指定此 <see cref="BEncodeDictionary"/> 及子元素是只读的。</param>
        /// <exception cref="Exception"/>
        public BEncodeDictionary(Stream content, bool readOnly) : base(BEncodeElementKind.BEncodeDictionary)
        {
            if (content == null)
            {
                throw new ArgumentNullException(nameof(content));
            }
            int kc = content.ReadByte();
            if (kc != 100)  // "d"
            {
                throw new ArgumentException($"The header char is not a dictionary identification char \"d\". Stop at position: {content.Position}.");
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
                    content.Seek(-1, SeekOrigin.Current);
                    var key = new BEncodeString(content, readOnly);
                    kc = content.ReadByte();
                    switch (kc)
                    {
                        case 100: content.Seek(-1, SeekOrigin.Current); _elements.Add(key, new BEncodeDictionary(content, readOnly)); break;// "d"
                        case 108: content.Seek(-1, SeekOrigin.Current); _elements.Add(key, new BEncodeList(content, readOnly)); break;      // "l"
                        case 105: content.Seek(-1, SeekOrigin.Current); _elements.Add(key, new BEncodeInteger(content, readOnly)); break;   // "i"
                        case 48:                                                                                                            // "0"
                        case 49:                                                                                                            // "1"
                        case 50:                                                                                                            // "2"
                        case 51:                                                                                                            // "3"
                        case 52:                                                                                                            // "4"
                        case 53:                                                                                                            // "5"
                        case 54:                                                                                                            // "6"
                        case 55:                                                                                                            // "7"
                        case 56:                                                                                                            // "8"
                        case 57: content.Seek(-1, SeekOrigin.Current); _elements.Add(key, new BEncodeString(content, readOnly)); break;     // "9"
                        default: throw new ArgumentException($"The incorrect identification char \"{kc}\". Stop at position: {content.Position}.");
                    }
                    kc = content.ReadByte();
                }
            }
            _keyExhibits = new KeyCollection(_elements);
            _valueExhibits = new ValueCollection(_elements);
            _isReadOnly = readOnly;
        }

        #endregion Construction

        #region Add

        /// <summary>
        /// 添加一个元素。
        /// </summary>
        /// <typeparam name="T">指定元素类型。</typeparam>
        /// <param name="key">元素的键。</param>
        /// <param name="value">元素的值。</param>
        /// <exception cref="Exception"/>
        public T Add<T>(BEncodeString key, T value) where T : BEncodeElement
        {
            if (_isReadOnly)
            {
                throw new NotSupportedException("Collection is read-only.");
            }
            if (key == null)
            {
                throw new ArgumentNullException(nameof(key));
            }
            if (value == null)
            {
                throw new ArgumentNullException(nameof(value));
            }
            key.ChangeReadOnly(false);
            value.ChangeReadOnly(false);
            _elements.Add(key, value);
            return value;
        }

        /// <summary>
        /// 添加一个元素。转换元素的键时默认使用 <see cref="Encoding.UTF8"/> 编码。
        /// </summary>
        /// <typeparam name="T">指定元素类型。</typeparam>
        /// <param name="key">元素的键。</param>
        /// <param name="value">元素的值。</param>
        /// <exception cref="Exception"/>
        public T Add<T>(string key, T value) where T : BEncodeElement
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
        /// <typeparam name="T">指定元素类型。</typeparam>
        /// <param name="key">元素的键。</param>
        /// <param name="keyEncoding">用于转换元素的键的字符编码。</param>
        /// <param name="value">元素的值。</param>
        /// <exception cref="Exception"/>
        public T Add<T>(string key, Encoding keyEncoding, T value) where T : BEncodeElement
        {
            if (string.IsNullOrWhiteSpace(key))
            {
                throw new ArgumentNullException(nameof(key));
            }
            var k = new BEncodeString(key, keyEncoding);
            return Add(k, value);
        }

        #endregion Add

        #region GetOrAdd

        /// <summary>
        /// 获取与指定名称关联的元素。如果不存在，添加一个 <typeparamref name="T"/> 类型的元素并返回值。
        /// <br/>如果元素存在但不是指定的类型，方法抛出 <see cref="ArgumentException"/>。
        /// </summary>
        /// <typeparam name="T">指定元素类型。</typeparam>
        /// <param name="key">元素的键。</param>
        /// <param name="valueIfNotExists">指定名称关联的元素不存在时添加此元素。</param>
        /// <returns></returns>
        /// <exception cref="Exception"/>
        public T GetOrAdd<T>(BEncodeString key, T valueIfNotExists) where T : BEncodeElement
        {
            if (TryGetValue(key, out BEncodeElement value))
            {
                if (value is T val)
                {
                    return val;
                }
                else
                {
                    throw new ArgumentException($"The section exists but is not of the specified type - {typeof(T)}.");
                }
            }
            else
            {
                return Add(key, valueIfNotExists);
            }
        }

        /// <summary>
        /// 获取与指定名称关联的元素。如果不存在，添加一个 <typeparamref name="T"/> 类型的元素并返回值。转换元素的键时默认使用 <see cref="Encoding.UTF8"/> 编码。
        /// <br/>如果元素存在但不是指定的类型，方法抛出 <see cref="ArgumentException"/>。
        /// </summary>
        /// <typeparam name="T">指定元素类型。</typeparam>
        /// <param name="key">元素的键。</param>
        /// <param name="valueIfNotExists">指定名称关联的元素不存在时添加此元素。</param>
        /// <returns></returns>
        /// <exception cref="Exception"/>
        public T GetOrAdd<T>(string key, T valueIfNotExists) where T : BEncodeElement
        {
            if (key is null)
            {
                throw new ArgumentNullException(nameof(key));
            }
            var k = new BEncodeString(key, Encoding.UTF8);
            return GetOrAdd(k, valueIfNotExists);
        }

        /// <summary>
        /// 获取与指定名称关联的元素。如果不存在，添加一个 <typeparamref name="T"/> 类型的元素并返回值。
        /// <br/>如果元素存在但不是指定的类型，方法抛出 <see cref="ArgumentException"/>。
        /// </summary>
        /// <typeparam name="T">指定元素类型。</typeparam>
        /// <param name="key">元素的键。</param>
        /// <param name="keyEncoding">用于转换元素的键的字符编码。</param>
        /// <param name="valueIfNotExists">指定名称关联的元素不存在时添加此元素。</param>
        /// <returns></returns>
        /// <exception cref="Exception"/>
        public T GetOrAdd<T>(string key, Encoding keyEncoding, T valueIfNotExists) where T : BEncodeElement
        {
            if (key is null)
            {
                throw new ArgumentNullException(nameof(key));
            }
            var k = new BEncodeString(key, keyEncoding);
            return GetOrAdd(k, valueIfNotExists);
        }

        #endregion GetOrAdd

        #region AddOrUpdate

        /// <summary>
        /// 添加或更新一个元素。
        /// </summary>
        /// <typeparam name="T">指定元素类型。</typeparam>
        /// <param name="key">元素的键。</param>
        /// <param name="value">元素的值。</param>
        /// <exception cref="Exception"/>
        public T AddOrUpdate<T>(BEncodeString key, T value) where T : BEncodeElement
        {
            if (_isReadOnly)
            {
                throw new NotSupportedException("Collection is read-only.");
            }
            if (key == null)
            {
                throw new ArgumentNullException(nameof(key));
            }
            if (value == null)
            {
                throw new ArgumentNullException(nameof(value));
            }
            if (ContainsKey(key))
            {
                _elements[key] = value;
            }
            else
            {
                Add(key, value);
            }
            return value;
        }

        /// <summary>
        /// 添加或更新一个元素。转换元素的键时默认使用 <see cref="Encoding.UTF8"/> 编码。
        /// </summary>
        /// <typeparam name="T">指定元素类型。</typeparam>
        /// <param name="key">元素的键。</param>
        /// <param name="value">元素的值。</param>
        /// <returns></returns>
        /// <exception cref="Exception"/>
        public T AddOrUpdate<T>(string key, T value) where T : BEncodeElement
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
        /// <typeparam name="T">指定元素类型。</typeparam>
        /// <param name="key">元素的键。</param>
        /// <param name="keyEncoding">用于转换元素的键的字符编码。</param>
        /// <param name="value">元素的值。</param>
        /// <returns></returns>
        /// <exception cref="Exception"/>
        public T AddOrUpdate<T>(string key, Encoding keyEncoding, T value) where T : BEncodeElement
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
        /// <br/>如果没有找到指定键，返回 <see langword="false"/>。如果找到了指定键但指定的类型不符，则仍返回 <see langword="false"/>。
        /// </summary>
        /// <typeparam name="T">指定元素类型。</typeparam>
        /// <param name="key">元素的键。</param>
        /// <param name="value">元素的值。</param>
        /// <returns></returns>
        public bool TryGetValue<T>(BEncodeString key, out T value) where T : BEncodeElement
        {
            if (_elements.TryGetValue(key, out BEncodeElement val))
            {
                if (val is T v)
                {
                    value = v;
                    return true;
                }
            }
            value = null;
            return false;
        }

        /// <summary>
        /// 获取与指定键关联的元素的值。转换元素的键时默认使用 <see cref="Encoding.UTF8"/> 编码。
        /// <br/>如果没有找到指定键，返回 <see langword="false"/>。如果找到了指定键但指定的类型不符，则仍返回 <see langword="false"/>。
        /// </summary>
        /// <typeparam name="T">指定元素类型。</typeparam>
        /// <param name="key">元素的键。</param>
        /// <param name="value">元素的值。</param>
        /// <returns></returns>
        /// <exception cref="Exception"/>
        public bool TryGetValue<T>(string key, out T value) where T : BEncodeElement
        {
            var k = new BEncodeString(key, Encoding.UTF8);
            return TryGetValue(k, out value);
        }

        /// <summary>
        /// 获取与指定键关联的元素的值。
        /// <br/>如果没有找到指定键，返回 <see langword="false"/>。如果找到了指定键但指定的类型不符，则仍返回 <see langword="false"/>。
        /// </summary>
        /// <typeparam name="T">指定元素类型。</typeparam>
        /// <param name="key">元素的键。</param>
        /// <param name="keyEncoding">用于转换元素的键的字符编码。</param>
        /// <param name="value">元素的值。</param>
        /// <returns></returns>
        /// <exception cref="Exception"/>
        public bool TryGetValue<T>(string key, Encoding keyEncoding, out T value) where T : BEncodeElement
        {
            var k = new BEncodeString(key, keyEncoding);
            return TryGetValue(k, out value);
        }

        #endregion TryGetValue

        #region GetValue

        /// <summary>
        /// 获取与指定键关联的元素的值。如果没有找到指定键或者无法转换指定的类型，返回 <see langword="null"/>。
        /// </summary>
        /// <typeparam name="T">指定元素类型。</typeparam>
        /// <param name="key">元素的键。</param>
        /// <returns></returns>
        /// <exception cref="Exception"/>
        public T GetValue<T>(BEncodeString key) where T : BEncodeElement
        {
            return (T)_elements[key];
        }

        /// <summary>
        /// 获取与指定键关联的元素的值。如果没有找到指定键或者无法转换指定的类型，返回 <see langword="null"/>。转换元素的键时默认使用 <see cref="Encoding.UTF8"/> 编码。
        /// </summary>
        /// <typeparam name="T">指定元素类型。</typeparam>
        /// <param name="key">元素的键。</param>
        /// <returns></returns>
        /// <exception cref="Exception"/>
        public T GetValue<T>(string key) where T : BEncodeElement
        {
            var k = new BEncodeString(key, Encoding.UTF8);
            return GetValue<T>(k);
        }

        /// <summary>
        /// 获取与指定键关联的元素的值。如果没有找到指定键或者无法转换指定的类型，返回 <see langword="null"/>。
        /// </summary>
        /// <typeparam name="T">指定元素类型。</typeparam>
        /// <param name="key">元素的键。</param>
        /// <param name="keyEncoding">用于转换元素的键的字符编码。</param>
        /// <returns></returns>
        /// <exception cref="Exception"/>
        public T GetValue<T>(string key, Encoding keyEncoding) where T : BEncodeElement
        {
            var k = new BEncodeString(key, keyEncoding);
            return GetValue<T>(k);
        }

        #endregion GetValue

        #region GetValueOrDefault

        /// <summary>
        /// 获取与指定键关联的元素的值。如果没有找到指定键或者无法转换指定的类型，返回 <paramref name="defaultValue"/>。
        /// </summary>
        /// <typeparam name="T">指定元素类型。</typeparam>
        /// <param name="key">元素的键。</param>
        /// <param name="defaultValue">没有找到指定键时的元素的默认值。</param>
        /// <returns></returns>
        public T GetValue<T>(BEncodeString key, T defaultValue) where T : BEncodeElement
        {
            return TryGetValue(key, out T value) ? value : defaultValue;
        }

        /// <summary>
        /// 获取与指定键关联的元素的值。如果没有找到指定键或者无法转换指定的类型，返回 <paramref name="defaultValue"/>。转换元素的键时默认使用 <see cref="Encoding.UTF8"/> 编码。
        /// </summary>
        /// <typeparam name="T">指定元素类型。</typeparam>
        /// <param name="key">元素的键。</param>
        /// <param name="defaultValue">没有找到指定键时的元素的默认值。</param>
        /// <returns></returns>
        /// <exception cref="Exception"/>
        public T GetValue<T>(string key, T defaultValue) where T : BEncodeElement
        {
            var k = new BEncodeString(key, Encoding.UTF8);
            return GetValue<T>(k, defaultValue);
        }

        /// <summary>
        /// 获取与指定键关联的元素的值。如果没有找到指定键或者无法转换指定的类型，返回 <paramref name="defaultValue"/>。
        /// </summary>
        /// <typeparam name="T">指定元素类型。</typeparam>
        /// <param name="key">元素的键。</param>
        /// <param name="keyEncoding">用于转换元素的键的字符编码。</param>
        /// <param name="defaultValue">没有找到指定键时的元素的默认值。</param>
        /// <returns></returns>
        /// <exception cref="Exception"/>
        public T GetValue<T>(string key, Encoding keyEncoding, T defaultValue) where T : BEncodeElement
        {
            var k = new BEncodeString(key, keyEncoding);
            return GetValue<T>(k, defaultValue);
        }

        #endregion GetValueOrDefault

        /// <summary>
        /// 获取此实例的只读接口。
        /// </summary>
        public IReadOnlyBEncodeDictionary AsReadOnly()
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
        /// 确定元素集合是否包含带有指定键的元素。
        /// </summary>
        /// <param name="key">元素的键。</param>
        /// <returns></returns>
        public bool ContainsKey(BEncodeString key)
        {
            return _elements.ContainsKey(key);
        }

        /// <summary>
        /// 确定元素集合是否包含带有指定键的元素。转换元素的键时默认使用 <see cref="Encoding.UTF8"/> 编码。
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
        /// <param name="keyEncoding">用于转换元素的键的字符编码。</param>
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
        public IEnumerator<KeyValuePair<BEncodeString, BEncodeElement>> GetEnumerator()
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
            if (_isReadOnly)
            {
                throw new NotSupportedException("Collection is read-only.");
            }
            return _elements.Remove(key);
        }

        /// <summary>
        /// 从元素集合中移除带有指定键的元素。转换元素的键时默认使用 <see cref="Encoding.UTF8"/> 编码。
        /// <para/>如果该元素成功移除，返回 true。如果没有找到指定键，则仍返回 false。
        /// </summary>
        /// <param name="key">元素的键。</param>
        /// <returns></returns>
        /// <exception cref="Exception"/>
        public bool Remove(string key)
        {
            var k = new BEncodeString(key, Encoding.UTF8);
            return Remove(k);
        }

        /// <summary>
        /// 从元素集合中移除带有指定键的元素。
        /// <para/>如果该元素成功移除，返回 true。如果没有找到指定键，则仍返回 false。
        /// </summary>
        /// <param name="key">元素的键。</param>
        /// <param name="keyEncoding">用于转换元素的键的字符编码。</param>
        /// <returns></returns>
        /// <exception cref="Exception"/>
        public bool Remove(string key, Encoding keyEncoding)
        {
            var k = new BEncodeString(key, keyEncoding);
            return Remove(k);
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
            stream.WriteByte(100);  // "d"
            var enumerator = _elements.GetEnumerator();
            while (enumerator.MoveNext())
            {
                enumerator.Current.Key.Save(stream);
                enumerator.Current.Value.Save(stream);
            }
            stream.WriteByte(101);  // "e"
        }

        internal override void ChangeReadOnly(bool isReadOnly)
        {
            var enumerator = _elements.GetEnumerator();
            while (enumerator.MoveNext())
            {
                enumerator.Current.Value.ChangeReadOnly(isReadOnly);
            }
            _isReadOnly = isReadOnly;
        }
    }
}