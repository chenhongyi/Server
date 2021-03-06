using System.Collections.Generic;
namespace AutoData
{
    public class DepartmentInfo
    {
        /// <summary>
        /// 索引
        /// </summary>
        public int Id;
		/// <summary>
        /// 部门名称
        /// </summary>
        public string Name;
		/// <summary>
        /// 等级
        /// </summary>
        public int level;
		/// <summary>
        /// 公司等级
        /// </summary>
        public int CompanyLv;
		/// <summary>
        /// 主管人数
        /// </summary>
        public int DirectorCounts;
		/// <summary>
        /// 所需金砖
        /// </summary>
        public MoneyCls CostGold;
		/// <summary>
        /// 员工上限
        /// </summary>
        public int StaffLimit;
		/// <summary>
        /// 人才市场上限
        /// </summary>
        public int TalentLvLimit;
		/// <summary>
        /// 地产上限
        /// </summary>
        public int RealestateLimit;
		/// <summary>
        /// 店铺上限
        /// </summary>
        public int StoreLimit;
		/// <summary>
        /// 扩建上限
        /// </summary>
        public int ExtensionLimit;
		/// <summary>
        /// 店铺加成
        /// </summary>
        public float StoreAddtion;
		/// <summary>
        /// 宣传次数
        /// </summary>
        public int PropagandaCounts;
		/// <summary>
        /// 策略打击次数
        /// </summary>
        public int StrategicCounts;
		/// <summary>
        /// 公司采购
        /// </summary>
        public int PurchaseCounts;
		/// <summary>
        /// 贷款等级
        /// </summary>
        public int loanLvLimit;
		/// <summary>
        /// 兑换次数
        /// </summary>
        public int MakerCoinCounts;
		/// <summary>
        /// 身价
        /// </summary>
        public int Income;
		/// <summary>
        /// 图标
        /// </summary>
        public string Icon;
		
        public class MoneyCls
        {
            /// <summary>
        /// 类型
        /// </summary>
        public int Type;
		/// <summary>
        /// 数量
        /// </summary>
        public int Count;
		
            public MoneyCls(string[] data)
            {
            if (data.Length > 0) int.TryParse(data[0].Trim(), out Type);
			if (data.Length > 1) int.TryParse(data[1].Trim(), out Count);
			
            }
        }
		

        public DepartmentInfo(string[] data)
        {
            if (data.Length > 0) int.TryParse(data[0].Trim(), out Id);
			Name = data.Length <= 1 + 1 ? "" : data[1];
			if (data.Length > 2) int.TryParse(data[2].Trim(), out level);
			if (data.Length > 3) int.TryParse(data[3].Trim(), out CompanyLv);
			if (data.Length > 4) int.TryParse(data[4].Trim(), out DirectorCounts);
			CostGold = new MoneyCls(data[5].Split(','));
			if (data.Length > 6) int.TryParse(data[6].Trim(), out StaffLimit);
			if (data.Length > 7) int.TryParse(data[7].Trim(), out TalentLvLimit);
			if (data.Length > 8) int.TryParse(data[8].Trim(), out RealestateLimit);
			if (data.Length > 9) int.TryParse(data[9].Trim(), out StoreLimit);
			if (data.Length > 10) int.TryParse(data[10].Trim(), out ExtensionLimit);
			if (data.Length > 11) float.TryParse(data[11].Trim(), out StoreAddtion);
			if (data.Length > 12) int.TryParse(data[12].Trim(), out PropagandaCounts);
			if (data.Length > 13) int.TryParse(data[13].Trim(), out StrategicCounts);
			if (data.Length > 14) int.TryParse(data[14].Trim(), out PurchaseCounts);
			if (data.Length > 15) int.TryParse(data[15].Trim(), out loanLvLimit);
			if (data.Length > 16) int.TryParse(data[16].Trim(), out MakerCoinCounts);
			if (data.Length > 17) int.TryParse(data[17].Trim(), out Income);
			Icon = data.Length <= 18 + 1 ? "" : data[18];
			
        }

        static List<DepartmentInfo> _datas;
        static Dictionary<int, int> _dicDataIndexForId = new Dictionary<int, int>();
		

        public static void Load(string datas)
        {
            _datas = new List<DepartmentInfo>();
            foreach (var data in datas.Split('\n'))
            {
                if (!string.IsNullOrEmpty(data))
                {
                    var item = new AutoData.DepartmentInfo(data.Split('\t'));
                    _datas.Add(item);

                    _dicDataIndexForId[item.Id] = _datas.Count - 1;
					
                }
            }
        }
        public static void Load(byte[] bytes)
        {
            _datas = new List<DepartmentInfo>();
            var datas = System.Text.UTF8Encoding.UTF8.GetString(bytes, 0, bytes.Length);
            Load(datas);
        }
        public static List<DepartmentInfo> GetAll()
        {
            return _datas;
        }
        public static DepartmentInfo GetIndex(int index)
        {
            return _datas[index];
        }
        public static DepartmentInfo GetForId(int Id)
        {
            if (!_dicDataIndexForId.ContainsKey(Id))
                return null;
            return GetIndex(_dicDataIndexForId[Id]);
        }
		
    }
}