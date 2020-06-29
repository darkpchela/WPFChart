using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Chart.DataClasses
{
    [Serializable]
    public class UserData
    {
        public int? dayNum { get; set; }
        public int Rank { get; set; }
        public string User { get; set; }
        public string Status { get; set; }
        public int Steps { get; set; }
    }
}
