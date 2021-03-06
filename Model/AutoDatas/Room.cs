using System.Collections.Generic;
namespace AutoData
{
    public class Room
    {
        /// <summary>
        /// 主键
        /// </summary>
        public int Key;
		/// <summary>
        /// 名称
        /// </summary>
        public string Name;
		/// <summary>
        /// 图标
        /// </summary>
        public string Icon;
		/// <summary>
        /// 房间模型
        /// </summary>
        public string Model;
		/// <summary>
        /// 价格
        /// </summary>
        public MoneyCls Cost;
		/// <summary>
        /// 身价值
        /// </summary>
        public int Status;
		/// <summary>
        /// 初始家具
        /// </summary>
        public List<FurnitureCls> Furniture;
		
        public class FurnitureCls
        {
            /// <summary>
        /// 位置编号
        /// </summary>
        public int Point;
		/// <summary>
        /// 家具ID
        /// </summary>
        public int ItemID;
		
            public FurnitureCls(string[] data)
            {
            if (data.Length > 0) int.TryParse(data[0].Trim(), out Point);
			if (data.Length > 1) int.TryParse(data[1].Trim(), out ItemID);
			
            }
        }
		public class MoneyCls
        {
            /// <summary>
        /// 货币
        /// </summary>
        public int ItemId;
		/// <summary>
        /// 数量
        /// </summary>
        public int Count;
		
            public MoneyCls(string[] data)
            {
            if (data.Length > 0) int.TryParse(data[0].Trim(), out ItemId);
			if (data.Length > 1) int.TryParse(data[1].Trim(), out Count);
			
            }
        }
		

        public Room(string[] data)
        {
            if (data.Length > 0) int.TryParse(data[0].Trim(), out Key);
			Name = data.Length <= 1 + 1 ? "" : data[1];
			Icon = data.Length <= 2 + 1 ? "" : data[2];
			Model = data.Length <= 3 + 1 ? "" : data[3];
			Cost = new MoneyCls(data[4].Split(','));
			if (data.Length > 5) int.TryParse(data[5].Trim(), out Status);
			{
                Furniture = new List<FurnitureCls>();
                var sps = data[6].Split(';');
                for (int i = 0; i < sps.Length; i++)
                {
                    Furniture.Add(new FurnitureCls(sps[i].Split(',')));
                }
            }
			
        }

        static List<Room> _datas;
        static Dictionary<int, int> _dicDataIndexForKey = new Dictionary<int, int>();
		

        public static void Load(string datas)
        {
            _datas = new List<Room>();
            foreach (var data in datas.Split('\n'))
            {
                if (!string.IsNullOrEmpty(data))
                {
                    var item = new AutoData.Room(data.Split('\t'));
                    _datas.Add(item);

                    _dicDataIndexForKey[item.Key] = _datas.Count - 1;
					
                }
            }
        }
        public static void Load(byte[] bytes)
        {
            _datas = new List<Room>();
            var datas = System.Text.UTF8Encoding.UTF8.GetString(bytes, 0, bytes.Length);
            Load(datas);
        }
        public static List<Room> GetAll()
        {
            return _datas;
        }
        public static Room GetIndex(int index)
        {
            return _datas[index];
        }
        public static Room GetForKey(int Key)
        {
            if (!_dicDataIndexForKey.ContainsKey(Key))
                return null;
            return GetIndex(_dicDataIndexForKey[Key]);
        }
		
    }
}