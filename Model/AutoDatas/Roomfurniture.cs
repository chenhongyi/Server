using System.Collections.Generic;
namespace AutoData
{
    public class Roomfurniture
    {
        /// <summary>
        /// 主键
        /// </summary>
        public int key;
		/// <summary>
        /// 类型
        /// </summary>
        public int type;
		/// <summary>
        /// 原点
        /// </summary>
        public Vector3Cls pos;
		/// <summary>
        /// 旋转
        /// </summary>
        public Vector3Cls rotation;
		/// <summary>
        /// 缩放
        /// </summary>
        public Vector3Cls scale;
		/// <summary>
        /// 按钮坐标
        /// </summary>
        public Vector3Cls btn;
		/// <summary>
        /// 地板模型
        /// </summary>
        public string model;
		
        public class Vector3Cls
        {
            /// <summary>
        /// x
        /// </summary>
        public float x;
		/// <summary>
        /// y
        /// </summary>
        public float y;
		/// <summary>
        /// z
        /// </summary>
        public float z;
		
            public Vector3Cls(string[] data)
            {
            if (data.Length > 0) float.TryParse(data[0].Trim(), out x);
			if (data.Length > 1) float.TryParse(data[1].Trim(), out y);
			if (data.Length > 2) float.TryParse(data[2].Trim(), out z);
			
            }
        }
		

        public Roomfurniture(string[] data)
        {
            if (data.Length > 0) int.TryParse(data[0].Trim(), out key);
			if (data.Length > 1) int.TryParse(data[1].Trim(), out type);
			pos = new Vector3Cls(data[2].Split(','));
			rotation = new Vector3Cls(data[3].Split(','));
			scale = new Vector3Cls(data[4].Split(','));
			btn = new Vector3Cls(data[5].Split(','));
			model = data.Length <= 6 + 1 ? "" : data[6];
			
        }

        static List<Roomfurniture> _datas;
        static Dictionary<int, int> _dicDataIndexForKey = new Dictionary<int, int>();
		

        public static void Load(string datas)
        {
            _datas = new List<Roomfurniture>();
            foreach (var data in datas.Split('\n'))
            {
                if (!string.IsNullOrEmpty(data))
                {
                    var item = new AutoData.Roomfurniture(data.Split('\t'));
                    _datas.Add(item);

                    _dicDataIndexForKey[item.key] = _datas.Count - 1;
					
                }
            }
        }
        public static void Load(byte[] bytes)
        {
            _datas = new List<Roomfurniture>();
            var datas = System.Text.UTF8Encoding.UTF8.GetString(bytes, 0, bytes.Length);
            Load(datas);
        }
        public static List<Roomfurniture> GetAll()
        {
            return _datas;
        }
        public static Roomfurniture GetIndex(int index)
        {
            return _datas[index];
        }
        public static Roomfurniture GetForKey(int key)
        {
            if (!_dicDataIndexForKey.ContainsKey(key))
                return null;
            return GetIndex(_dicDataIndexForKey[key]);
        }
		
    }
}