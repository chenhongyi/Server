using System.Collections.Generic;
namespace AutoData
{
    public class CompanyInfo
    {
        /// <summary>
        /// 等级
        /// </summary>
        public int Id;
		/// <summary>
        /// 总营收
        /// </summary>
        public int TotalRevenue;
		/// <summary>
        /// 所需人事部等级
        /// </summary>
        public int PersonnelLv;
		/// <summary>
        /// 所需财务部等级
        /// </summary>
        public int FinanceLv;
		/// <summary>
        /// 所需市场部等级
        /// </summary>
        public int MarketLv;
		/// <summary>
        /// 所需投资部等级
        /// </summary>
        public int InvestmentLv;
		/// <summary>
        /// 金砖成本
        /// </summary>
        public int CostGold;
		/// <summary>
        /// 基础客流
        /// </summary>
        public int Basicpassenger;
		/// <summary>
        /// 部门等级上限
        /// </summary>
        public int DepartLvLimit;
		/// <summary>
        /// 公司营收加成
        /// </summary>
        public float CompanyAddition;
		/// <summary>
        /// 身价
        /// </summary>
        public int Income;
		
        

        public CompanyInfo(string[] data)
        {
            if (data.Length > 0) int.TryParse(data[0].Trim(), out Id);
			if (data.Length > 1) int.TryParse(data[1].Trim(), out TotalRevenue);
			if (data.Length > 2) int.TryParse(data[2].Trim(), out PersonnelLv);
			if (data.Length > 3) int.TryParse(data[3].Trim(), out FinanceLv);
			if (data.Length > 4) int.TryParse(data[4].Trim(), out MarketLv);
			if (data.Length > 5) int.TryParse(data[5].Trim(), out InvestmentLv);
			if (data.Length > 6) int.TryParse(data[6].Trim(), out CostGold);
			if (data.Length > 7) int.TryParse(data[7].Trim(), out Basicpassenger);
			if (data.Length > 8) int.TryParse(data[8].Trim(), out DepartLvLimit);
			if (data.Length > 9) float.TryParse(data[9].Trim(), out CompanyAddition);
			if (data.Length > 10) int.TryParse(data[10].Trim(), out Income);
			
        }

        static List<CompanyInfo> _datas;
        static Dictionary<int, int> _dicDataIndexForId = new Dictionary<int, int>();
		

        public static void Load(string datas)
        {
            _datas = new List<CompanyInfo>();
            foreach (var data in datas.Split('\n'))
            {
                if (!string.IsNullOrEmpty(data))
                {
                    var item = new AutoData.CompanyInfo(data.Split('\t'));
                    _datas.Add(item);

                    _dicDataIndexForId[item.Id] = _datas.Count - 1;
					
                }
            }
        }
        public static void Load(byte[] bytes)
        {
            _datas = new List<CompanyInfo>();
            var datas = System.Text.UTF8Encoding.UTF8.GetString(bytes, 0, bytes.Length);
            Load(datas);
        }
        public static List<CompanyInfo> GetAll()
        {
            return _datas;
        }
        public static CompanyInfo GetIndex(int index)
        {
            return _datas[index];
        }
        public static CompanyInfo GetForId(int Id)
        {
            if (!_dicDataIndexForId.ContainsKey(Id))
                return null;
            return GetIndex(_dicDataIndexForId[Id]);
        }
		
    }
}