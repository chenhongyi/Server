using System.Collections.Generic;
namespace AutoData
{
    public class Companyname
    {
        /// <summary>
        /// 索引
        /// </summary>
        public int Id;
		/// <summary>
        /// 地名
        /// </summary>
        public string WhereArea;
		/// <summary>
        /// 名字
        /// </summary>
        public string Name;
		/// <summary>
        /// 类型
        /// </summary>
        public string Nature;
		
        

        public Companyname(string[] data)
        {
            if (data.Length > 0) int.TryParse(data[0].Trim(), out Id);
			WhereArea = data.Length <= 1 + 1 ? "" : data[1];
			Name = data.Length <= 2 + 1 ? "" : data[2];
			Nature = data.Length <= 3 + 1 ? "" : data[3];
			
        }

        static List<Companyname> _datas;
        static Dictionary<int, int> _dicDataIndexForId = new Dictionary<int, int>();
		

        public static void Load(string datas)
        {
            _datas = new List<Companyname>();
            foreach (var data in datas.Split('\n'))
            {
                if (!string.IsNullOrEmpty(data))
                {
                    var item = new AutoData.Companyname(data.Split('\t'));
                    _datas.Add(item);

                    _dicDataIndexForId[item.Id] = _datas.Count - 1;
					
                }
            }
        }
        public static void Load(byte[] bytes)
        {
            _datas = new List<Companyname>();
            var datas = System.Text.UTF8Encoding.UTF8.GetString(bytes, 0, bytes.Length);
            Load(datas);
        }
        public static List<Companyname> GetAll()
        {
            return _datas;
        }
        public static Companyname GetIndex(int index)
        {
            return _datas[index];
        }
        public static Companyname GetForId(int Id)
        {
            if (!_dicDataIndexForId.ContainsKey(Id))
                return null;
            return GetIndex(_dicDataIndexForId[Id]);
        }
		
    }
}