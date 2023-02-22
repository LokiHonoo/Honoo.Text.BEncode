using System;
using System.IO;
using System.Text;

namespace Honoo.Text
{
    /// <summary>
    /// BEncode 串行数据类型。
    /// </summary>
    public sealed class BEncodeSingle : BEncodeValue
    {
        private readonly byte[] _value;

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
        /// 初始化 BEncodeSingle 类的新实例。
        /// </summary>
        /// <param name="value">字节数据类型的值。</param>
        /// <exception cref="Exception"></exception>
        public BEncodeSingle(byte[] value) : base(BEncodeValueKind.Single)
        {
            if (value == null)
            {
                throw new ArgumentNullException(nameof(value));
            }
            _value = (byte[])value.Clone();
        }

        /// <summary>
        /// 初始化 BEncodeSingle 类的新实例。默认使用 Encoding.UTF8 编码。
        /// </summary>
        /// <param name="value">文本类型的值。</param>
        /// <exception cref="Exception"></exception>
        public BEncodeSingle(string value) : this(value, Encoding.UTF8)
        {
        }

        /// <summary>
        /// 初始化 BEncodeSingle 类的新实例。
        /// </summary>
        /// <param name="value">文本类型的值。</param>
        /// <param name="encoding">用于编码的字符编码。</param>
        /// <exception cref="Exception"></exception>
        public BEncodeSingle(string value, Encoding encoding) : base(BEncodeValueKind.Single)
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
        }

        /// <summary>
        /// 初始化 BEncodeSingle 类的新实例。
        /// </summary>
        /// <param name="content">指定从中读取的流。</param>
        /// <exception cref="Exception"></exception>
        public BEncodeSingle(Stream content) : base(BEncodeValueKind.Single)
        {
            var lenString = new StringBuilder();
            int kc = content.ReadByte();
            if (kc < 48 || kc > 57)  // '0'-'9'
            {
                throw new Exception($"The header char is not a single identification char '0'-'9'. Stop at position: {content.Position}.");
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
            int valueLen = int.Parse(lenString.ToString());
            _value = new byte[valueLen];
            content.Read(_value, 0, _value.Length);
        }

        #endregion Construction

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
        /// <param name="encoding">用于解码的字符编码。</param>
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
            byte[] length = Encoding.ASCII.GetBytes(_value.Length.ToString());
            stream.Write(length, 0, length.Length);
            stream.WriteByte(58);  // ':'
            stream.Write(_value, 0, _value.Length);
        }
    }
}