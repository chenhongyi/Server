using System.Collections.Generic;
namespace AutoData
{
    public class Item
    {
        /// <summary>
        /// 主键
        /// </summary>
        public int id;
		/// <summary>
        /// 名称
        /// </summary>
        public string name;
		/// <summary>
        /// 模型
        /// </summary>
        public string model;
		/// <summary>
        /// 分类
        /// </summary>
        public int type;
		/// <summary>
        /// 家具分类
        /// </summary>
        public int furtype;
		/// <summary>
        /// 障碍
        /// </summary>
        public bool obstacle;
		
        

        public Item(string[] data)
        {
            if (data.Length > 0) int.TryParse(data[0].Trim(), out id);
			name = data.Length <= 1 + 1 ? "" : data[1];
			model = data.Length <= 2 + 1 ? "" : data[2];
			if (data.Length > 3) int.TryParse(data[3].Trim(), out type);
			if (data.Length > 4) int.TryParse(data[4].Trim(), out furtype);
			obstacle = data.Length <= 5 + 1 ? false : (data[5] == "true" || data[5] == "1") ? true : false;
			
        }

        static List<Item> _datas;
        static Dictionary<int, int> _dicDataIndexForId = new Dictionary<int, int>();
		static Dictionary<string, int> _dicDataIndexForName = new Dictionary<string, int>();
		static Dictionary<string, int> _dicDataIndexForModel = new Dictionary<string, int>();
		

        public static void Load(string datas)
        {
            _datas = new List<Item>();
            foreach (var data in datas.Split('\n'))
            {
                if (!string.IsNullOrEmpty(data))
                {
                    var item = new AutoData.Item(data.Split('\t'));
                    _datas.Add(item);

                    _dicDataIndexForId[item.id] = _datas.Count - 1;
					_dicDataIndexForName[item.name] = _datas.Count - 1;
					_dicDataIndexForModel[item.model] = _datas.Count - 1;
					
                }
            }
        }
        public static void Load(byte[] bytes)
        {
            _datas = new List<Item>();
            var datas = System.Text.Encoding.Unicode.GetString(bytes, 0, bytes.Length);
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
        public static Item GetForId(int id)
        {
            if (!_dicDataIndexForId.ContainsKey(id))
                return null;
            return GetIndex(_dicDataIndexForId[id]);
        }
		public static Item GetForName(string name)
        {
            if (!_dicDataIndexForName.ContainsKey(name))
                return null;
            return GetIndex(_dicDataIndexForName[name]);
        }
		public static Item GetForModel(string model)
        {
            if (!_dicDataIndexForModel.ContainsKey(model))
                return null;
            return GetIndex(_dicDataIndexForModel[model]);
        }
		
    }
}