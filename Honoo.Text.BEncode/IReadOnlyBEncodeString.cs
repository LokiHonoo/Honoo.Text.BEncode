using System;
using System.IO;
using System.Text;

namespace Honoo.Text.BEncode
{
    /// <summary>
    /// BEncode 串行数据类型只读接口。
    /// </summary>
    public interface IReadOnlyBEncodeString
    {
        /// <summary>
        /// 获取一个值，该值指示此 <see cref="BEncodeString"/> 是否为只读。
        /// </summary>
        bool IsReadOnly { get; }

        /// <summary>
        /// 获取原始字节类型的数据值。
        /// </summary>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Performance", "CA1819:属性不应返回数组", Justification = "<挂起>")]
        byte[] Value { get; }

        /// <summary>
        /// 获取转换为十六进制字符串格式的数据值。
        /// </summary>
        /// <returns></returns>
        string GetHexValue();

        /// <summary>
        /// 获取转换为 string 格式的数据值。转换时默认使用 <see cref="Encoding.UTF8"/> 编码。
        /// </summary>
        /// <returns></returns>
        /// <exception cref="Exception"/>
        string GetStringValue();

        /// <summary>
        /// 获取转换为 string 格式的数据值。
        /// </summary>
        /// <param name="encoding">用于转换的字符编码。</param>
        /// <returns></returns>
        /// <exception cref="Exception"/>
        string GetStringValue(Encoding encoding);

        /// <summary>
        /// 保存到指定的流。
        /// </summary>
        /// <param name="stream">指定保存的目标流。</param>
        /// <exception cref="Exception"/>
        void Save(Stream stream);

        /// <summary>
        /// 方法已重写。获取字符串表示形式的数据值。转换时默认使用 <see cref="Encoding.UTF8"/> 编码。
        /// </summary>
        /// <returns></returns>
        string ToString();

        /// <summary>
        /// 获取字符串表示形式的数据值。
        /// </summary>
        /// <param name="encoding">用于转换的字符编码。</param>
        /// <returns></returns>
        /// <exception cref="Exception"/>
        string ToString(Encoding encoding);
    }
}