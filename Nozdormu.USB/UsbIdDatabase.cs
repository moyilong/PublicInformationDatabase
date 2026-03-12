using System;
using System.Globalization;
using System.IO;
using System.IO.Compression;
using System.Reflection;

namespace Nozdormu.USB
{
    /// <summary>
    /// USB ID 数据库
    /// </summary>
    public static class UsbIdDatabase
    {
        /// <summary>
        /// 查询USB数据库
        /// </summary>
        /// <param name="vid">VID</param>
        /// <param name="pid">PID</param>
        /// <param name="vendor">Vendor</param>
        /// <param name="product">产品</param>
        /// <returns>结果</returns>
        public static bool TryGetUSB(uint vid, uint pid, out string vendor, out string product)
        {
            using var fo = Assembly.GetExecutingAssembly().GetManifestResourceStream("usb.ids.gz");
            using var gz = new GZipStream(fo, CompressionMode.Decompress);
            using var sr = new StreamReader(gz);

            vendor = null;
            product = null;

            do
            {
                var line = sr.ReadLine();
                if (line == null)
                    break;
                if (line is "# List of known device classes, subclasses and protocols")
                    break;
                if (string.IsNullOrWhiteSpace(line))
                    continue;
                if (line.StartsWith("#", StringComparison.OrdinalIgnoreCase))
                    continue;
                // 不处理接口
                if (line[0] == '\t' && line[1] == '\t')
                    continue;
                if (line[0] == '\t')
                {
                    if (vendor is not null)
                    {
                        var id = uint.Parse(
                            line.Substring(0, 5),
                            NumberStyles.HexNumber,
                            null
                        );
                        if (id == pid)
                        {
                            product = line.Substring(6);
                            return true;
                        }
                    }
                }
                else
                {
                    line = line.Trim();
                    var id = uint.Parse(
                        line.Substring(0, 5),
                        NumberStyles.HexNumber,
                        null
                    );

                    if (id == vid)
                    {
                        vendor = line.Substring(6).Trim();
                    }
                }
            } while (true);

            vendor = null;
            product = null;
            return false;
        }
    }
}
