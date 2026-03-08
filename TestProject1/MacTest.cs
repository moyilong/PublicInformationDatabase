using System.Net.NetworkInformation;
using System.Text.Json;
using Nozdormu.MacOui;

namespace TestProject1
{
    /// <summary>
    /// Mac 数据库测试
    /// </summary>
    internal sealed class MacTest
    {
        /// <summary>
        /// 测试MAC信息获取
        /// </summary>
        /// <param name="inputMac">输入Mac</param>
        /// <param name="isnull">是否有结果</param>
        [TestCase("00:30:67:ec:cb:22", false)]
        [TestCase("15:77:64:ec:cb:22", true)]
        public void TestMac(string inputMac, bool isnull)
        {
            var result = MacAddressDB.TryLookup(PhysicalAddress.Parse(inputMac));
            Assert.That(result, isnull ? Is.Null : Is.Not.Null);
            Console.WriteLine(JsonSerializer.Serialize(result));
        }

        /// <summary>
        /// 查询测试
        /// </summary>
        [Test]
        [TestCase(0x00, 0x30, 0x67)] // BIOSTAR RTL8111F
        [TestCase(0x00, 0x15, 0x5D)] // Hyper-V
        [TestCase(0x4C, 0x23, 0x38)] // Qualcomm Wifi7 Network
        [TestCase(0x34, 0x5A, 0x60)] // MSI RTL8125B
        [TestCase(0x90, 0xe2, 0xba)] // Intel X520-DA2
        public async Task Test(byte a1, byte a2, byte a3)
        {
            var data = MacAddressDB.TryLookup([a1, a2, a3]);
            ;
            Assert.That(data, Is.Not.Null);
            Console.Write(JsonSerializer.Serialize(data));
        }
    }
}
