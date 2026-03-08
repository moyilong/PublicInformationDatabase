using System.Text.Json;
using Nozdormu.USB;

namespace TestProject1
{
    internal sealed class USBPidVidTest
    {
        /// <summary>
        /// 测试USB信息获取
        /// </summary>
        [TestCase((uint)0x1d6b, (uint)0x0001, true)]
        [TestCase((uint)0x1d6b, (uint)0x0002, true)]
        [TestCase((uint)0x2357, (uint)0x0700, true)]
        public void TestUSB(uint vid, uint pid, bool existen)
        {
            var result = UsbIdDatabase.TryGetUSB(vid, pid, out var vendor, out var product);
             Assert.That(result, Is.EqualTo(existen));
            Console.WriteLine($"Prod={product}");
            Console.WriteLine($"Vendor={vendor}");
        }
    }
}
