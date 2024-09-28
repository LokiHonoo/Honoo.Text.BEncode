using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Text;

namespace Honoo.Text.BEncode
{
    /// <summary>
    /// BEncode 串行数据类型。
    /// </summary>
    public class BEncodeString : BEncodeElement, IEquatable<BEncodeString>, IComparer<BEncodeString>, IComparable
    {
        #region DEBUG

#if DEBUG
        private string _utf8;

        internal string Utf8 => _utf8;
#endif

        #endregion DEBUG

        private string _hexValue;
        private bool _isReadOnly;
        private byte[] _value;

        /// <summary>
        /// 获取一个值，该值指示 <see cref="BEncodeString"/> 是否为只读。
        /// </summary>
        public bool IsReadOnly => _isReadOnly;

        /// <summary>
        /// 获取原始格式的数据值。
        /// </summary>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Performance", "CA1819:属性不应返回数组", Justification = "<挂起>")]
        public byte[] Value => (byte[])_value.Clone();

        #region Construction

        /// <summary>
        /// 初始化 BEncodeString 类的新实例。
        /// </summary>
        /// <param name="value">字节数据类型的值。</param>
        /// <exception cref="Exception"/>
        public BEncodeString(byte[] value) : base(BEncodeElementKind.BEncodeString)
        {
            if (value == null)
            {
                throw new ArgumentNullException(nameof(value));
            }
            _value = (byte[])value.Clone();
            _hexValue = BitConverter.ToString(value).Replace("-", null);
#if DEBUG
            _utf8 = Encoding.UTF8.GetString(value);
#endif
        }

        /// <summary>
        /// 初始化 BEncodeString 类的新实例。转换时默认使用 <see cref="Encoding.UTF8"/> 编码。
        /// </summary>
        /// <param name="value">文本类型的值。</param>
        /// <exception cref="Exception"/>
        public BEncodeString(string value) : this(value, Encoding.UTF8)
        {
        }

        /// <summary>
        /// 初始化 BEncodeString 类的新实例。
        /// </summary>
        /// <param name="value">文本类型的值。</param>
        /// <param name="encoding">用于转换的字符编码。</param>
        /// <exception cref="Exception"/>
        public BEncodeString(string value, Encoding encoding) : base(BEncodeElementKind.BEncodeString)
        {
            if (value == null)
            {
                throw new ArgumentNullException(nameof(value));
            }
            if (encoding == null)
            {
                throw new ArgumentNullException(nameof(encoding));
            }
            byte[] bytes = encoding.GetBytes(value);
            _value = bytes;
            _hexValue = BitConverter.ToString(bytes).Replace("-", null);
#if DEBUG
            _utf8 = value;
#endif
        }

        /// <summary>
        /// 初始化 BEncodeString 类的新实例。
        /// </summary>
        /// <param name="content">指定从中读取的流。定位必须在编码标记 <see langword="0"/>-<see langword="9"/> 处。</param>
        /// <param name="readOnly">指定此 <see cref="BEncodeString"/> 及子元素是只读的。</param>
        /// <exception cref="Exception"/>
        public BEncodeString(Stream content, bool readOnly) : base(BEncodeElementKind.BEncodeString)
        {
            if (content == null)
            {
                throw new ArgumentNullException(nameof(content));
            }
            var lenString = new StringBuilder();
            int kc = content.ReadByte();
            if (kc < 48 || kc > 57)  // '0'-'9'
            {
                throw new ArgumentException($"The header char is not a single identification char '0'-'9'. Stop at position: {content.Position}.");
            }
            while (true)
            {
                if (kc == 58)  // ':'
                {
                    break;
                }
                else
                {
                    lenString.Append((char)kc);
                    kc = content.ReadByte();
                }
            }
            int valueLen = int.Parse(lenString.ToString(), CultureInfo.InvariantCulture);
            byte[] bytes = new byte[valueLen];
            content.Read(bytes, 0, bytes.Length);
            _value = bytes;
            _hexValue = BitConverter.ToString(bytes).Replace("-", null);
#if DEBUG
            _utf8 = Encoding.UTF8.GetString(bytes);
#endif
            _isReadOnly = readOnly;
        }

        #endregion Construction

