using System.Collections.Generic;
namespace AutoData
{
    public class LocalString
    {
        /// <summary>
        /// id
        /// </summary>
        public int id;
		/// <summary>
        /// 文本
        /// </summary>
        public string text;
		
        

        public LocalString(string[] data)
        {
            if (data.Length > 0) int.TryParse(data[0].Trim(), out id);
			text = data.Length <= 1 + 1 ? "" : data[1];
			
        }

        static List<LocalString> _datas;
        static Dictionary<int, int> _dicDataIndexForId = new Dictionary<int, int>();
		

        public static void Load(string datas)
        {
            _datas = new List<LocalString>();
            foreach (var data in datas.Split('\n'))
            {
                if (!string.IsNullOrEmpty(data))
                {
                    var item = new AutoData.LocalString(data.Split('\t'));
                    _datas.Add(item);

                    _dicDataIndexForId[item.id] = _datas.Count - 1;
					
                }
            }
        }
        public static void Load(byte[] bytes)
        {
            _datas = new List<LocalString>();
            var datas = System.Text.UTF8Encoding.UTF8.GetString(bytes, 0, bytes.Length);
            Load(datas);
        }
        public static List<LocalString> GetAll()
        {
            return _datas;
        }
        public static LocalString GetIndex(int index)
        {
            return _datas[index];
        }
        public static LocalString GetForId(int id)
        {
            if (!_dicDataIndexForId.ContainsKey(id))
                return null;
            return GetIndex(_dicDataIndexForId[id]);
        }
		
    }
}