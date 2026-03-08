using System.Diagnostics.CodeAnalysis;

namespace Nozdormu.MacOui
{
    /// <summary>
    /// Mac
    /// </summary>
    public sealed class MacModel
    {
        /// <summary>
        /// 前缀
        /// </summary>
        [SuppressMessage("Performance", "CA1819:属性不应返回数组", Justification = "<挂起>")]
        public byte[] Prefix { get; set; }

        /// <summary>
        /// 公司
        /// </summary>
        public string Corp { get; set; }

        /// <summary>
        /// 国家
        /// </summary>
        public string Region { get; set; }

        /// <summary>
        /// 地址
        /// </summary>
        public string AddressLine1 { get; set; }

        /// <summary>
        /// 地址2
        /// </summary>
        public string AddressLine2 { get; set; }
    }
}
