using System;

namespace AutoData
{
    public class Loader
    {
        /// <summary>
        /// 初始化数据
        /// </summary>
        /// <param name="loadhandle">回调接口,arg1:加载文件名,arg2:回调函数传入此文件的byte数据</param>
        public static int Init(Action<string, Action<byte[]>> loadhandle)
        {
            loadhandle("Avatar", delegate (byte[] data)
            {
                Avatar.Load(data);
            });
            loadhandle("Character", delegate (byte[] data)
            {
                Character.Load(data);
            });
            loadhandle("Item", delegate (byte[] data)
            {
                Item.Load(data);
            });
            loadhandle("Level", delegate (byte[] data)
            {
                Level.Load(data);
            });
            loadhandle("Roomfurniture", delegate (byte[] data)
            {
                Roomfurniture.Load(data);
            });

            return 5;
        }
    }
}
