﻿using System;
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
    public class BEncodeInteger : BEncodeElement, IEquatable<BEncodeInteger>, IComparer<BEncodeInteger>, IComparable
    {
        #region Members

        private readonly string _value;
        private BigInteger _numericValue;

        /// <summary>
        /// 获取原始格式的数据值。
        /// </summary>
        public string Value => _value;

        internal BigInteger NumericValue => _numericValue;

        #endregion Members

        #region Construction

        /// <summary>
        /// 初始化 BEncodeInteger 类的新实例。
        /// </summary>
        /// <param name="value">十进制文本表示的长数值类型的值。</param>
        /// <param name="document">所属的 <see cref="BEncodeDocument"/> 实例。</param>
        /// <exception cref="Exception"/>
        internal BEncodeInteger(string value, BEncodeDocument document) : base(BEncodeElementType.BEncodeInteger, document)
        {
            if (string.IsNullOrWhiteSpace(value))
            {
                throw new ArgumentNullException(nameof(value));
            }
            _numericValue = BigInteger.Parse(value, NumberStyles.Number, CultureInfo.InvariantCulture);
            _value = _numericValue.ToString(CultureInfo.InvariantCulture);
        }

        /// <summary>
        /// 初始化 BEncodeInteger 类的新实例。
        /// </summary>
        /// <param name="value">Int32 类型的值。</param>
        /// <param name="document">所属的 <see cref="BEncodeDocument"/> 实例。</param>
        internal BEncodeInteger(int value, BEncodeDocument document) : base(BEncodeElementType.BEncodeInteger, document)
        {
            _numericValue = value;
            _value = value.ToString(CultureInfo.InvariantCulture);
        }

        /// <summary>
        /// 初始化 BEncodeInteger 类的新实例。
        /// </summary>
        /// <param name="value">Int64 类型的值。</param>
        /// <param name="document">所属的 <see cref="BEncodeDocument"/> 实例。</param>
        internal BEncodeInteger(long value, BEncodeDocument document) : base(BEncodeElementType.BEncodeInteger, document)
        {
            _numericValue = value;
            _value = value.ToString(CultureInfo.InvariantCulture);
        }

        /// <summary>
        /// 初始化 BEncodeInteger 类的新实例。
        /// </summary>
        /// <param name="content">指定从中读取的流。定位必须在编码标记 <see langword="i"/> 处。</param>
        /// <param name="document">所属的 <see cref="BEncodeDocument"/> 实例。</param>
        /// <exception cref="Exception"/>
        internal BEncodeInteger(Stream content, BEncodeDocument document) : base(BEncodeElementType.BEncodeInteger, document)
        {
            if (content == null)
            {
                throw new ArgumentNullException(nameof(content));
            }
            int kc = content.ReadByte();
            if (kc != 105)  // "i"
            {
                throw new ArgumentException($"The header char is not a integer identification char \"i\". Stop at position: {content.Position}.");
            }
            var valueString = new StringBuilder();
            kc = content.ReadByte();
            while (true)
            {
                if (kc == 101)  // "e"
                {
                    break;
                }
                else
                {
                    valueString.Append((char)kc);
                    kc = content.ReadByte();
                }
            }
            _numericValue = BigInteger.Parse(valueString.ToString(), NumberStyles.Number, CultureInfo.InvariantCulture);
            _value = _numericValue.ToString(CultureInfo.InvariantCulture);
        }

        #endregion Construction

        /// <summary>
        /// 比较两个对象并返回一个值。该值指示一个对象是小于、等于还是大于另一个对象。
        /// </summary>
        /// <param name="x">要比较的第一个对象。</param>
        /// <param name="y">要比较的第二个对象。</param>
        /// <returns></returns>
        public static int Compare(BEncodeInteger x, BEncodeInteger y)
        {
            if (x is null)
            {
                throw new ArgumentNullException(nameof(x));
            }
            if (y is null)
            {
                throw new ArgumentNullException(nameof(y));
            }
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
            return Compare(left, right) < 0;
        }

        /// <summary>
        /// 比较。
        /// </summary>
        /// <param name="left"></param>
        /// <param name="right"></param>
        /// <returns></returns>
        public static bool operator <=(BEncodeInteger left, BEncodeInteger right)
        {
            return Compare(left, right) <= 0;
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
            return Compare(left, right) > 0;
        }

        /// <summary>
        /// 比较。
        /// </summary>
        /// <param name="left"></param>
        /// <param name="right"></param>
        /// <returns></returns>
        public static bool operator >=(BEncodeInteger left, BEncodeInteger right)
        {
            return Compare(left, right) >= 0;
        }

        /// <summary>
        /// 比较两个对象并返回一个值。该值指示一个对象是小于、等于还是大于另一个对象。
        /// </summary>
        /// <param name="x">要比较的第一个对象。</param>
        /// <param name="y">要比较的第二个对象。</param>
        /// <returns></returns>
        int IComparer<BEncodeInteger>.Compare(BEncodeInteger x, BEncodeInteger y)
        {
            return BigInteger.Compare(x._numericValue, y._numericValue);
        }

        /// <summary>
        /// 将当前实例与另一个对象比较并返回一个值。该值指示当前实例在排序位置是小于、等于还是大于另一个对象。
        /// </summary>
        /// <param name="obj">要比较的对象。</param>
        /// <returns></returns>
        /// <exception cref="Exception"/>
        public int CompareTo(object obj)
        {
            if (obj is BEncodeInteger other)
            {
                return CompareTo(other);
            }
            throw new ArgumentException($"{nameof(obj)} is not a BEncodeInteger.");
        }

        /// <summary>
        /// 将当前实例与另一个对象比较并返回一个值。该值指示当前实例在排序位置是小于、等于还是大于另一个对象。
        /// </summary>
        /// <param name="other">要比较的对象。</param>
        /// <returns></returns>
        /// <exception cref="Exception"/>
        public int CompareTo(BEncodeInteger other)
        {
            if (other is null)
            {
                throw new ArgumentNullException(nameof(other));
            }
            return _numericValue.CompareTo(other._numericValue);
        }

        /// <summary>
        /// 确定此实例和指定的对象具有相同的值。
        /// </summary>
        /// <param name="other">比较的对象。</param>
        /// <returns></returns>
        public bool Equals(BEncodeInteger other)
        {
            return other != null && GetHashCode() == other.GetHashCode();
        }

        /// <summary>
        /// 确定指定的对象是否等于当前对象。
        /// </summary>
        /// <param name="obj">比较的对象。</param>
        /// <returns></returns>
        public override bool Equals(object obj)
        {
            return obj is BEncodeInteger other && Equals(other);
        }

        /// <summary>
        /// 方法已重写。获取数值类型的数据值的哈希代码。
        /// </summary>
        /// <returns></returns>
        public override int GetHashCode()
        {
            return -1832168491 + EqualityComparer<BigInteger>.Default.GetHashCode(_numericValue) + EqualityComparer<BEncodeDocument>.Default.GetHashCode(base.Document);
        }

        /// <summary>
        /// 获取转换为 Int32 格式的数据值。
        /// </summary>
        /// <returns></returns>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Design", "CA1024:在适用处使用属性", Justification = "<挂起>")]
        public int GetInt32Value()
        {
            return (int)_numericValue;
        }

        /// <summary>
        /// 获取转换为 Int64 格式的数据值。
        /// </summary>
        /// <returns></returns>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Design", "CA1024:在适用处使用属性", Justification = "<挂起>")]
        public long GetInt64Value()
        {
            return (long)_numericValue;
        }

        /// <summary>
        /// 获取原始格式的数据值。
        /// </summary>
        /// <returns></returns>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Design", "CA1024:在适用处使用属性", Justification = "<挂起>")]
        public string GetRawValue()
        {
            return _value;
        }

        /// <summary>
        /// 方法已重写。获取字符串表示形式的数据值。
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return _value;
        }

        /// <summary>
        /// 保存到指定的流。
        /// </summary>
        /// <param name="stream">指定保存的目标流。</param>
        /// <exception cref="Exception"/>
        internal override void Save(Stream stream)
        {
            if (stream == null)
            {
                throw new ArgumentNullException(nameof(stream));
            }
            stream.WriteByte(105);  // "i"
            byte[] value = Encoding.ASCII.GetBytes(_value);
            stream.Write(value, 0, value.Length);
            stream.WriteByte(101);  // "e"
        }
    }
}