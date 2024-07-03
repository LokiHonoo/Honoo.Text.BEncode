using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Numerics;
using System.Text;

namespace Honoo.Text.BEncode
{
    /// <summary>
    /// BEncode 数值类型。
    /// </summary>
    public class BEncodeInteger : BEncodeValue, IEquatable<BEncodeInteger>, IComparer<BEncodeInteger>, IComparable
    {
        private readonly BigInteger _numericValue;
        private readonly string _value;

        /// <summary>
        /// 获取原始格式的数据值。
        /// </summary>
        public string Value => _value;

        internal BigInteger NumericValue => _numericValue;

        #region Construction

        /// <summary>
        /// 初始化 BEncodeInteger 类的新实例。
        /// </summary>
        /// <param name="value">十进制文本表示的长数值类型的值。</param>
        /// <exception cref="Exception"></exception>
        public BEncodeInteger(string value) : base(BEncodeValueKind.BEncodeInteger)
        {
            if (string.IsNullOrWhiteSpace(value))
            {
                throw new ArgumentNullException(nameof(value));
            }
            _value = value;
            _numericValue = BigInteger.Parse(value, NumberStyles.Number, CultureInfo.InvariantCulture);
        }

        /// <summary>
        /// 初始化 BEncodeInteger 类的新实例。
        /// </summary>
        /// <param name="value">Int64 类型的值。</param>
        /// <exception cref="Exception"></exception>
        public BEncodeInteger(long value) : base(BEncodeValueKind.BEncodeInteger)
        {
            _value = value.ToString(CultureInfo.InvariantCulture);
            _numericValue = value;
        }

        /// <summary>
        /// 初始化 BEncodeInteger 类的新实例。
        /// </summary>
        /// <param name="content">指定从中读取的流。</param>
        /// <exception cref="Exception"></exception>
        public BEncodeInteger(Stream content) : base(BEncodeValueKind.BEncodeInteger)
        {
            int kc = content.ReadByte();
            if (kc != 105)  // 'i'
            {
                throw new ArgumentException($"The header char is not a integer identification char 'i'. Stop at position: {content.Position}.");
            }
            var valueString = new StringBuilder();
            kc = content.ReadByte();
            while (true)
            {
                if (kc == 101)  // 'e'
                {
                    break;
                }
                else
                {
                    valueString.Append((char)kc);
                    kc = content.ReadByte();
                }
            }
            _value = valueString.ToString();
            _numericValue = BigInteger.Parse(_value, NumberStyles.Number, CultureInfo.InvariantCulture);
        }

        #endregion Construction

        /// <summary>
        /// 比较两个对象并返回一个值。该值指示一个对象是小于、等于还是大于另一个对象。
        /// </summary>
        /// <param name="x">要比较的第一个对象。</param>
        /// <param name="y">要比较的第二个对象。</param>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>
        public static int Compare(BEncodeInteger x, BEncodeInteger y)
        {
            return BigInteger.Compare(x._numericValue, y._numericValue);
        }

        /// <summary>
        /// 比较。
        /// </summary>
        /// <param name="left"></param>
        /// <param name="right"></param>
        /// <returns></returns>
        public static bool operator !=(BEncodeInteger left, BEncodeInteger right)
        {
            return !(left == right);
        }

        /// <summary>
        /// 比较。
        /// </summary>
        /// <param name="left"></param>
        /// <param name="right"></param>
        /// <returns></returns>
        public static bool operator <(BEncodeInteger left, BEncodeInteger right)
        {
            return (Compare(left, right) < 0);
        }

        /// <summary>
        /// 比较。
        /// </summary>
        /// <param name="left"></param>
        /// <param name="right"></param>
        /// <returns></returns>
        public static bool operator <=(BEncodeInteger left, BEncodeInteger right)
        {
            return (Compare(left, right) <= 0);
        }

        /// <summary>
        /// 比较。
        /// </summary>
        /// <param name="left"></param>
        /// <param name="right"></param>
        /// <returns></returns>
        public static bool operator ==(BEncodeInteger left, BEncodeInteger right)
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
        public static bool operator >(BEncodeInteger left, BEncodeInteger right)
        {
            return (Compare(left, right) > 0);
        }

        /// <summary>
        /// 比较。
        /// </summary>
        /// <param name="left"></param>
        /// <param name="right"></param>
        /// <returns></returns>
        public static bool operator >=(BEncodeInteger left, BEncodeInteger right)
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
        int IComparer<BEncodeInteger>.Compare(BEncodeInteger x, BEncodeInteger y)
        {
            return BigInteger.Compare(x._numericValue, y._numericValue);
        }

        /// <summary>
        /// 将当前实例与另一个对象比较并返回一个值。该值指示当前实例在排序位置是小于、等于还是大于另一个对象。
        /// </summary>
        /// <param name="obj">要比较的对象。</param>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>
        public int CompareTo(object obj)
        {
            return _numericValue.CompareTo((obj as BEncodeInteger)._numericValue);
        }

        /// <summary>
        /// 将当前实例与另一个对象比较并返回一个值。该值指示当前实例在排序位置是小于、等于还是大于另一个对象。
        /// </summary>
        /// <param name="obj">要比较的对象。</param>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>
        public int CompareTo(BEncodeInteger other)
        {
            return _numericValue.CompareTo(other._numericValue);
        }

        /// <summary>
        /// 确定此实例和指定的对象具有相同的值。
        /// </summary>
        /// <param name="other">比较的对象。</param>
        /// <returns></returns>
        public bool Equals(BEncodeInteger other)
        {
            return other is BEncodeInteger && _numericValue == other._numericValue;
        }

        /// <summary>
        /// 确定指定的对象是否等于当前对象。
        /// </summary>
        /// <param name="obj">比较的对象。</param>
        /// <returns></returns>
        public override bool Equals(object obj)
        {
            return obj is BEncodeInteger other && _numericValue == other._numericValue;
        }

        /// <summary>
        ///
        /// </summary>
        /// <returns></returns>
        public override int GetHashCode()
        {
            return -1939223833 + EqualityComparer<BigInteger>.Default.GetHashCode(_numericValue);
        }

        /// <summary>
        /// 获取转换为 Int32 格式的数据值。
        /// </summary>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>
        public int GetInt32Value()
        {
            return (int)_numericValue;
        }

        /// <summary>
        /// 获取转换为 Int64 格式的数据值。
        /// </summary>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>
        public long GetInt64Value()
        {
            return (long)_numericValue;
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
            stream.WriteByte(105);  // 'i'
            byte[] value = Encoding.ASCII.GetBytes(_value);
            stream.Write(value, 0, value.Length);
            stream.WriteByte(101);  // 'e'
        }

        /// <summary>
        /// 方法已重写。获取原始格式的数据值。
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return _value;
        }
    }
}