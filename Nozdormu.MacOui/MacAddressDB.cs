using System;
using System.IO;
using System.Net.NetworkInformation;
using System.Reflection;
using System.Text;

namespace Nozdormu.MacOui
{
    /// <summary>
    /// MAC 地址数据库
    /// </summary>
    public static class MacAddressDB
    {
        /// <summary>
        /// 查询数据库
        /// </summary>
        /// <param name="addr">Mac</param>
        /// <returns>查询结果</returns>
        public static MacModel TryLookup(PhysicalAddress addr)
        {
            if (addr is null)
            {
                throw new ArgumentNullException(nameof(addr));
            }

            return TryLookup(addr.GetAddressBytes());
        }

        /// <summary>
        /// 查询数据库
        /// </summary>
        /// <param name="pfx">3位Mac前缀</param>
        /// <returns>查询结果</returns>
        public static MacModel TryLookup(byte[] pfx)
        {
            if (pfx?.Length != 3 && pfx?.Length != 6)
            {
                throw new ArgumentException($"pfx must by count =3 or =6 but got '{pfx?.Length}'");
            }
            using var sr = Assembly.GetExecutingAssembly().GetManifestResourceStream("oui.dat");
            using var br = new BinaryReader(sr);

            uint count = br.ReadUInt32();

            for (uint n = 0; n < count; n++)
            {
                sr.Position = sizeof(uint) + n * (sizeof(long) + 3);
                var prefix = br.ReadBytes(3);
                long offset = br.ReadInt64();
                if (prefix[0] == pfx[0] && prefix[1] == pfx[1] && prefix[2] == pfx[2])
                {
                    sr.Position = sizeof(uint) + count * (sizeof(long) + 3) + offset;
                    var strLen = br.ReadInt32();
                    var str = Encoding.ASCII.GetString(br.ReadBytes(strLen)).Split('\n');
                    return new MacModel
                    {
                        Prefix = pfx,
                        Corp = str[0],
                        Region = str[1],
                        AddressLine1 = str[2],
                        AddressLine2 = str[3],
                    };
                }
            }

            return null;
        }
    }
}