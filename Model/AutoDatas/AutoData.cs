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
            loadhandle("Building", delegate (byte[] data)
            {
                Building.Load(data);
            });
            loadhandle("BuildingLevel", delegate (byte[] data)
            {
                BuildingLevel.Load(data);
            });
            loadhandle("Character", delegate (byte[] data)
            {
                Character.Load(data);
            });
            loadhandle("CompanyInfo", delegate (byte[] data)
            {
                CompanyInfo.Load(data);
            });
            loadhandle("Companyname", delegate (byte[] data)
            {
                Companyname.Load(data);
            });
            loadhandle("DepartmentInfo", delegate (byte[] data)
            {
                DepartmentInfo.Load(data);
            });
            loadhandle("Extension", delegate (byte[] data)
            {
                Extension.Load(data);
            });
            loadhandle("Item", delegate (byte[] data)
            {
                Item.Load(data);
            });
            loadhandle("LandInfo", delegate (byte[] data)
            {
                LandInfo.Load(data);
            });
            loadhandle("Level", delegate (byte[] data)
            {
                Level.Load(data);
            });
            loadhandle("LocalString", delegate (byte[] data)
            {
                LocalString.Load(data);
            });
            loadhandle("Room", delegate (byte[] data)
            {
                Room.Load(data);
            });
            loadhandle("Roomfurniture", delegate (byte[] data)
            {
                Roomfurniture.Load(data);
            });

            return 13;
        }
    }
}
