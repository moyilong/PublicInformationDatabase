using System.Text.Json;
using Nozdormu.USB;

namespace TestProject1
{
    internal sealed class USBPidVid
    {
        /// <summary>
        /// 测试USB信息获取
        /// </summary>
        [TestCase((uint)0x8086, (uint)0x10de, true)]
        public void TestUSB(uint vid, uint pid, bool existen)
        {
            var result = UsbIdDatabase.TryGetUSB(vid, pid, out var vendor, out var product);
            Assert.That(result, Is.EqualTo(existen));
            Console.WriteLine(JsonSerializer.Serialize(result));
        }
    }
}
