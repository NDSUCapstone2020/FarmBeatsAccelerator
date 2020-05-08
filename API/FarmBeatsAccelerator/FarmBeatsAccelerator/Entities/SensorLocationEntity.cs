using Microsoft.Azure.Cosmos.Table;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FarmBeatsAccelerator.Entities
{
    public class SensorLocationEntity : TableEntity
    {
        public int SkuId { get; set; }
        public double longitude { get; set; }
        public double latitude { get; set; }
        public override string ToString()
        {

            return SkuId.ToString();
        }
    }
}
