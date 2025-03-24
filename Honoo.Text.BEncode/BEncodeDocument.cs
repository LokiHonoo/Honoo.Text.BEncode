using System;
using System.IO;
using System.Text;

namespace Honoo.Text.BEncode
{
    /// <summary>
    /// BEncode 文档。
    /// </summary>
    public class BEncodeDocument
    {
        #region Members

        private readonly BEncodeDictionary _root;
        private Encoding _encoding = new UTF8Encoding(false);

        /// <summary>
        /// 获取或设置读写此文档元素的默认字符编码。
        /// </summary>
        public Encoding Encoding { get => _encoding; set => _encoding = value; }

        /// <summary>
        /// 获取此文档的根元素。
        /// </summary>
        public BEncodeDictionary Root => _root;

        #endregion Members

        #region Construction

        /// <summary>
        /// 初始化 BEncodeDocument 类的新实例。
        /// </summary>
        public BEncodeDocument()
        {
            _root = new BEncodeDictionary(this);
        }

        /// <summary>
        /// 初始化 BEncodeDocument 类的新实例。
        /// </summary>
        /// <param name="content">指定从中读取的流。定位必须在编码标记 <see langword="d"/> 处。</param>
        /// <exception cref="Exception"/>
        public BEncodeDocument(Stream content)
        {
            _root = new BEncodeDictionary(content, this);
        }

        /// <summary>
        /// 初始化 BEncodeDocument 类的新实例。
        /// </summary>
        /// <param name="document">指定从中复制的文档。</param>
        /// <exception cref="Exception"/>
        public BEncodeDocument(BEncodeDocument document)
        {
            if (document is null)
            {
                throw new ArgumentNullException(nameof(document));
            }
            _root = new BEncodeDictionary(this);
            CopyFrom(document.Root, _root);
        }

        /// <summary>
        /// 初始化 BEncodeDocument 类的新实例。
        /// </summary>
        /// <param name="element">指定从中复制的元素。</param>
        /// <exception cref="Exception"/>
        public BEncodeDocument(BEncodeDictionary element)
        {
            if (element is null)
            {
                throw new ArgumentNullException(nameof(element));
            }
            _root = new BEncodeDictionary(this);
            CopyFrom(element, _root);
        }

        #endregion Construction

        /// <summary>
        /// 创建属于此文档的 BEncodeDictionary 类型的元素。
        /// </summary>
        public BEncodeDictionary CreateDictionary()
        {
            return new BEncodeDictionary(this);
        }

        /// <summary>
        /// 创建属于此文档的 BEncodeInteger 类型的元素。
        /// </summary>
        /// <param name="value">十进制文本表示的长数值类型的值。</param>
        public BEncodeInteger CreateInteger(string value)
        {
            return new BEncodeInteger(value, this);
        }

        /// <summary>
        /// 创建属于此文档的 BEncodeInteger 类型的元素。
        /// </summary>
        /// <param name="value">Int32 类型的值。</param>
        public BEncodeInteger CreateInteger(int value)
        {
            return new BEncodeInteger(value, this);
        }

        /// <summary>
        /// 创建属于此文档的 BEncodeInteger 类型的元素。
        /// </summary>
        /// <param name="value">Int64 类型的值。</param>
        public BEncodeInteger CreateInteger(long value)
        {
            return new BEncodeInteger(value, this);
        }

        /// <summary>
        /// 创建属于此文档的 BEncodeList 类型的元素。
        /// </summary>
        public BEncodeList CreateList()
        {
            return new BEncodeList(this);
        }

        /// <summary>
        /// 创建属于此文档的 BEncodeString 类型的元素。
        /// </summary>
        /// <param name="value">字节数据类型的值。</param>
        public BEncodeString CreateString(byte[] value)
        {
            return new BEncodeString(value, this);
        }

        /// <summary>
        /// 创建属于此文档的 BEncodeString 类型的元素。
        /// </summary>
        /// <param name="value">文本类型的值。</param>
        public BEncodeString CreateString(string value)
        {
            return new BEncodeString(value, this);
        }

        /// <summary>
        /// 创建属于此文档的 BEncodeString 类型的元素。
        /// </summary>
        /// <param name="value">文本类型的值。</param>
        /// <param name="encoding">指定用于转换的字符编码。不使用所属的 <see cref="BEncodeDocument"/> 实例的默认字符编码。</param>
        public BEncodeString CreateString(string value, Encoding encoding)
        {
            return new BEncodeString(value, encoding, this);
        }

        /// <summary>
        /// 保存到指定的流。
        /// </summary>
        /// <param name="stream">指定保存的目标流。</param>
        /// <exception cref="Exception"/>
        public void Save(Stream stream)
        {
            _root.Save(stream);
        }

        private void CopyFrom(BEncodeElement readElement, BEncodeElement writeElement)
        {
            if (writeElement.ElementType != writeElement.ElementType)
            {
                throw new ArgumentException($"Write error at type - \"{writeElement.ElementType}\".");
            }
            switch (readElement.ElementType)
            {
                case BEncodeElementType.BEncodeDictionary:
                    BEncodeDictionary dict = (BEncodeDictionary)writeElement;
                    foreach (var ele in (BEncodeDictionary)readElement)
                    {
                        BEncodeString key = new BEncodeString(ele.Key.GetStringValue(), this);
                        switch (ele.Value.ElementType)
                        {
                            case BEncodeElementType.BEncodeDictionary: CopyFrom(ele.Value, dict.Add(key, new BEncodeDictionary(this))); break;
                            case BEncodeElementType.BEncodeList: CopyFrom(ele.Value, dict.Add(key, new BEncodeList(this))); break;
                            case BEncodeElementType.BEncodeInteger: dict.Add(key, new BEncodeInteger(((BEncodeInteger)ele.Value).GetRawValue(), this)); break;
                            case BEncodeElementType.BEncodeString: dict.Add(key, new BEncodeString(((BEncodeString)ele.Value).GetStringValue(), this)); break;
                            default: throw new ArgumentException($"Read error at type - \"{writeElement.ElementType}\".");
                        }
                    }
                    break;

                case BEncodeElementType.BEncodeList:
                    BEncodeList list = (BEncodeList)writeElement;
                    foreach (var ele in (BEncodeList)readElement)
                    {
                        switch (ele.ElementType)
                        {
                            case BEncodeElementType.BEncodeDictionary: CopyFrom(ele, list.Add(new BEncodeDictionary(this))); break;
                            case BEncodeElementType.BEncodeList: CopyFrom(ele, list.Add(new BEncodeList(this))); break;
                            case BEncodeElementType.BEncodeInteger: list.Add(new BEncodeInteger(((BEncodeInteger)ele).GetRawValue(), this)); break;
                            case BEncodeElementType.BEncodeString: list.Add(new BEncodeString(((BEncodeString)ele).GetStringValue(), this)); break;
                            default: throw new ArgumentException($"Read error at type - \"{writeElement.ElementType}\".");
                        }
                    }
                    break;

                default: throw new ArgumentException($"Read error at type - \"{readElement.ElementType}\".");
            }
        }
    }
}