        /// <summary>
        /// 比较两个对象并返回一个值。该值指示一个对象是小于、等于还是大于另一个对象。
        /// </summary>
        /// <param name="x">要比较的第一个对象。</param>
        /// <param name="y">要比较的第二个对象。</param>
        /// <returns></returns>
        /// <exception cref="Exception"/>
        public static int Compare(BEncodeString x, BEncodeString y)
        {
            if (x is null)
            {
                throw new ArgumentNullException(nameof(x));
            }
            if (y is null)
            {
                throw new ArgumentNullException(nameof(y));
            }
            return string.Compare(x._hexValue, y._hexValue, StringComparison.Ordinal);
        }

        /// <summary>
        /// 比较。
        /// </summary>
        /// <param name="left"></param>
        /// <param name="right"></param>
        /// <returns></returns>
        public static bool operator !=(BEncodeString left, BEncodeString right)
        {
            return !(left == right);
        }

        /// <summary>
        /// 比较。
        /// </summary>
        /// <param name="left"></param>
        /// <param name="right"></param>
        /// <returns></returns>
        public static bool operator <(BEncodeString left, BEncodeString right)
        {
            return (Compare(left, right) < 0);
        }

        /// <summary>
        /// 比较。
        /// </summary>
        /// <param name="left"></param>
        /// <param name="right"></param>
        /// <returns></returns>
        public static bool operator <=(BEncodeString left, BEncodeString right)
        {
            return (Compare(left, right) <= 0);
        }

        /// <summary>
        /// 比较。
        /// </summary>
        /// <param name="left"></param>
        /// <param name="right"></param>
        /// <returns></returns>
        public static bool operator ==(BEncodeString left, BEncodeString right)
        {
            if (left is null)
            {
                return right is null;
            }
            return left.Equals(right);
        }

        /// <summary>
        /// 比较。
        /// </summary>
        /// <param name="left"></param>
        /// <param name="right"></param>
        /// <returns></returns>
        public static bool operator >(BEncodeString left, BEncodeString right)
        {
            return (Compare(left, right) > 0);
        }

        /// <summary>
        /// 比较。
        /// </summary>
        /// <param name="left"></param>
        /// <param name="right"></param>
        /// <returns></returns>
        public static bool operator >=(BEncodeString left, BEncodeString right)
        {
            return (Compare(left, right) >= 0);
        }

        /// <summary>
        /// 比较两个对象并返回一个值。该值指示一个对象是小于、等于还是大于另一个对象。
        /// </summary>
        /// <param name="x">要比较的第一个对象。</param>
        /// <param name="y">要比较的第二个对象。</param>
        /// <returns></returns>
        int IComparer<BEncodeString>.Compare(BEncodeString x, BEncodeString y)
        {
            return string.Compare(x._hexValue, y._hexValue, StringComparison.Ordinal);
        }

        /// <summary>
        /// 将当前实例与另一个对象比较并返回一个值。该值指示当前实例在排序位置是小于、等于还是大于另一个对象。
        /// </summary>
        /// <param name="obj">要比较的对象。</param>
        /// <returns></returns>
        /// <exception cref="Exception"/>
        public int CompareTo(object obj)
        {
            if (obj is BEncodeString other)
            {
                return CompareTo(other);
            }
            throw new ArgumentException($"{nameof(obj)} is not a BEncodeString.");
        }

        /// <summary>
        /// 将当前实例与另一个对象比较并返回一个值。该值指示当前实例在排序位置是小于、等于还是大于另一个对象。
        /// </summary>
        /// <param name="other">要比较的对象。</param>
        /// <returns></returns>
        /// <exception cref="Exception"/>
        public int CompareTo(BEncodeString other)
        {
            if (other is null)
            {
                throw new ArgumentNullException(nameof(other));
            }
            return string.Compare(_hexValue, other._hexValue, StringComparison.Ordinal);
        }

        /// <summary>
        /// 确定此实例和指定的对象具有相同的值。
        /// </summary>
        /// <param name="other">比较的对象。</param>
        /// <returns></returns>
        public bool Equals(BEncodeString other)
        {
            return other is BEncodeString && other._hexValue == _hexValue;
        }

