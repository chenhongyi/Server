using System.Collections.Generic;
namespace AutoData
{
    public class Extension
    {
        /// <summary>
        /// 等级
        /// </summary>
        public int Id;
		/// <summary>
        /// 所需等级
        /// </summary>
        public int NeedLv;
		/// <summary>
        /// 扩建等级
        /// </summary>
        public int ExtensionLv;
		/// <summary>
        /// 升级消耗
        /// </summary>
        public MoneyCls UpgradeCost;
		/// <summary>
        /// 员工上限增加
        /// </summary>
        public int ClerkAddtion;
		/// <summary>
        /// 基础客流上限增加
        /// </summary>
        public int CustomerAddtion;
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
		

        public Extension(string[] data)
        {
            if (data.Length > 0) int.TryParse(data[0].Trim(), out Id);
			if (data.Length > 1) int.TryParse(data[1].Trim(), out NeedLv);
			if (data.Length > 2) int.TryParse(data[2].Trim(), out ExtensionLv);
			UpgradeCost = new MoneyCls(data[3].Split(','));
			if (data.Length > 4) int.TryParse(data[4].Trim(), out ClerkAddtion);
			if (data.Length > 5) int.TryParse(data[5].Trim(), out CustomerAddtion);
			if (data.Length > 6) int.TryParse(data[6].Trim(), out Income);
			
        }

        static List<Extension> _datas;
        static Dictionary<int, int> _dicDataIndexForId = new Dictionary<int, int>();
		

        public static void Load(string datas)
        {
            _datas = new List<Extension>();
            foreach (var data in datas.Split('\n'))
            {
                if (!string.IsNullOrEmpty(data))
                {
                    var item = new AutoData.Extension(data.Split('\t'));
                    _datas.Add(item);

                    _dicDataIndexForId[item.Id] = _datas.Count - 1;
					
                }
            }
        }
        public static void Load(byte[] bytes)
        {
            _datas = new List<Extension>();
            var datas = System.Text.UTF8Encoding.UTF8.GetString(bytes, 0, bytes.Length);
            Load(datas);
        }
        public static List<Extension> GetAll()
        {
            return _datas;
        }
        public static Extension GetIndex(int index)
        {
            return _datas[index];
        }
        public static Extension GetForId(int Id)
        {
            if (!_dicDataIndexForId.ContainsKey(Id))
                return null;
            return GetIndex(_dicDataIndexForId[Id]);
        }
		
    }
}