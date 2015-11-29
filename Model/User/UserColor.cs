using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Weather.Model
{
    public class UserColor
    {
        public string isSelected { get; set; }
        public List<SingleColor> SingleColors { get; set; }
    }
    public class SingleColor
    {
        public int R { get; set; }
        public int G { get; set; }
        public int B { get; set; }
        public int A { get; set; }
        public string colorStr { get; set; }
        public string Ext { get; set; }

    }
}
