using Microsoft.Azure.Cosmos.Table;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FarmBeatsAccelerator.Entities
{
    public class SensorDataEntity : TableEntity
    {
        public int SkuId { get; set; }
        public double CompChannel0 { get; set; }
        public double CompChannel1 { get; set; }
        public double CompChannel2 { get; set; }
        public double CompChannel3 { get; set; }
        public double CompChannel4 { get; set; }
        public double CompChannel5 { get; set; }
        public double CompChannel6 { get; set; }
        public double CompChannel7 { get; set; }
        public double CompChannel8 { get; set; }
        public double CompChannel9 { get; set; }
        public double CompChannel10 { get; set; }
        public double CompChannel11 { get; set; }
        public double CompChannel12 { get; set; }
        public double CompChannel13 { get; set; }
        public DateTime TimeStamp { get; set; }
        public SensorDataEntity()
        {
        }
        public override string ToString()
        {
            return SkuId.ToString() + " " + CompChannel0.ToString() + " " + CompChannel1.ToString() + " " + CompChannel2.ToString() + " " + CompChannel3.ToString() + " " + CompChannel4.ToString() + " " + CompChannel5.ToString() + " " + CompChannel6.ToString() + " " + CompChannel7.ToString() + " " + CompChannel8.ToString() + " " + CompChannel9.ToString() + " " + CompChannel10.ToString() + " " + CompChannel11.ToString() + " " + CompChannel12.ToString() + " " + CompChannel13.ToString();
        }
    }
}

