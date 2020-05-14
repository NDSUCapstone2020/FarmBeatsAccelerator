using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FarmBeatsAccelerator.Classes
{
    public class DataMapping
    {

            public DateTime[] timestamps { get; set; }
            public Property[] properties { get; set; }
      

        public class Property
        {
            public double[] values { get; set; }
            public string name { get; set; }
            public string type { get; set; }
            public double[] getValues()
            {
                return this.values;
            }
        }

    }
}
