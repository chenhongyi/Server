using System.Collections.Generic;
namespace AutoData
{
    public class LandInfo
    {
        /// <summary>
        /// 土地级别
        /// </summary>
        public int Level;
		/// <summary>
        /// 地皮面积
        /// </summary>
        public LandAcreageCls Acreage;
		/// <summary>
        /// 地皮价格
        /// </summary>
        public LandPriceCls Price;
		/// <summary>
        /// 街区影响
        /// </summary>
        public LandBlockCls Block;
		/// <summary>
        /// 土地图标
        /// </summary>
        public string Icon;
		/// <summary>
        /// 身价
        /// </summary>
        public int Income;
		
        public class LandPriceCls
        {
            /// <summary>
        /// 货币ID
        /// </summary>
        public int CurrencyID;
		/// <summary>
        /// 数量
        /// </summary>
        public int Count;
		
            public LandPriceCls(string[] data)
            {
            if (data.Length > 0) int.TryParse(data[0].Trim(), out CurrencyID);
			if (data.Length > 1) int.TryParse(data[1].Trim(), out Count);
			
            }
        }
		public class LandAcreageCls
        {
            /// <summary>
        /// 宽度
        /// </summary>
        public int Width;
		/// <summary>
        /// 高度
        /// </summary>
        public int Height;
		
            public LandAcreageCls(string[] data)
            {
            if (data.Length > 0) int.TryParse(data[0].Trim(), out Width);
			if (data.Length > 1) int.TryParse(data[1].Trim(), out Height);
			
            }
        }
		public class LandBlockCls
        {
            /// <summary>
        /// 宽度
        /// </summary>
        public int Width;
		/// <summary>
        /// 高度
        /// </summary>
        public int Height;
		
            public LandBlockCls(string[] data)
            {
            if (data.Length > 0) int.TryParse(data[0].Trim(), out Width);
			if (data.Length > 1) int.TryParse(data[1].Trim(), out Height);
			
            }
        }
		

        public LandInfo(string[] data)
        {
            if (data.Length > 0) int.TryParse(data[0].Trim(), out Level);
			Acreage = new LandAcreageCls(data[1].Split(','));
			Price = new LandPriceCls(data[2].Split(','));
			Block = new LandBlockCls(data[3].Split(','));
			Icon = data.Length <= 4 + 1 ? "" : data[4];
			if (data.Length > 5) int.TryParse(data[5].Trim(), out Income);
			
        }

        static List<LandInfo> _datas;
        static Dictionary<int, int> _dicDataIndexForLevel = new Dictionary<int, int>();
		

        public static void Load(string datas)
        {
            _datas = new List<LandInfo>();
            foreach (var data in datas.Split('\n'))
            {
                if (!string.IsNullOrEmpty(data))
                {
                    var item = new AutoData.LandInfo(data.Split('\t'));
                    _datas.Add(item);

                    _dicDataIndexForLevel[item.Level] = _datas.Count - 1;
					
                }
            }
        }
        public static void Load(byte[] bytes)
        {
            _datas = new List<LandInfo>();
            var datas = System.Text.UTF8Encoding.UTF8.GetString(bytes, 0, bytes.Length);
            Load(datas);
        }
        public static List<LandInfo> GetAll()
        {
            return _datas;
        }
        public static LandInfo GetIndex(int index)
        {
            return _datas[index];
        }
        public static LandInfo GetForLevel(int Level)
        {
            if (!_dicDataIndexForLevel.ContainsKey(Level))
                return null;
            return GetIndex(_dicDataIndexForLevel[Level]);
        }
		
    }
}