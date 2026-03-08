using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using Microsoft.Build.Framework;
using Microsoft.Build.Utilities;

namespace BuildTasks
{
    /// <summary>
    /// Mac OUI 数据库生成
    /// </summary>
    public class GenerateMacOUI : Task
    {
        /// <summary>
        /// 输入文件
        /// </summary>
        [Required]
        public string InputFile { get; set; }

        /// <summary>
        /// 输出文件
        /// </summary>
        [Required]
        public string OutputFile { get; set; }

        private IEnumerable<string[]> Reader()
        {
            using var fr = File.OpenRead(InputFile);
            using var tr = new StreamReader(fr);

            List<string> Lines = [];

            do
            {
                var line = tr.ReadLine();
                if (line is null)
                {
                    yield break;
                }
                if (string.IsNullOrEmpty(line))
                {
                    if (Lines.Count > 0)
                    {
                        yield return Lines.ToArray();
                    }
                    Lines.Clear();
                    continue;
                }
                else
                {
                    Lines.Add(line);
                }
            } while (true);
        }

        /// <inheritdoc/>
        [SuppressMessage("Globalization", "CA1305:指定 IFormatProvider", Justification = "<挂起>")]
        [SuppressMessage("Usage", "CA2201:不要引发保留的异常类型", Justification = "<挂起>")]
        [SuppressMessage("Style", "IDE0063:使用简单的 \"using\" 语句", Justification = "<挂起>")]
        public override bool Execute()
        {
            List<MacModel> Models = [];
            foreach (var i in Reader().Skip(1))
            {
                try
                {
                    var thisModel = new MacModel
                    {
                        Prefix =
                        [
                            [
                                .. i[0]
                                    .Substring(0, 8)
                                    .Split('-')
                                    .Select(x => byte.Parse(x, NumberStyles.HexNumber)),
                            ],
                        ],
                        Region = i.Length > 2 ? i[4].Trim() : string.Empty,
                        AddressLine1 = i.Length > 2 ? i[2].Trim() : string.Empty,
                        AddressLine2 = i.Length > 2 ? i[3].Trim() : string.Empty,
                        Corp = i[0].Substring(17).Trim(),
                    };

                    var tfirst = Models.Where(x => x.Payload == thisModel.Payload).FirstOrDefault();
                    if (tfirst is null)
                    {
                        Models.Add(thisModel);
                    }
                    else
                    {
                        tfirst.Prefix.Add(thisModel.Prefix[0]);
                    }
                }
                catch (Exception ex)
                {
                    throw new Exception($"D={i.Length} " + string.Join("\n", i), ex);
                }
            }

            if (File.Exists(OutputFile))
            {
                File.Delete(OutputFile);
            }
            long payloadOffset = 0;
            using (var fw = File.OpenWrite(OutputFile))
            using (var tw = new BinaryWriter(fw))
            {
                tw.Write((uint)Models.Sum(x => x.Prefix.Count));
                foreach (var i in Models)
                {
                    foreach (var j in i.Prefix)
                    {
                        tw.Write(j);
                        tw.Write(payloadOffset);
                    }
                    payloadOffset += Encoding.ASCII.GetByteCount(i.Payload) + sizeof(int);
                }
                foreach (var i in Models)
                {
                    tw.Write((int)Encoding.ASCII.GetByteCount(i.Payload));
                    tw.Write(Encoding.ASCII.GetBytes(i.Payload));
                }
            }
            return true;
        }

        private sealed class MacModel
        {
            /// <summary>
            /// 前缀
            /// </summary>
            public List<byte[]> Prefix { get; set; } = [];

            /// <summary>
            /// 公司
            /// </summary>
            public string Corp { get; set; }

            /// <summary>
            /// 区域
            /// </summary>
            public string Region { get; set; }

            /// <summary>
            /// 地址行一
            /// </summary>
            public string AddressLine1 { get; set; }

            /// <summary>
            /// 地址行二
            /// </summary>
            public string AddressLine2 { get; set; }

            /// <summary>
            /// 负载
            /// </summary>
            public string Payload => $"{Corp}\n{Region}\n{AddressLine1}\n{AddressLine2}";
        }
    }
}