        /// <summary>
        /// 确定指定的对象是否等于当前对象。
        /// </summary>
        /// <param name="obj">比较的对象。</param>
        /// <returns></returns>
        public override bool Equals(object obj)
        {
            return obj is BEncodeString other && other._hexValue == _hexValue;
        }

        /// <summary>
        ///
        /// </summary>
        /// <returns></returns>
        public override int GetHashCode()
        {
            return -1939223833 + EqualityComparer<string>.Default.GetHashCode(_hexValue);
        }

        /// <summary>
        /// 获取转换为十六进制字符串格式的数据值。
        /// </summary>
        /// <returns></returns>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Design", "CA1024:在适用处使用属性", Justification = "<挂起>")]
        public string GetHexValue()
        {
            return _hexValue;
        }

        /// <summary>
        /// 获取转换为 String 格式的数据值。转换时默认使用 <see cref="Encoding.UTF8"/> 编码。
        /// </summary>
        /// <returns></returns>
        /// <exception cref="Exception"/>
        public string GetStringValue()
        {
            return GetStringValue(Encoding.UTF8);
        }

        /// <summary>
        /// 获取转换为 String 格式的数据值。
        /// </summary>
        /// <param name="encoding">用于转换的字符编码。</param>
        /// <returns></returns>
        /// <exception cref="Exception"/>
        public string GetStringValue(Encoding encoding)
        {
            if (encoding is null)
            {
                throw new ArgumentNullException(nameof(encoding));
            }
            return encoding.GetString(_value);
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
            byte[] length = Encoding.ASCII.GetBytes(_value.Length.ToString(CultureInfo.InvariantCulture));
            stream.Write(length, 0, length.Length);
            stream.WriteByte(58);  // ':'
            stream.Write(_value, 0, _value.Length);
        }

        /// <summary>
        /// 设置值。
        /// </summary>
        /// <param name="value">字节数据类型的值。</param>
        /// <returns></returns>
        /// <exception cref="Exception"/>
        public BEncodeString SetValue(byte[] value)
        {
            if (_isReadOnly)
            {
                throw new NotSupportedException("Element is read-only.");
            }
            if (value == null)
            {
                throw new ArgumentNullException(nameof(value));
            }
            _value = (byte[])value.Clone();
            _hexValue = BitConverter.ToString(value).Replace("-", null);
#if DEBUG
            _utf8 = Encoding.UTF8.GetString(value);
#endif
            return this;
        }

        /// <summary>
        /// 设置值。转换时默认使用 <see cref="Encoding.UTF8"/> 编码。
        /// </summary>
        /// <param name="value">文本类型的值。</param>
        /// <returns></returns>
        /// <exception cref="Exception"/>
        public BEncodeString SetValue(string value)
        {
            return SetValue(value, Encoding.UTF8);
        }

        /// <summary>
        /// 设置值。
        /// </summary>
        /// <param name="value">文本类型的值。</param>
        /// <param name="encoding">用于转换的字符编码。</param>
        /// <returns></returns>
        /// <exception cref="Exception"/>
        public BEncodeString SetValue(string value, Encoding encoding)
        {
            if (_isReadOnly)
            {
                throw new NotSupportedException("Element is read-only.");
            }
            if (value == null)
            {
                throw new ArgumentNullException(nameof(value));
            }
            if (encoding == null)
            {
                throw new ArgumentNullException(nameof(encoding));
            }
            byte[] bytes = encoding.GetBytes(value);
            _value = bytes;
            _hexValue = BitConverter.ToString(bytes).Replace("-", null);
#if DEBUG
            _utf8 = value;
#endif
            return this;
        }

        /// <summary>
        /// 方法已重写。获取字符串表示形式的数据值。转换时默认使用 <see cref="Encoding.UTF8"/> 编码。
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return ToString(Encoding.UTF8);
        }

        /// <summary>
        /// 获取字符串表示形式的数据值。
        /// </summary>
        /// <param name="encoding">用于转换的字符编码。</param>
        /// <returns></returns>
        /// <exception cref="Exception"/>
        public string ToString(Encoding encoding)
        {
            if (encoding == null)
            {
                throw new ArgumentNullException(nameof(encoding));
            }
            return encoding.GetString(_value);
        }

        internal override void ChangeReadOnly(bool isReadOnly)
        {
            _isReadOnly = isReadOnly;
        }
    }
}