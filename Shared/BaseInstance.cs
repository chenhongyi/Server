using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shared
{
    public class BaseInstance<T>
    {
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
