using System.Collections.Generic;
namespace AutoData
{
    public class Item
    {
        /// <summary>
        /// 编号
        /// </summary>
        public int Id;
		/// <summary>
        /// 名称
        /// </summary>
        public string Name;
		/// <summary>
        /// 图标
        /// </summary>
        public string Icon;
		/// <summary>
        /// 道具种类
        /// </summary>
        public int Kind;
		/// <summary>
        /// 道具类型
        /// </summary>
        public int Type;
		/// <summary>
        /// 性别
        /// </summary>
        public int Sex;
		/// <summary>
        /// 部位
        /// </summary>
        public List<int> Parts;
		/// <summary>
        /// 模型名
        /// </summary>
        public string Avatar;
		/// <summary>
        /// 使用等级
        /// </summary>
        public int Level;
		/// <summary>
        /// 堆叠数量
        /// </summary>
        public int Count;
		/// <summary>
        /// 买入价格
        /// </summary>
        public MoneyCls Cost;
		/// <summary>
        /// 卖出价格
        /// </summary>
        public MoneyCls Sell;
		/// <summary>
        /// 使用效果
        /// </summary>
        public List<UseEffetCls> UseEffet;
		/// <summary>
        /// 身价值
        /// </summary>
        public int Status;
		/// <summary>
        /// 身价值是否叠加
        /// </summary>
        public int Superposition;
		/// <summary>
        /// 增加属性
        /// </summary>
        public List<AttributeCls> Attribute;
		/// <summary>
        /// 道具说明
        /// </summary>
        public string Desc;
		
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
		public class AttributeCls
        {
            /// <summary>
        /// 属性
        /// </summary>
        public int AttributeID;
		/// <summary>
        /// 值
        /// </summary>
        public int Count;
		
            public AttributeCls(string[] data)
            {
            if (data.Length > 0) int.TryParse(data[0].Trim(), out AttributeID);
			if (data.Length > 1) int.TryParse(data[1].Trim(), out Count);
			
            }
        }
		public class UseEffetCls
        {
            /// <summary>
        /// 效果
        /// </summary>
        public int EffectId;
		/// <summary>
        /// 值1
        /// </summary>
        public int Value1;
		/// <summary>
        /// 值2
        /// </summary>
        public int Value2;
		/// <summary>
        /// 值3
        /// </summary>
        public int Value3;
		/// <summary>
        /// 值4
        /// </summary>
        public int Value4;
		
            public UseEffetCls(string[] data)
            {
            if (data.Length > 0) int.TryParse(data[0].Trim(), out EffectId);
			if (data.Length > 1) int.TryParse(data[1].Trim(), out Value1);
			if (data.Length > 2) int.TryParse(data[2].Trim(), out Value2);
			if (data.Length > 3) int.TryParse(data[3].Trim(), out Value3);
			if (data.Length > 4) int.TryParse(data[4].Trim(), out Value4);
			
            }
        }
		

        public Item(string[] data)
        {
            if (data.Length > 0) int.TryParse(data[0].Trim(), out Id);
			Name = data.Length <= 1 + 1 ? "" : data[1];
			Icon = data.Length <= 2 + 1 ? "" : data[2];
			if (data.Length > 3) int.TryParse(data[3].Trim(), out Kind);
			if (data.Length > 4) int.TryParse(data[4].Trim(), out Type);
			if (data.Length > 5) int.TryParse(data[5].Trim(), out Sex);
			{
                Parts = new List<int>();
                var strd = data[6];
                if(!string.IsNullOrEmpty(strd))
                {
                    var sps = strd.Split(',');
                    for (int i = 0; i < sps.Length; i++)
                    {
                        var v = 0;
                        if (sps.Length > i) int.TryParse(sps[i].Trim(), out v);
                        Parts.Add(v);
                    }
                }
            }
			Avatar = data.Length <= 7 + 1 ? "" : data[7];
			if (data.Length > 8) int.TryParse(data[8].Trim(), out Level);
			if (data.Length > 9) int.TryParse(data[9].Trim(), out Count);
			Cost = new MoneyCls(data[10].Split(','));
			Sell = new MoneyCls(data[11].Split(','));
			{
                UseEffet = new List<UseEffetCls>();
                var sps = data[12].Split(';');
                for (int i = 0; i < sps.Length; i++)
                {
                    UseEffet.Add(new UseEffetCls(sps[i].Split(',')));
                }
            }
			if (data.Length > 13) int.TryParse(data[13].Trim(), out Status);
			if (data.Length > 14) int.TryParse(data[14].Trim(), out Superposition);
			{
                Attribute = new List<AttributeCls>();
                var sps = data[15].Split(';');
                for (int i = 0; i < sps.Length; i++)
                {
                    Attribute.Add(new AttributeCls(sps[i].Split(',')));
                }
            }
			Desc = data.Length <= 16 + 1 ? "" : data[16];
			
        }

        static List<Item> _datas;
        static Dictionary<int, int> _dicDataIndexForId = new Dictionary<int, int>();
		

        public static void Load(string datas)
        {
            _datas = new List<Item>();
            foreach (var data in datas.Split('\n'))
            {
                if (!string.IsNullOrEmpty(data))
                {
                    var item = new AutoData.Item(data.Split('\t'));
                    _datas.Add(item);

                    _dicDataIndexForId[item.Id] = _datas.Count - 1;
					
                }
            }
        }
        public static void Load(byte[] bytes)
        {
            _datas = new List<Item>();
            var datas = System.Text.UTF8Encoding.UTF8.GetString(bytes, 0, bytes.Length);
            Load(datas);
        }
        public static List<Item> GetAll()
        {
            return _datas;
        }
        public static Item GetIndex(int index)
        {
            return _datas[index];
        }
        public static Item GetForId(int Id)
        {
            if (!_dicDataIndexForId.ContainsKey(Id))
                return null;
            return GetIndex(_dicDataIndexForId[Id]);
        }
		
    }
}