using System;
using System.Globalization;
using System.IO;
using System.Numerics;
using System.Text;

namespace Honoo.Text
{
    /// <summary>
    /// BEncode 数值类型。
    /// </summary>
    public sealed class BEncodeInteger : BEncodeValue
    {
        private readonly string _value;

        /// <summary>
        /// 获取原始格式的数据值。
        /// </summary>
        public string Value => _value;

        #region Construction

        /// <summary>
        /// 初始化 BEncodeInteger 类的新实例。
        /// </summary>
        /// <param name="value">十进制文本表示的长数值类型的值。</param>
        /// <exception cref="Exception"></exception>
        public BEncodeInteger(string value) : base(BEncodeValueKind.Integer)
        {
            if (string.IsNullOrWhiteSpace(value))
            {
                throw new ArgumentNullException(nameof(value));
            }
            _ = BigInteger.Parse(value, NumberStyles.Number);
            _value = value;
        }

        /// <summary>
        /// 初始化 BEncodeInteger 类的新实例。
        /// </summary>
        /// <param name="value">Int64 类型的值。</param>
        /// <exception cref="Exception"></exception>
        public BEncodeInteger(long value) : base(BEncodeValueKind.Integer)
        {
            _value = value.ToString();
        }

        /// <summary>
        /// 初始化 BEncodeInteger 类的新实例。
        /// </summary>
        /// <param name="content">指定从中读取的流。</param>
        /// <exception cref="Exception"></exception>
        public BEncodeInteger(Stream content) : base(BEncodeValueKind.Integer)
        {
            int kc = content.ReadByte();
            if (kc != 105)  // 'i'
            {
                throw new Exception($"The header char is not a integer identification char 'i'. Stop at position: {content.Position}.");
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
        }

        #endregion Construction

        /// <summary>
        /// 获取转换为 Int32 格式的数据值。
        /// </summary>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>
        public int GetInt32Value()
        {
            return int.Parse(_value);
        }

        /// <summary>
        /// 获取转换为 Int64 格式的数据值。
        /// </summary>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>
        public long GetInt64Value()
        {
            return long.Parse(_value);
        }

        /// <summary>
        /// 保存到指定的流。
        /// </summary>
        /// <param name="stream">指定保存的目标流。</param>
        /// <exception cref="Exception"/>
        public void Save(Stream stream)
        {
            SaveInternal(stream, null);
        }

        internal override void SaveInternal(Stream stream, Encoding keyEncoding)
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
    }
}