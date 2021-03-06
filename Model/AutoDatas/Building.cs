using System.Collections.Generic;
namespace AutoData
{
    public class Building
    {
        /// <summary>
        /// 编号
        /// </summary>
        public int Id;
		/// <summary>
        /// 店铺的icon
        /// </summary>
        public string Icon;
		/// <summary>
        /// 店铺名称
        /// </summary>
        public string Name;
		/// <summary>
        /// 类型
        /// </summary>
        public int Type;
		/// <summary>
        /// 建造消耗金砖
        /// </summary>
        public MoneyCls BuildingCost;
		/// <summary>
        /// 拆除消耗钞票
        /// </summary>
        public MoneyCls DestoryCost;
		/// <summary>
        /// 身价
        /// </summary>
        public int Income;
		
        public class MoneyCls
        {
            /// <summary>
        /// 货币类型
        /// </summary>
        public int GoldType;
		/// <summary>
        /// 货币数量
        /// </summary>
        public int Count;
		
            public MoneyCls(string[] data)
            {
            if (data.Length > 0) int.TryParse(data[0].Trim(), out GoldType);
			if (data.Length > 1) int.TryParse(data[1].Trim(), out Count);
			
            }
        }
		

        public Building(string[] data)
        {
            if (data.Length > 0) int.TryParse(data[0].Trim(), out Id);
			Icon = data.Length <= 1 + 1 ? "" : data[1];
			Name = data.Length <= 2 + 1 ? "" : data[2];
			if (data.Length > 3) int.TryParse(data[3].Trim(), out Type);
			BuildingCost = new MoneyCls(data[4].Split(','));
			DestoryCost = new MoneyCls(data[5].Split(','));
			if (data.Length > 6) int.TryParse(data[6].Trim(), out Income);
			
        }

        static List<Building> _datas;
        static Dictionary<int, int> _dicDataIndexForId = new Dictionary<int, int>();
		

        public static void Load(string datas)
        {
            _datas = new List<Building>();
            foreach (var data in datas.Split('\n'))
            {
                if (!string.IsNullOrEmpty(data))
                {
                    var item = new AutoData.Building(data.Split('\t'));
                    _datas.Add(item);

                    _dicDataIndexForId[item.Id] = _datas.Count - 1;
					
                }
            }
        }
        public static void Load(byte[] bytes)
        {
            _datas = new List<Building>();
            var datas = System.Text.UTF8Encoding.UTF8.GetString(bytes, 0, bytes.Length);
            Load(datas);
        }
        public static List<Building> GetAll()
        {
            return _datas;
        }
        public static Building GetIndex(int index)
        {
            return _datas[index];
        }
        public static Building GetForId(int Id)
        {
            if (!_dicDataIndexForId.ContainsKey(Id))
                return null;
            return GetIndex(_dicDataIndexForId[Id]);
        }
		
    }
}