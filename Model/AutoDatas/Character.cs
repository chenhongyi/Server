using System.Collections.Generic;
namespace AutoData
{
    public class Character
    {
        /// <summary>
        /// 主键
        /// </summary>
        public int Id;
		/// <summary>
        /// 名字
        /// </summary>
        public string Name;
		/// <summary>
        /// 头像
        /// </summary>
        public string Icon;
		/// <summary>
        /// 类型
        /// </summary>
        public int Type;
		/// <summary>
        /// 性别
        /// </summary>
        public int Sex;
		/// <summary>
        /// 等级
        /// </summary>
        public int Level;
		/// <summary>
        /// 属性
        /// </summary>
        public List<AttributeCls> Attribute;
		/// <summary>
        /// 外观
        /// </summary>
        public List<int> Avatar;
		/// <summary>
        /// 说明
        /// </summary>
        public string Desc;
		
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
		

        public Character(string[] data)
        {
            if (data.Length > 0) int.TryParse(data[0].Trim(), out Id);
			Name = data.Length <= 1 + 1 ? "" : data[1];
			Icon = data.Length <= 2 + 1 ? "" : data[2];
			if (data.Length > 3) int.TryParse(data[3].Trim(), out Type);
			if (data.Length > 4) int.TryParse(data[4].Trim(), out Sex);
			if (data.Length > 5) int.TryParse(data[5].Trim(), out Level);
			{
                Attribute = new List<AttributeCls>();
                var sps = data[6].Split(';');
                for (int i = 0; i < sps.Length; i++)
                {
                    Attribute.Add(new AttributeCls(sps[i].Split(',')));
                }
            }
			{
                Avatar = new List<int>();
                var strd = data[7];
                if(!string.IsNullOrEmpty(strd))
                {
                    var sps = strd.Split(',');
                    for (int i = 0; i < sps.Length; i++)
                    {
                        var v = 0;
                        if (sps.Length > i) int.TryParse(sps[i].Trim(), out v);
                        Avatar.Add(v);
                    }
                }
            }
			Desc = data.Length <= 8 + 1 ? "" : data[8];
			
        }

        static List<Character> _datas;
        static Dictionary<int, int> _dicDataIndexForId = new Dictionary<int, int>();
		

        public static void Load(string datas)
        {
            _datas = new List<Character>();
            foreach (var data in datas.Split('\n'))
            {
                if (!string.IsNullOrEmpty(data))
                {
                    var item = new AutoData.Character(data.Split('\t'));
                    _datas.Add(item);

                    _dicDataIndexForId[item.Id] = _datas.Count - 1;
					
                }
            }
        }
        public static void Load(byte[] bytes)
        {
            _datas = new List<Character>();
            var datas = System.Text.UTF8Encoding.UTF8.GetString(bytes, 0, bytes.Length);
            Load(datas);
        }
        public static List<Character> GetAll()
        {
            return _datas;
        }
        public static Character GetIndex(int index)
        {
            return _datas[index];
        }
        public static Character GetForId(int Id)
        {
            if (!_dicDataIndexForId.ContainsKey(Id))
                return null;
            return GetIndex(_dicDataIndexForId[Id]);
        }
		
    }
}