using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Model
{
    public static class TxtReader
    {
        public static byte[] Read(string path)
        {
            if (string.IsNullOrEmpty(path))
            {
                throw new Exception("文件路径不能为空");
            }
            return System.IO.File.ReadAllBytes(path);
        }

        public static void Init()
        {
            AutoData.Loader.Init((string file, Action<byte[]> load) =>
            {
                load(Read(@"F:\ServiceFabric\Server\Model\DataConfig\" + file+".txt"));
            });
        }
    }
}
