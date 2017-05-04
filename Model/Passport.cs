using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Model
{
    public class Passport
    {
        public string PassportID { get; set; }

        public Guid AccountID { get; set; }

        public string IMEI { get; set; }
    }
}
