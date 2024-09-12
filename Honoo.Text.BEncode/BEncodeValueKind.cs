namespace Honoo.Text.BEncode
{
    /// <summary>
    /// BEncode 值的类型。
    /// </summary>
    public enum BEncodeValueKind
    {
        /// <summary>
        /// BEncode 字典类型。
        /// </summary>
        BEncodeDictionary = 0,

        /// <summary>
        /// BEncode 列表类型。
        /// </summary>
        BEncodeList,

        /// <summary>
        /// BEncode 数值类型。
        /// </summary>
        BEncodeInteger,

        /// <summary>
        /// BEncode 串行数据类型。
        /// </summary>
        BEncodeString,
    }
}