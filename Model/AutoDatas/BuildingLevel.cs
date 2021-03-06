using System.Collections.Generic;
namespace AutoData
{
    public class BuildingLevel
    {
        /// <summary>
        /// 等级
        /// </summary>
        public int Id;
		/// <summary>
        /// 知名度
        /// </summary>
        public int Popularity;
		/// <summary>
        /// 升级消耗金砖
        /// </summary>
        public MoneyCls UpgradeCost;
		/// <summary>
        /// 基础客流
        /// </summary>
        public int CustomerAddtion;
		/// <summary>
        /// 员工数量
        /// </summary>
        public int ClerkNums;
		/// <summary>
        /// 宣传费用
        /// </summary>
        public int PropagandaCost;
		/// <summary>
        /// 宣传随机数
        /// </summary>
        public RandomNumberCls RandomNumber;
		/// <summary>
        /// 消耗体力
        /// </summary>
        public int NeedStrength;
		/// <summary>
        /// 拆除费用
        /// </summary>
        public MoneyCls DismantleCost;
		/// <summary>
        /// 身价
        /// </summary>
        public int Income;
		
        public class MoneyCls
        {
            /// <summary>
        /// 货币ID
        /// </summary>
        public int CurrencyID;
		/// <summary>
        /// 数量
        /// </summary>
        public int Count;
		
            public MoneyCls(string[] data)
            {
            if (data.Length > 0) int.TryParse(data[0].Trim(), out CurrencyID);
			if (data.Length > 1) int.TryParse(data[1].Trim(), out Count);
			
            }
        }
		public class RandomNumberCls
        {
            /// <summary>
        /// 最小值
        /// </summary>
        public int Minimum;
		/// <summary>
        /// 最大值
        /// </summary>
        public int Maximum;
		
            public RandomNumberCls(string[] data)
            {
            if (data.Length > 0) int.TryParse(data[0].Trim(), out Minimum);
			if (data.Length > 1) int.TryParse(data[1].Trim(), out Maximum);
			
            }
        }
		

        public BuildingLevel(string[] data)
        {
            if (data.Length > 0) int.TryParse(data[0].Trim(), out Id);
			if (data.Length > 1) int.TryParse(data[1].Trim(), out Popularity);
			UpgradeCost = new MoneyCls(data[2].Split(','));
			if (data.Length > 3) int.TryParse(data[3].Trim(), out CustomerAddtion);
			if (data.Length > 4) int.TryParse(data[4].Trim(), out ClerkNums);
			if (data.Length > 5) int.TryParse(data[5].Trim(), out PropagandaCost);
			RandomNumber = new RandomNumberCls(data[6].Split(','));
			if (data.Length > 7) int.TryParse(data[7].Trim(), out NeedStrength);
			DismantleCost = new MoneyCls(data[8].Split(','));
			if (data.Length > 9) int.TryParse(data[9].Trim(), out Income);
			
        }

        static List<BuildingLevel> _datas;
        static Dictionary<int, int> _dicDataIndexForId = new Dictionary<int, int>();
		

        public static void Load(string datas)
        {
            _datas = new List<BuildingLevel>();
            foreach (var data in datas.Split('\n'))
            {
                if (!string.IsNullOrEmpty(data))
                {
                    var item = new AutoData.BuildingLevel(data.Split('\t'));
                    _datas.Add(item);

                    _dicDataIndexForId[item.Id] = _datas.Count - 1;
					
                }
            }
        }
        public static void Load(byte[] bytes)
        {
            _datas = new List<BuildingLevel>();
            var datas = System.Text.UTF8Encoding.UTF8.GetString(bytes, 0, bytes.Length);
            Load(datas);
        }
        public static List<BuildingLevel> GetAll()
        {
            return _datas;
        }
        public static BuildingLevel GetIndex(int index)
        {
            return _datas[index];
        }
        public static BuildingLevel GetForId(int Id)
        {
            if (!_dicDataIndexForId.ContainsKey(Id))
                return null;
            return GetIndex(_dicDataIndexForId[Id]);
        }
		
    }
}