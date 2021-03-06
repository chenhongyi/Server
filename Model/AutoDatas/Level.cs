using System.Collections.Generic;
namespace AutoData
{
    public class Level
    {
        /// <summary>
        /// 等级
        /// </summary>
        public int Lv;
		/// <summary>
        /// 经验值
        /// </summary>
        public int Exp;
		/// <summary>
        /// 增加属性
        /// </summary>
        public List<AttributeCls> Attribute;
		
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
		

        public Level(string[] data)
        {
            if (data.Length > 0) int.TryParse(data[0].Trim(), out Lv);
			if (data.Length > 1) int.TryParse(data[1].Trim(), out Exp);
			{
                Attribute = new List<AttributeCls>();
                var sps = data[2].Split(';');
                for (int i = 0; i < sps.Length; i++)
                {
                    Attribute.Add(new AttributeCls(sps[i].Split(',')));
                }
            }
			
        }

        static List<Level> _datas;
        static Dictionary<int, int> _dicDataIndexForLv = new Dictionary<int, int>();
		

        public static void Load(string datas)
        {
            _datas = new List<Level>();
            foreach (var data in datas.Split('\n'))
            {
                if (!string.IsNullOrEmpty(data))
                {
                    var item = new AutoData.Level(data.Split('\t'));
                    _datas.Add(item);

                    _dicDataIndexForLv[item.Lv] = _datas.Count - 1;
					
                }
            }
        }
        public static void Load(byte[] bytes)
        {
            _datas = new List<Level>();
            var datas = System.Text.UTF8Encoding.UTF8.GetString(bytes, 0, bytes.Length);
            Load(datas);
        }
        public static List<Level> GetAll()
        {
            return _datas;
        }
        public static Level GetIndex(int index)
        {
            return _datas[index];
        }
        public static Level GetForLv(int Lv)
        {
            if (!_dicDataIndexForLv.ContainsKey(Lv))
                return null;
            return GetIndex(_dicDataIndexForLv[Lv]);
        }
		
    }
}