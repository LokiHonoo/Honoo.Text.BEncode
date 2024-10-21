using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace Honoo.Text.BEncode
{
    /// <summary>
    /// BEncode 字典类型只读接口。
    /// </summary>
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Naming", "CA1711:标识符应采用正确的后缀", Justification = "<挂起>")]
    public interface IReadOnlyBEncodeDictionary
    {
        /// <summary>
        /// 获取元素集合中包含的元素数。
        /// </summary>
        int Count { get; }

        /// <summary>
        /// 获取一个值，该值指示此 <see cref="BEncodeDictionary"/> 是否为只读。
        /// </summary>
        bool IsReadOnly { get; }

        /// <summary>
        /// 获取元素集合的键的集合。
        /// </summary>
        BEncodeDictionary.KeyCollection Keys { get; }

        /// <summary>
        /// 获取元素集合的值的集合。
        /// </summary>
        BEncodeDictionary.ValueCollection Values { get; }

        /// <summary>
        /// 获取具有指定键的元素的值。
        /// </summary>
        /// <param name="key">元素的键。</param>
        /// <returns></returns>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Design", "CA1043:将整型或字符串参数用于索引器", Justification = "<挂起>")]
        BEncodeElement this[BEncodeString key] { get; }

        /// <summary>
        /// 获取具有指定键的元素的值。转换元素的键时默认使用 <see cref="Encoding.UTF8"/> 编码。
        /// </summary>
        /// <param name="key">元素的键。</param>
        /// <returns></returns>
        BEncodeElement this[string key] { get; }

        /// <summary>
        /// 获取具有指定键的元素的值。
        /// </summary>
        /// <param name="key">元素的键。</param>
        /// <param name="keyEncoding">用于转换元素的键的字符编码。</param>
        /// <returns></returns>
        /// <exception cref="Exception"/>
        BEncodeElement this[string key, Encoding keyEncoding] { get; }

        /// <summary>
        /// 确定元素集合是否包含带有指定键的元素。
        /// </summary>
        /// <param name="key">元素的键。</param>
        /// <returns></returns>
        bool ContainsKey(BEncodeString key);

        /// <summary>
        /// 确定元素集合是否包含带有指定键的元素。转换元素的键时默认使用 <see cref="Encoding.UTF8"/> 编码。
        /// </summary>
        /// <param name="key">元素的键。</param>
        /// <returns></returns>
        /// <exception cref="Exception"/>
        bool ContainsKey(string key);

        /// <summary>
        /// 确定元素集合是否包含带有指定键的元素。
        /// </summary>
        /// <param name="key">元素的键。</param>
        /// <param name="keyEncoding">用于转换元素的键的字符编码。</param>
        /// <returns></returns>
        /// <exception cref="Exception"/>
        bool ContainsKey(string key, Encoding keyEncoding);

        /// <summary>
        /// 支持在泛型集合上进行简单迭代。
        /// </summary>
        /// <returns></returns>
        IEnumerator<KeyValuePair<BEncodeString, BEncodeElement>> GetEnumerator();

        /// <summary>
        /// 获取与指定键关联的元素的值。如果没有找到指定键或者无法转换指定的类型，返回 <see langword="null"/>。
        /// </summary>
        /// <typeparam name="T">指定元素类型。</typeparam>
        /// <param name="key">元素的键。</param>
        /// <returns></returns>
        T GetValue<T>(BEncodeString key) where T : BEncodeElement;

        /// <summary>
        /// 获取与指定键关联的元素的值。如果没有找到指定键或者无法转换指定的类型，返回 <see langword="null"/>。转换元素的键时默认使用 <see cref="Encoding.UTF8"/> 编码。
        /// </summary>
        /// <typeparam name="T">指定元素类型。</typeparam>
        /// <param name="key">元素的键。</param>
        /// <returns></returns>
        /// <exception cref="Exception"/>
        T GetValue<T>(string key) where T : BEncodeElement;

        /// <summary>
        /// 获取与指定键关联的元素的值。如果没有找到指定键或者无法转换指定的类型，返回 <see langword="null"/>。
        /// </summary>
        /// <typeparam name="T">指定元素类型。</typeparam>
        /// <param name="key">元素的键。</param>
        /// <param name="keyEncoding">用于转换元素的键的字符编码。</param>
        /// <returns></returns>
        /// <exception cref="Exception"/>
        T GetValue<T>(string key, Encoding keyEncoding) where T : BEncodeElement;

        /// <summary>
        /// 获取与指定键关联的元素的值。如果没有找到指定键或者无法转换指定的类型，返回 <paramref name="defaultValue"/>。
        /// </summary>
        /// <typeparam name="T">指定元素类型。</typeparam>
        /// <param name="key">元素的键。</param>
        /// <param name="defaultValue">没有找到指定键时的元素的默认值。</param>
        /// <returns></returns>
        T GetValue<T>(BEncodeString key, T defaultValue) where T : BEncodeElement;

        /// <summary>
        /// 获取与指定键关联的元素的值。如果没有找到指定键或者无法转换指定的类型，返回 <paramref name="defaultValue"/>。转换元素的键时默认使用 <see cref="Encoding.UTF8"/> 编码。
        /// </summary>
        /// <typeparam name="T">指定元素类型。</typeparam>
        /// <param name="key">元素的键。</param>
        /// <param name="defaultValue">没有找到指定键时的元素的默认值。</param>
        /// <returns></returns>
        /// <exception cref="Exception"/>
        T GetValue<T>(string key, T defaultValue) where T : BEncodeElement;

        /// <summary>
        /// 获取与指定键关联的元素的值。如果没有找到指定键或者无法转换指定的类型，返回 <paramref name="defaultValue"/>。
        /// </summary>
        /// <typeparam name="T">指定元素类型。</typeparam>
        /// <param name="key">元素的键。</param>
        /// <param name="keyEncoding">用于转换元素的键的字符编码。</param>
        /// <param name="defaultValue">没有找到指定键时的元素的默认值。</param>
        /// <returns></returns>
        /// <exception cref="Exception"/>
        T GetValue<T>(string key, Encoding keyEncoding, T defaultValue) where T : BEncodeElement;

        /// <summary>
        /// 保存到指定的流。
        /// </summary>
        /// <param name="stream">指定保存的目标流。</param>
        /// <exception cref="Exception"/>
        void Save(Stream stream);

        /// <summary>
        /// 获取与指定键关联的元素的值。
        /// <br/>如果没有找到指定键，返回 <see langword="false"/>。如果找到了指定键但指定的类型不符，则仍返回 <see langword="false"/>。
        /// </summary>
        /// <typeparam name="T">指定元素类型。</typeparam>
        /// <param name="key">元素的键。</param>
        /// <param name="value">元素的值。</param>
        /// <returns></returns>
        bool TryGetValue<T>(BEncodeString key, out T value) where T : BEncodeElement;

        /// <summary>
        /// 获取与指定键关联的元素的值。
        /// <br/>如果没有找到指定键，返回 <see langword="false"/>。如果找到了指定键但指定的类型不符，则仍返回 <see langword="false"/>。
        /// </summary>
        /// <typeparam name="T">指定元素类型。</typeparam>
        /// <param name="key">元素的键。</param>
        /// <param name="keyEncoding">用于转换元素的键的字符编码。</param>
        /// <param name="value">元素的值。</param>
        /// <returns></returns>
        /// <exception cref="Exception"/>
        bool TryGetValue<T>(string key, Encoding keyEncoding, out T value) where T : BEncodeElement;

        /// <summary>
        /// 获取与指定键关联的元素的值。转换元素的键时默认使用 <see cref="Encoding.UTF8"/> 编码。
        /// <br/>如果没有找到指定键，返回 <see langword="false"/>。如果找到了指定键但指定的类型不符，则仍返回 <see langword="false"/>。
        /// </summary>
        /// <typeparam name="T">指定元素类型。</typeparam>
        /// <param name="key">元素的键。</param>
        /// <param name="value">元素的值。</param>
        /// <returns></returns>
        /// <exception cref="Exception"/>
        bool TryGetValue<T>(string key, out T value) where T : BEncodeElement;
    }
}