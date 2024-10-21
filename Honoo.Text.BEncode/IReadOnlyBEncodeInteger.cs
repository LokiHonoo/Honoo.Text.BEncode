using System;
using System.IO;

namespace Honoo.Text.BEncode
{
    /// <summary>
    /// BEncode 数值类型。
    /// </summary>
    public interface IReadOnlyBEncodeInteger
    {
        /// <summary>
        /// 获取一个值，该值指示此 <see cref="BEncodeInteger"/> 是否为只读。
        /// </summary>
        bool IsReadOnly { get; }

        /// <summary>
        /// 获取原始格式的数据值。
        /// </summary>
        string Value { get; }

        /// <summary>
        /// 获取转换为 Int32 格式的数据值。
        /// </summary>
        /// <returns></returns>
        int GetInt32Value();

        /// <summary>
        /// 获取转换为 Int64 格式的数据值。
        /// </summary>
        /// <returns></returns>
        long GetInt64Value();

        /// <summary>
        /// 保存到指定的流。
        /// </summary>
        /// <param name="stream">指定保存的目标流。</param>
        /// <exception cref="Exception"/>
        void Save(Stream stream);

        /// <summary>
        /// 方法已重写。获取字符串表示形式的数据值。
        /// </summary>
        /// <returns></returns>
        string ToString();
    }
}