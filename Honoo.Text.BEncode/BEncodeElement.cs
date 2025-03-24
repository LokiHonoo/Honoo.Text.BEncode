using System;
using System.IO;

namespace Honoo.Text.BEncode
{
    /// <summary>
    /// 表示所有 BEncode 元素类型从中继承的基本类型。
    /// </summary>
    public abstract class BEncodeElement
    {
        #region Members

        private readonly BEncodeDocument _document;
        private readonly BEncodeElementType _elementType;

        #endregion Members

        /// <summary>
        /// 获取所属的 <see cref="BEncodeDocument"/> 实例。
        /// </summary>
        public BEncodeDocument Document => _document;

        /// <summary>
        /// 获取 BEncode 元素的类型。
        /// </summary>
        public BEncodeElementType ElementType => _elementType;

        #region Construction

        /// <summary>
        /// 初始化 BEncodeElement 类的新实例。
        /// </summary>
        /// <param name="elementType">BEncode 元素的类型。</param>
        /// <param name="document">所属的 <see cref="BEncodeDocument"/> 实例。</param>
        protected BEncodeElement(BEncodeElementType elementType, BEncodeDocument document)
        {
            _elementType = elementType;
            _document = document;
        }

        #endregion Construction

        /// <summary>
        /// 保存到指定的流。
        /// </summary>
        /// <param name="stream">指定保存的目标流。</param>
        /// <exception cref="Exception"/>
        internal abstract void Save(Stream stream);
    }
}