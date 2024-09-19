using System;
using System.Collections.Generic;
using System.IO;

namespace Honoo.Text.BEncode
{
    /// <summary>
    /// BEncode 列表类型只读接口。
    /// </summary>
    public interface IReadOnlyBEncodeList
    {
        /// <summary>
        /// 获取元素集合中包含的元素数。
        /// </summary>
        int Count { get; }

        /// <summary>
        /// 获取指定索引的元素的值。
        /// </summary>
        /// <param name="index">元素的索引。</param>
        /// <returns></returns>
        /// <exception cref="Exception"/>
        BEncodeValue this[int index] { get; }

        /// <summary>
        /// 确定指定元素是否在集合中。
        /// </summary>
        /// <typeparam name="T">指定元素类型。</typeparam>
        /// <param name="value">搜索的指定对象。</param>
        /// <returns></returns>
        bool Contains<T>(T value) where T : BEncodeValue;

        /// <summary>
        /// 从指定数组索引开始将值元素复制到到指定数组。
        /// </summary>
        /// <typeparam name="T">指定元素类型。</typeparam>
        /// <param name="array">要复制到的目标数组。</param>
        /// <param name="arrayIndex">目标数组中从零开始的索引，从此处开始复制。</param>
        void CopyTo<T>(T[] array, int arrayIndex) where T : BEncodeValue;

        /// <summary>
        /// 支持在泛型集合上进行简单迭代。
        /// </summary>
        /// <returns></returns>
        IEnumerator<BEncodeValue> GetEnumerator();

        /// <summary>
        /// 搜索指定对象，并返回第一个匹配项从零开始的索引。
        /// </summary>
        /// <typeparam name="T">指定元素类型。</typeparam>
        /// <param name="value">搜索的指定对象。</param>
        /// <returns></returns>
        int IndexOf<T>(T value) where T : BEncodeValue;

        /// <summary>
        /// 保存到指定的流。
        /// </summary>
        /// <param name="stream">指定保存的目标流。</param>
        /// <exception cref="Exception"/>
        void Save(Stream stream);
    }
}