namespace Honoo.Text.BEncode
{
    /// <summary>
    /// BEncode 值的类型。
    /// </summary>
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Design", "CA1008:枚举应具有零值", Justification = "<挂起>")]
    public enum BEncodeValueKind
    {
        /// <summary>
        /// BEncode 字典类型。
        /// </summary>
        BEncodeDictionary = 1,

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