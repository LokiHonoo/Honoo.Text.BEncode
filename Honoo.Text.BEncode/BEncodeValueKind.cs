namespace Honoo.Text.BEncode
{
    /// <summary>
    /// BEncode 值的类型。
    /// </summary>
    public enum BEncodeValueKind
    {
        /// <summary>
        /// BEncode 串行数据类型。
        /// </summary>
        String = 1,

        /// <summary>
        /// BEncode 数值类型。
        /// </summary>
        Integer,

        /// <summary>
        /// BEncode 列表类型。
        /// </summary>
        List,

        /// <summary>
        /// BEncode 字典类型。
        /// </summary>
        Dictionary,
    }
}