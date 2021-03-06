using System.Collections.Generic;
namespace AutoData
{
    public class Avatar
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
        /// 性别
        /// </summary>
        public int Sex;
		/// <summary>
        /// 部位
        /// </summary>
        public int Parts;
		/// <summary>
        /// 模型名
        /// </summary>
        public string AvatarName;
		/// <summary>
        /// 穿戴等级
        /// </summary>
        public int Level;
		/// <summary>
        /// 买入价格
        /// </summary>
        public MoneyCls Cost;
		/// <summary>
        /// 卖出价格
        /// </summary>
        public MoneyCls Sell;
		/// <summary>
        /// 身价值
        /// </summary>
        public int Status;
		/// <summary>
        /// 增加属性
        /// </summary>
        public List<AttributeCls> Attribute;
		
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
		

        public Avatar(string[] data)
        {
            if (data.Length > 0) int.TryParse(data[0].Trim(), out Id);
			Name = data.Length <= 1 + 1 ? "" : data[1];
			Icon = data.Length <= 2 + 1 ? "" : data[2];
			if (data.Length > 3) int.TryParse(data[3].Trim(), out Sex);
			if (data.Length > 4) int.TryParse(data[4].Trim(), out Parts);
			AvatarName = data.Length <= 5 + 1 ? "" : data[5];
			if (data.Length > 6) int.TryParse(data[6].Trim(), out Level);
			Cost = new MoneyCls(data[7].Split(','));
			Sell = new MoneyCls(data[8].Split(','));
			if (data.Length > 9) int.TryParse(data[9].Trim(), out Status);
			{
                Attribute = new List<AttributeCls>();
                var sps = data[10].Split(';');
                for (int i = 0; i < sps.Length; i++)
                {
                    Attribute.Add(new AttributeCls(sps[i].Split(',')));
                }
            }
			
        }

        static List<Avatar> _datas;
        static Dictionary<int, int> _dicDataIndexForId = new Dictionary<int, int>();
		

        public static void Load(string datas)
        {
            _datas = new List<Avatar>();
            foreach (var data in datas.Split('\n'))
            {
                if (!string.IsNullOrEmpty(data))
                {
                    var item = new AutoData.Avatar(data.Split('\t'));
                    _datas.Add(item);

                    _dicDataIndexForId[item.Id] = _datas.Count - 1;
					
                }
            }
        }
        public static void Load(byte[] bytes)
        {
            _datas = new List<Avatar>();
            var datas = System.Text.UTF8Encoding.UTF8.GetString(bytes, 0, bytes.Length);
            Load(datas);
        }
        public static List<Avatar> GetAll()
        {
            return _datas;
        }
        public static Avatar GetIndex(int index)
        {
            return _datas[index];
        }
        public static Avatar GetForId(int Id)
        {
            if (!_dicDataIndexForId.ContainsKey(Id))
                return null;
            return GetIndex(_dicDataIndexForId[Id]);
        }
		
    }
}