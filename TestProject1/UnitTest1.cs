using Nozdormu.MacOui;
using System.Net.NetworkInformation;
using System.Text.Json;

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
        [TestCase("00:30:67:ec:cb:22",false)]
        [TestCase("15:77:64:ec:cb:22",true)]
        public void TestMac(string inputMac,bool isnull)
        {
            var result = MacAddressDB.TryLookup(PhysicalAddress.Parse(inputMac));
            Assert.That(result, isnull ? Is.Null : Is.Not.Null);
            Console.WriteLine(JsonSerializer.Serialize(result));
        }
    }
}