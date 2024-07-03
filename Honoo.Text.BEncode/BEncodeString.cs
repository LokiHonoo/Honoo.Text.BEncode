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
    public class BEncodeString : BEncodeValue, IEquatable<BEncodeString>, IComparer<BEncodeString>, IComparable
    {
        private readonly string _hexValue;
#if DEBUG
        private readonly string _utf8Value;
        private readonly byte[] _value;
#endif

        /// <summary>
        /// 获取原始格式的数据值。
        /// </summary>
        public byte[] Value
        {
            get
            {
                return (byte[])_value.Clone();
            }
        }

        #region Construction

        /// <summary>
        /// 初始化 BEncodeString 类的新实例。
        /// </summary>
        /// <param name="value">字节数据类型的值。</param>
        /// <exception cref="Exception"></exception>
        public BEncodeString(IList<byte> value) : base(BEncodeValueKind.BEncodeString)
        {
            if (value == null)
            {
                throw new ArgumentNullException(nameof(value));
            }
            _value = new byte[value.Count];
            value.CopyTo(_value, 0);
            _hexValue = BitConverter.ToString(_value).Replace("-", null);
#if DEBUG
            _utf8Value = Encoding.UTF8.GetString(_value);
#endif
        }

        /// <summary>
        /// 初始化 BEncodeString 类的新实例。默认使用 Encoding.UTF8 编码。
        /// </summary>
        /// <param name="value">文本类型的值。</param>
        /// <exception cref="Exception"></exception>
        public BEncodeString(string value) : this(value, Encoding.UTF8)
        {
        }

        /// <summary>
        /// 初始化 BEncodeString 类的新实例。
        /// </summary>
        /// <param name="value">文本类型的值。</param>
        /// <param name="encoding">用于转换的字符编码。</param>
        /// <exception cref="Exception"></exception>
        public BEncodeString(string value, Encoding encoding) : base(BEncodeValueKind.BEncodeString)
        {
            if (value == null)
            {
                throw new ArgumentNullException(nameof(value));
            }
            if (encoding == null)
            {
                throw new ArgumentNullException(nameof(encoding));
            }
            _value = encoding.GetBytes(value);
            _hexValue = BitConverter.ToString(_value).Replace("-", null);
#if DEBUG
            _utf8Value = Encoding.UTF8.GetString(_value);
#endif
        }

        /// <summary>
        /// 初始化 BEncodeString 类的新实例。
        /// </summary>
        /// <param name="content">指定从中读取的流。</param>
        /// <exception cref="Exception"></exception>
        public BEncodeString(Stream content) : base(BEncodeValueKind.BEncodeString)
        {
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
            _value = new byte[valueLen];
            content.Read(_value, 0, _value.Length);
            _hexValue = BitConverter.ToString(_value).Replace("-", null);
#if DEBUG
            _utf8Value = Encoding.UTF8.GetString(_value);
#endif
        }

        #endregion Construction

        /// <summary>
        /// 比较两个对象并返回一个值。该值指示一个对象是小于、等于还是大于另一个对象。
        /// </summary>
        /// <param name="x">要比较的第一个对象。</param>
        /// <param name="y">要比较的第二个对象。</param>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>
        public static int Compare(BEncodeString x, BEncodeString y)
        {
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
        /// <exception cref="Exception"></exception>
        int IComparer<BEncodeString>.Compare(BEncodeString x, BEncodeString y)
        {
            return string.Compare(x._hexValue, y._hexValue, StringComparison.Ordinal);
        }

        /// <summary>
        /// 将当前实例与另一个对象比较并返回一个值。该值指示当前实例在排序位置是小于、等于还是大于另一个对象。
        /// </summary>
        /// <param name="obj">要比较的对象。</param>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>
        public int CompareTo(object obj)
        {
            return string.Compare(_hexValue, (obj as BEncodeString)._hexValue, StringComparison.Ordinal);
        }

        /// <summary>
        /// 将当前实例与另一个对象比较并返回一个值。该值指示当前实例在排序位置是小于、等于还是大于另一个对象。
        /// </summary>
        /// <param name="obj">要比较的对象。</param>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>
        public int CompareTo(BEncodeString other)
        {
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
            return -1939223833 + EqualityComparer<byte[]>.Default.GetHashCode(_value);
        }

        /// <summary>
        /// 获取转换为十六进制字符串格式的数据值。
        /// </summary>
        /// <returns></returns>
        public string GetHexValue()
        {
            return _hexValue;
        }

        /// <summary>
        /// 获取转换为 String 格式的数据值。默认使用 Encoding.UTF8 编码。
        /// </summary>
        /// <returns></returns>
        public string GetStringValue()
        {
            return Encoding.UTF8.GetString(_value);
        }

        /// <summary>
        /// 获取转换为 String 格式的数据值。
        /// </summary>
        /// <param name="encoding">用于转换的字符编码。</param>
        /// <returns></returns>
        public string GetStringValue(Encoding encoding)
        {
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
        /// 方法已重写。获取转换为十六进制字符串格式的数据值。
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return _hexValue;
        }
    }
}