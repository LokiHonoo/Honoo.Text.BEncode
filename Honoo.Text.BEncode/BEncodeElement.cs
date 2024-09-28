using System;
using System.IO;

namespace Honoo.Text.BEncode
{
    /// <summary>
    /// 表示所有 BEncode 元素类型从中继承的基本类型。
    /// </summary>
    public abstract class BEncodeElement
    {
        private readonly BEncodeElementKind _kind;

        /// <summary>
        /// 获取 BEncode 元素的类型。
        /// </summary>
        public BEncodeElementKind Kind => _kind;

        #region Construction

        /// <summary>
        /// 初始化 BEncodeElement 类的新实例。
        /// </summary>
        /// <param name="kind">BEncode 元素的类型。</param>
        protected BEncodeElement(BEncodeElementKind kind)
        {
            _kind = kind;
        }

        #endregion Construction

        /// <summary>
        /// 保存到指定的流。
        /// </summary>
        /// <param name="stream">指定保存的目标流。</param>
        /// <exception cref="Exception"/>
        public abstract void Save(Stream stream);

        internal abstract void ChangeReadOnly(bool isReadOnly);
    }
}