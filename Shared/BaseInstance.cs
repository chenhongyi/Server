using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shared
{
    public  class BaseInstance<T>
    {


        //private static readonly BaseInstance<T> instance = new BaseInstance<T>();
        //static BaseInstance() { }
        //private BaseInstance() { }
        //public static BaseInstance<T> Instance
        //{
        //    get
        //    {
        //        return instance;
        //    }
        //}

        //private static BaseInstance<T> _instance;
        //private static object _lock = new object();

        //public static BaseInstance<T> Instance()
        //{
        //    if (_instance == null)
        //    {
        //        lock (_lock)
        //        {
        //            if (_instance == null)
        //            {
        //                _instance = new BaseInstance<T>();
        //            }
        //        }
        //    }
        //    return _instance;
        //}

        private static T _Instance;
        public static T Instance
        {
            get
            {
                if (_Instance == null)
                    _Instance = System.Activator.CreateInstance<T>();
                return _Instance;
            }
        }
    }
}
